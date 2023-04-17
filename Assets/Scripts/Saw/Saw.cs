using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saw : MonoBehaviour
{
	[SerializeField] private float movementSpeed;
	[SerializeField] private AudioEvent cutDownTreeSFX;

	private void Start()
	{
		ScoreManager.Instance.LevelEnded += OnLevelEnd;
	}

	private void Update()
	{
		transform.position += movementSpeed * Time.deltaTime * transform.forward;
	}

	private void OnTriggerEnter(Collider other)
	{
		var tree = other.GetComponentInParent<Tree>();
		if (tree)
		{
			ScoreManager.Instance.RemoveTree(tree);
			AudioManager.Instance.PlayOnce(cutDownTreeSFX);
			Destroy(tree.gameObject);
		}
	}

	private void OnDestroy()
	{
		ScoreManager.Instance.LevelEnded -= OnLevelEnd;
	}

	private void OnLevelEnd()
	{
		if (this != null)
			Destroy(gameObject);
	}
}

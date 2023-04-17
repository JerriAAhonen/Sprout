using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class CursorController : MonoBehaviour
{
	[SerializeField] private TreeInfo treeInfo;
	[SerializeField] private LayerMask treeLayer;

	private Tree currentTree;

	private void Start()
	{
		ScoreManager.Instance.LevelEnded += OnLevelEnd;
	}

	private void Update()
	{
		if (!ScoreManager.Instance.LevelOngoing)
			return;

		var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out var hit, Mathf.Infinity, treeLayer))
		{
			var newTree = hit.collider.GetComponentInParent<Tree>();
			if (newTree != null && newTree != currentTree)
			{
				OnHoverOverNewTree(newTree);
			}
		}
		else if (currentTree != null)
		{
			OnStopHovering();
		}
	}

	private void OnHoverOverNewTree(Tree newTree)
	{
		OnStopHovering();

		newTree.ShowOutline(true);
		treeInfo.Show(newTree.transform.position, newTree.Water, newTree.Fertilizer, newTree.CurrentState == TreeState.Mature);
		currentTree = newTree;

		currentTree.VitalsUpdated += OnVitalsUpdated;
		currentTree.Dead += OnDead;
	}

	private void OnStopHovering()
	{
		treeInfo.Hide();
		if (currentTree == null)
			return;

		currentTree.VitalsUpdated -= OnVitalsUpdated;
		currentTree.Dead -= OnDead;
		currentTree.ShowOutline(false);
		currentTree = null;
	}

	private void OnVitalsUpdated()
	{
		treeInfo.Show(currentTree.transform.position, currentTree.Water, currentTree.Fertilizer, currentTree.CurrentState == TreeState.Mature);
	}

	private void OnDead()
	{
		OnStopHovering();
	}

	private void OnLevelEnd()
	{
		OnStopHovering();
		ScoreManager.Instance.LevelEnded -= OnLevelEnd;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SawSpawner : MonoBehaviour
{
	[SerializeField] private Saw sawPrefab;
	[SerializeField] private float spawnDistMax;

	public void Spawn()
	{
		var spawnPos = transform.position + Random.insideUnitSphere * Random.Range(0, spawnDistMax);
		spawnPos.y = transform.position.y;

		var saw = Instantiate(sawPrefab, spawnPos, Quaternion.identity);
		saw.transform.rotation = transform.rotation;
	}

#if UNITY_EDITOR

	private void OnDrawGizmos()
	{
		Handles.DrawWireDisc(transform.position, Vector3.up, spawnDistMax);

		var startPos = transform.position + transform.right * spawnDistMax;
		Debug.DrawLine(startPos, startPos + transform.forward * 20);

		startPos = transform.position - transform.right * spawnDistMax;
		Debug.DrawLine(startPos, startPos + transform.forward * 20);
	}

#endif
}

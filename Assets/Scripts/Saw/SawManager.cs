using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SawManager : MonoBehaviour
{
	[SerializeField] private float sawsPerTreePerMinute;

	private int treeCount;
	private float sawsPerMNinute;
	private float timer;
	private float spawnInterval;
	private List<SawSpawner> spawners = new();

	private void Start()
	{
		foreach (var spawner in GetComponentsInChildren<SawSpawner>())
			spawners.Add(spawner);

		ScoreManager.Instance.CurrentTreeCountChanged += OnCurrentTreeCountChanged;
	}

	private void Update()
	{
		if (!ScoreManager.Instance.LevelOngoing)
			return;

		if (treeCount <= 0)
			return;

		timer += Time.deltaTime;
		if (timer > spawnInterval)
		{
			Spawn();
			timer = 0f;
		}
	}

	private void OnCurrentTreeCountChanged(int trees)
	{
		treeCount = trees;
		UpdateSpawnInterval();
	}

	private void UpdateSpawnInterval()
	{
		sawsPerMNinute = CalculateSawsPerMinute(treeCount);
		spawnInterval = 60f / sawsPerMNinute;
	}

	private float CalculateSawsPerMinute(int trees)
	{
		return trees * sawsPerTreePerMinute;
	}

	private void Spawn()
	{
		spawners.Random().Spawn();
	}

	private void OnDestroy()
	{
		ScoreManager.Instance.CurrentTreeCountChanged -= OnCurrentTreeCountChanged;
	}
}

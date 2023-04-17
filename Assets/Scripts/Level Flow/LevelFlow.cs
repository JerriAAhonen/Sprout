using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LevelFlow : MonoBehaviour
{
	public abstract int Goal { get; }

	protected virtual IEnumerator Start()
	{
		ScoreManager.Instance.TreeStateChanged += OnTreeStateChanged;
		ScoreManager.Instance.WaitForLevelLoaded(OnLevelLoaded);

		yield return null;

		void OnLevelLoaded()
		{
			StartCoroutine(ScoreManager.Instance.UI.OnLevelStart(Goal, OnStart));
		}

		void OnStart()
		{
			ScoreManager.Instance.OnLevelStart();
		}
	}

	private void OnTreeStateChanged()
	{
		if (ScoreManager.Instance.Trees.Count < Goal)
		{
			return;
		}

		int fullyGrownTrees = 0;
		foreach (var tree in ScoreManager.Instance.Trees)
		{
			if (tree.CurrentState == TreeState.Mature)
				fullyGrownTrees++;
		}

		if (fullyGrownTrees < Goal)
			return;

		LevelEnd();
	}

	public void LevelEnd()
	{
		ScoreManager.Instance.TreeStateChanged -= OnTreeStateChanged;
		ScoreManager.Instance.OnLevelEnd();
		StartCoroutine(ScoreManager.Instance.UI.OnLevelEnd());
	}
}

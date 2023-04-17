using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum TreeEvent
{
	Planted,
	Died_Water,
	Died_Fertilizer,
	Died_Saw,
}

public class ScoreManager : Singleton<ScoreManager>
{
	private LevelUI ui;
	private readonly List<Tree> trees = new();
	private bool levelLoaded;
	private readonly List<Action> levelLoadedListeners = new();

	public event Action<int> CurrentTreeCountChanged;
	public event Action TreeStateChanged;
	public event Action LevelStarted;
	public event Action LevelEnded;
    
	public bool LevelOngoing { get; private set; }
	public int CurrentScore { get; private set; }
	public IReadOnlyList<Tree> Trees => trees;
	public LevelUI UI => ui;

	private void Start()
	{
		DontDestroyOnLoad(this);
		LevelLoader.LevelLoaded += OnLevelLoaded;
		LevelOngoing = false;
	}

	public void ModifyScore(int delta)
    {
		if (!LevelOngoing) 
			return;

		var oldScore = CurrentScore;
        CurrentScore = Mathf.Max(0, CurrentScore + delta);
		ui.SetScore(oldScore, CurrentScore);
	}

	public void OnLevelLoaded(int levelIndex)
	{
		if (levelIndex != 0)
			ui = GameObject.FindGameObjectWithTag(LevelUI.Tag).GetComponent<LevelUI>();

		ResetScore();

		trees.Clear();
		CurrentTreeCountChanged?.Invoke(trees.Count);

		LevelOngoing = false;
		levelLoaded = true;
		foreach (var listener in levelLoadedListeners)
			listener?.Invoke();
		levelLoadedListeners.Clear();
	}

	public void WaitForLevelLoaded(Action onLoaded)
	{
		if (levelLoaded)
			onLoaded();
		else
			levelLoadedListeners.Add(onLoaded);
	}

	public void OnLevelStart()
	{
		LevelOngoing = true;
		LevelStarted?.Invoke();
	}

	public void OnLevelEnd()
	{
		LevelOngoing = false;
		LevelEnded?.Invoke();
		levelLoaded = false;
	}

	public void AddTree(Tree tree)
	{
		trees.Add(tree);
		CurrentTreeCountChanged?.Invoke(trees.Count);
		tree.StateChanged += OnTreeStateChanged;
	}

	public void RemoveTree(Tree tree)
	{
		trees.Remove(tree);
		CurrentTreeCountChanged?.Invoke(trees.Count);
		tree.StateChanged -= OnTreeStateChanged;
	}

	private void OnTreeStateChanged(TreeState state)
	{
		TreeStateChanged?.Invoke();
	}

	private void ResetScore()
	{
		CurrentScore = 0;
	}
}

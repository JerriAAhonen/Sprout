using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class LevelLoader
{
	private class LoadingMonoBehaviour : MonoBehaviour { }

	public static event Action<int> LevelLoaded;

	private static GameObject loaderGO;
	private static LoadingCanvas lc;

	private static int currentLevel;

	public static void LoadLevel(int level)
	{
		if (loaderGO == null)
		{
			loaderGO = new("Scene Loader");
			var loadingCanvas = Resources.Load<GameObject>("LoadingCanvas");
			lc = GameObject.Instantiate(loadingCanvas, loaderGO.transform).GetComponent<LoadingCanvas>();
			UnityEngine.Object.DontDestroyOnLoad(loaderGO);
			//Debug.Break();
		}

		loaderGO.GetOrAddComponent<LoadingMonoBehaviour>().StartCoroutine(LoadRoutine(level));

		currentLevel = level;
	}

	public static void LoadNextLevel()
	{
		if (SceneManager.sceneCountInBuildSettings >= currentLevel + 2)
			LoadLevel(currentLevel + 1);
		else
			LoadMainMenu();
	}

	public static void ReloadLevel()
	{
		LoadLevel(currentLevel);
	}

	public static void LoadMainMenu()
	{
		LoadLevel(0);
	}

	private static IEnumerator LoadRoutine(int levelIndex)
	{
		lc.SetVisible(true, 0.5f);
		yield return new WaitForSeconds(0.5f);

		var asyncOperation = SceneManager.LoadSceneAsync(levelIndex);
		asyncOperation.allowSceneActivation = true;

		var elapsed = 0f;
		while (!asyncOperation.isDone)
		{
			elapsed += Time.deltaTime;
			yield return null;
		}

		lc.SetVisible(false, 0.5f);

		yield return new WaitForSeconds(0.5f);

		LevelLoaded?.Invoke(levelIndex);
	}
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ListExtensions
{
	public static T Random<T>(this List<T> self)
	{
		if (self.Count == 0)
			return default;
		return self[UnityEngine.Random.Range(0, self.Count)];
	}
}

public static class GameObjectExtensions
{
	public static T GetOrAddComponent<T>(this GameObject go) where T : Component
	{
		return go.GetComponent<T>() ?? go.AddComponent<T>();
	}
}

public static class CanvasGroupExtensions
{
	public static void Toggle(this CanvasGroup self, bool visible)
	{
		self.alpha= visible ? 1f : 0f;
		self.interactable = visible;
		self.blocksRaycasts = visible;
	}
}

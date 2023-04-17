using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TreeInfo : MonoBehaviour
{
	[SerializeField] private CanvasGroup rootCG;
	[SerializeField] private CanvasGroup vitalsCG;
	[SerializeField] private CanvasGroup fullyGrownCG;
	[SerializeField] private List<Image> waterImage;
	[SerializeField] private Image fertilizerImage;

	private RectTransform rt;

	private void Start()
	{
		rt = GetComponent<RectTransform>();
		Hide();
	}

	public void Show(Vector3 worldPos, float water, float fertilizer, bool fullyGrown)
	{
		Vector2 screenPoint = Camera.main.WorldToScreenPoint(worldPos);
		rt.position = screenPoint;

		if (fullyGrown)
		{
			vitalsCG.alpha = 0f;
			fullyGrownCG.alpha = 1f;
		}
		else
		{
			vitalsCG.alpha = 1f;
			fullyGrownCG.alpha = 0f;
			foreach (var item in waterImage)
				item.fillAmount = water;
			fertilizerImage.fillAmount = fertilizer;
		}

		SetVisible(true);
	}

	public void Hide() => SetVisible(false);

	private void SetVisible(bool visible) => rootCG.alpha = visible ? 1 : 0;
}

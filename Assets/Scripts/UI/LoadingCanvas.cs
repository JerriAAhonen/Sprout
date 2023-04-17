using UnityEngine;

public class LoadingCanvas : MonoBehaviour
{
	[SerializeField] private CanvasGroup cg;

	private void Awake() => SetVisible(false, 0f);

	public void SetVisible(bool visible, float animDur)
	{
		if (animDur <= 0f)
		{
			cg.alpha = visible ? 1f : 0f;
			cg.interactable = visible;
			cg.blocksRaycasts = visible;
			return;
		}

		LeanTween.value(visible ? 0f : 1f, visible ? 1f : 0f, animDur).setOnUpdate(v => cg.alpha = v)
			.setOnComplete(() =>
			{
				cg.interactable = visible;
				cg.blocksRaycasts = visible;
			});
	}
}

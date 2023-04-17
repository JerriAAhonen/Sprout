using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUI : MonoBehaviour
{
	// Name of the Tag on the gameobject
	public const string Tag = "LevelUI";

	[SerializeField] private TextMeshProUGUI scoreLabel;
	[SerializeField] private CanvasGroup scoreLabelCG;
	[SerializeField] private Image reloadIndicator;
	[Header("Level Start")]
	[SerializeField] private CanvasGroup levelStartCG;
	[SerializeField] private CanvasGroup goalLabelCG;
	[SerializeField] private TextMeshProUGUI goalLabel;
	[Header("Level End")]
	[SerializeField] private CanvasGroup levelEndCG;
	[SerializeField] private TextMeshProUGUI endScore;
	[SerializeField] private CanvasGroup levelEndButtonsCG;
	[SerializeField] private Button restartButton;
	[SerializeField] private Button nextButton;
	[SerializeField] private Button menuButton;

	private Coroutine animationRoutine;
	private int reloadID;

	private void Awake()
	{
		levelStartCG.alpha = 1.0f;
		reloadIndicator.fillAmount = 0f;
	}

	/// <summary>
	/// Sets and animates score shown at the top of the screen
	/// </summary>
	/// <param name="oldScore">from</param>
	/// <param name="currentScore">to</param>
	public void SetScore(int oldScore, int currentScore)
	{
		if (animationRoutine != null)
			StopCoroutine(animationRoutine);

		animationRoutine = StartCoroutine(AnimateNumber(oldScore, currentScore, 0.5f, scoreLabel));
	}

	/// <summary>
	/// Show level goal text before fading out of black
	/// </summary>
	/// <param name="goal">How many trees needs to be grown</param>
	/// <param name="onComplete">Called when bleck is faded out</param>
	/// <returns></returns>
	public IEnumerator OnLevelStart(int goal, Action onComplete)
	{
		var newText = goalLabel.text.Replace("*", goal.ToString());
		goalLabel.text = newText;

		LeanTween.value(0f, 1f, 0.2f)
			.setOnUpdate(v => goalLabelCG.alpha = v);

		yield return new WaitForSeconds(2f);

		LeanTween.value(1f, 0f, 0.5f)
			.setOnUpdate(v => levelStartCG.alpha = v)
			.setOnComplete(onComplete);
	}

	/// <summary>
	/// End of level sequence
	/// </summary>
	/// <returns></returns>
	public IEnumerator OnLevelEnd()
	{
		float animDur = 0.5f;
		LeanTween.value(1f, 0f, animDur).setOnUpdate(v => scoreLabelCG.alpha = v);

		yield return new WaitForSeconds(animDur);

		endScore.text = "0";
		LeanTween.value(0f, 1f, animDur).setOnUpdate(v => levelEndCG.alpha = v);

		yield return new WaitForSeconds(animDur);

		StartCoroutine(AnimateNumber(0, ScoreManager.Instance.CurrentScore, 1f, endScore));

		yield return new WaitForSeconds(1f);

		LeanTween.value(0f, 1f, animDur).setOnUpdate(v => levelEndButtonsCG.alpha = v);

		restartButton.onClick.AddListener(LevelLoader.ReloadLevel);
		nextButton.onClick.AddListener(LevelLoader.LoadNextLevel);
		menuButton.onClick.AddListener(LevelLoader.LoadMainMenu);
	}

	/// <summary>
	/// Animate an int changing value
	/// </summary>
	/// <param name="startNumber">From</param>
	/// <param name="endNumber">To</param>
	/// <param name="animationDuration">How long does the animation play</param>
	/// <param name="label">Component to animate</param>
	/// <returns></returns>
	private IEnumerator AnimateNumber(int startNumber, int endNumber, float animationDuration, TextMeshProUGUI label)
	{
		float offset = (endNumber - startNumber) / animationDuration;
		float displayedNumber = startNumber;

		for (float t = 0; t < animationDuration; t += Time.deltaTime)
		{
			displayedNumber += offset * Time.deltaTime;
			label.text = Mathf.Round(displayedNumber).ToString();
			yield return null;
		}

		label.text = Mathf.Round(endNumber).ToString();
		animationRoutine = null;
	}

	/// <summary>
	/// Animates cannon reload icon
	/// </summary>
	/// <param name="dur">Reload duration</param>
	public void AnimateReload(float dur)
	{
		LeanTween.cancel(reloadID);

		Cursor.visible = false;

		reloadID = LeanTween.value(0f, 1f, dur)
			.setOnUpdate(v => reloadIndicator.fillAmount = v)
			.setOnComplete(() => {
				reloadIndicator.fillAmount = 0f;
				Cursor.visible = true;
			})
			.uniqueId;
	}
}

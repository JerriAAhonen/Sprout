using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TreeState
{
	Sapling, Young, Mature
}

public class Tree : MonoBehaviour
{
	[SerializeField] private float transitionDuration;
	[SerializeField] private float growthDuration;
	[SerializeField] private AudioEvent growSFX;
	[SerializeField] private float waterConsumptionPerSecond;
	[SerializeField] private float fertilizerConsumptionPerSecond;
	[SerializeField] private float stateMultiplierForConsumption;
	[Header("Scales")]
	[SerializeField] private float saplingScale;
	[SerializeField] private float youngScale;
	[SerializeField] private float matureScale;
	[SerializeField] private float scaleVariance;
	[Space]
	[SerializeField] private SphereCollider proximityCollider;
	[SerializeField] private Outline outline;

	private int growthTweenId;
	public TreeState CurrentState { get; private set; }
	public event Action<TreeState> StateChanged;

	// Percentual amount of vitals, range 0...1
	public float Water { get; private set; } = 1;
	public float Fertilizer { get; private set; } = 1;

	public event Action VitalsUpdated;
	public event Action Dead;

	private void Start()
	{
		ShowOutline(false);
	}

	private void Update()
	{
		// The multiplier decreases as the tree grows
		var mult = stateMultiplierForConsumption * ( 3 - (int)CurrentState);
		var waterDelta = -waterConsumptionPerSecond * Time.deltaTime * mult;
		var fertilizerDelta = -fertilizerConsumptionPerSecond * Time.deltaTime * mult;
		
		if (!ModifyVitals(waterDelta, fertilizerDelta))
			Die();
	}

	public void Plant()
	{
		CurrentState = TreeState.Sapling;
		transform.localScale = Vector3.zero;

		AnimateStateTransition(saplingScale);
		StartCoroutine(GrowthRoutine(FromSpalingToYoung));

		ScoreManager.Instance.ModifyScore(10);
		ScoreManager.Instance.AddTree(this);
	}

	public void ShowOutline(bool show)
	{
		outline.OutlineWidth = show ? 2 : 0;
	}

	/// <summary>
	/// Modify the Water and Fertilizer levels
	/// </summary>
	/// <param name="waterDelta">Water amount to change</param>
	/// <param name="fertilizerDelta">Fertilizer amount to change</param>
	/// <returns>True if the tree is still alive after the modifications</returns>
	public bool ModifyVitals(float waterDelta, float fertilizerDelta)
	{
		Water = Mathf.Clamp01(Water + waterDelta);
		Fertilizer = Mathf.Clamp01(Fertilizer + fertilizerDelta);

		VitalsUpdated?.Invoke();

		return Water > 0 && Fertilizer > 0;
	}

	private void FromSpalingToYoung()
	{
		AudioManager.Instance.PlayOnce(growSFX);

		ChangeState(TreeState.Young);

		AnimateStateTransition(youngScale);
		StartCoroutine(GrowthRoutine(FromYoungToMature));

		ScoreManager.Instance.ModifyScore(10);
	}

	private void FromYoungToMature()
	{
		AudioManager.Instance.PlayOnce(growSFX);

		ChangeState(TreeState.Mature);
		ModifyVitals(1, 1);

		AnimateStateTransition(matureScale);

		ScoreManager.Instance.ModifyScore(10);
	}

	private void AnimateStateTransition(float newScale)
	{
		var variance = UnityEngine.Random.Range(-scaleVariance, scaleVariance);

		growthTweenId = LeanTween.value(0f, newScale, transitionDuration)
			.setOnUpdate(v => transform.localScale = Vector3.one * (v + variance))
			.setEase(LeanTweenType.easeOutElastic).uniqueId;
	}

	private IEnumerator GrowthRoutine(Action onComplete)
	{
		float elapsed = 0f;
		while (elapsed < growthDuration)
		{
			yield return null;
			elapsed += Time.deltaTime;
		}

		onComplete?.Invoke();
	}

	private void ChangeState(TreeState newState)
	{
		CurrentState = newState;
		StateChanged?.Invoke(CurrentState);
	}

	private void Die()
	{
		Dead?.Invoke();
		Destroy(gameObject);
	}

	private void OnDestroy()
	{
		LeanTween.cancel(growthTweenId);
		if (ScoreManager.Instance == null)
			return;

		switch (CurrentState)
		{
			case TreeState.Sapling:
				ScoreManager.Instance.ModifyScore(-10);
				break;
			case TreeState.Young:
				ScoreManager.Instance.ModifyScore(-20);
				break;
			case TreeState.Mature:
				ScoreManager.Instance.ModifyScore(-30);
				break;
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level01Flow : LevelFlow
{
	[SerializeField] private CanvasGroup tutorialCG;

	public override int Goal => 3;

	protected override IEnumerator Start()
	{
		// Show tutorial text
		tutorialCG.Toggle(true);

		while (!Input.GetMouseButton(0))
		{
			yield return null;
		}

		tutorialCG.Toggle(false);
		
		StartCoroutine(base.Start());
	}
}

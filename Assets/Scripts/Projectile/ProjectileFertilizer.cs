using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileFertilizer : Projectile
{
	[SerializeField] private float effectiveRadius;

	protected override void OnCollision(Collision collision)
	{
		var hits = Physics.OverlapSphere(transform.position, effectiveRadius);
		foreach (var hit in hits)
		{
			if (hit.gameObject.TryGetComponent<Tree>(out var tree))
			{
				tree.ModifyVitals(0, 1);
			}
		}

		base.OnCollision(collision);
	}
}

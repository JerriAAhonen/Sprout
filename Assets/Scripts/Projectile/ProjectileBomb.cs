using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBomb : Projectile
{
	[SerializeField] private float effectiveRadius;

	protected override void OnCollision(Collision collision)
	{
		var hits = Physics.OverlapSphere(transform.position, effectiveRadius);
		foreach (var hit in hits)
		{
			if (hit.gameObject.TryGetComponent<Saw>(out var saw))
			{
				Destroy(saw.gameObject);
			}
		}

		base.OnCollision(collision);
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(transform.position, effectiveRadius);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSapling : Projectile
{
	[SerializeField] private Tree plantablePrefab;
	[SerializeField] private float minPlantingDistance;
	[SerializeField] private LayerMask treeLayer;

	protected override void OnCollision(Collision collision)
	{
		var landingPos = collision.contacts[0].point;
		var plantingPos = new Vector3(landingPos.x, 0f, landingPos.z);

		var canPlant = !Physics.CheckSphere(plantingPos, minPlantingDistance, treeLayer);
		if (canPlant)
		{
			var plantingRot = new Vector3(0f, Random.Range(0f, 359f), 0f);
			var planted = Instantiate(plantablePrefab, plantingPos, Quaternion.Euler(plantingRot));
			planted.Plant();
		}
		else
		{
			Debug.Log("Too close to another tree!");
		}

		base.OnCollision(collision);
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cannon : MonoBehaviour
{
	[SerializeField] private Transform shootpoint;
	[SerializeField] private ParticleSystem shootPS;
	[SerializeField] private AudioEvent shootSFX;
	[SerializeField] private Transform cannonGO;
	[SerializeField] private float maxShootForce;
	[SerializeField] private Vector3 projectileTorque;
	[SerializeField] private float shootForceDelta;
	[SerializeField] private float rotationSpeed;
	[Space]
	[SerializeField] private Image shootForceIndicator;

	private Plane plane; // Plane for calculating mouse position ingame
	private float currentShootForce;
	private ProjectileType currentProjectileType;
	private GameObject currentProjectile;
	private Rigidbody currentProjectileRB;
	private bool loadingNextProjectile;
	private Coroutine loadingRoutine;

	#region MonoBehaviour

	private void Start()
	{
		plane = new Plane(Vector3.up, Vector3.zero);

		UpdateShootForceIndicator();
		ScoreManager.Instance.WaitForLevelLoaded(OnLevelLoaded);
	}

	private void Update()
	{
		if (!ScoreManager.Instance.LevelOngoing) 
			return;

		if (!Input.GetMouseButton(1))
		{
			// Calculate mouse position
			Vector3 mousePos = Vector3.zero;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			if (plane.Raycast(ray, out var dist))
				mousePos = ray.GetPoint(dist);

			// Calculate target direction so that the cannon points towards the mouse
			var targetDir = mousePos - transform.position;
			var angleToMouse = Vector3.SignedAngle(Vector3.forward, targetDir, Vector3.up);

			// Correct the cannon rotation since it starts pointing away from the middle
			angleToMouse += 180;

			// Set cannon rotation
			cannonGO.rotation = Quaternion.Lerp(cannonGO.rotation, Quaternion.Euler(-45, angleToMouse, 0), Time.deltaTime * rotationSpeed);
		}

		if (loadingNextProjectile)
			return;

		if (Input.GetMouseButton(0))
		{
			currentShootForce = Mathf.Min(currentShootForce + Time.deltaTime * shootForceDelta, maxShootForce);
			UpdateShootForceIndicator();
		}

		if (Input.GetMouseButtonUp(0))
		{
			// Shoot the projectile at the direction of shootpoint
			currentProjectile.transform.parent = null;
			currentProjectileRB.isKinematic = false;

			// Rigidbody forces
			currentProjectileRB.AddForce(currentProjectile.transform.forward * currentShootForce, ForceMode.Impulse);
			currentProjectileRB.AddRelativeTorque(projectileTorque, ForceMode.Impulse);
			
			// Effects
			shootPS.Play();
			AudioManager.Instance.PlayOnce(shootSFX);

			// Remove references to projectile after it has been shot from the cannon
			currentProjectile = null;
			currentProjectileRB = null;

			StartLoadRoutine(currentProjectileType);
		}
	}

	#endregion

	public void StartLoadRoutine(ProjectileType type)
	{
		if (loadingRoutine != null)
			StopCoroutine(loadingRoutine);

		loadingRoutine = StartCoroutine(LoadRoutine(type));
	}

	private void OnLevelLoaded()
	{
		StartLoadRoutine(ProjectileType.Sapling);
	}

	private void UpdateShootForceIndicator()
	{
		var indicatorFillAmount = currentShootForce / maxShootForce;
		shootForceIndicator.fillAmount = indicatorFillAmount;
	}

	private IEnumerator LoadRoutine(ProjectileType type)
	{
		if (currentProjectile != null)
			Destroy(currentProjectile);
		currentProjectile = null;

		loadingNextProjectile = true;
		currentProjectileType = type;

		currentShootForce = 0f;
		UpdateShootForceIndicator();

		ScoreManager.Instance.UI.AnimateReload(1f);

		yield return new WaitForSeconds(1);

		var prefab = Resources.Load<ProjectileDatabase>("ProjectileDatabase").GetProjectile(type).Prefab;
		currentProjectile = Instantiate(prefab, shootpoint.position, shootpoint.rotation, shootpoint);

		currentProjectileRB = currentProjectile.GetComponent<Rigidbody>();
		currentProjectileRB.isKinematic = true;

		loadingNextProjectile = false;
		loadingRoutine = null;
	}
}

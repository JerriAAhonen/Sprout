using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
	[SerializeField] private ParticleSystem impactPS;
	[SerializeField] private GameObject model;
	[SerializeField] private Rigidbody rb;
	[SerializeField] private AudioEvent impactSFX;

	protected virtual void OnCollision(Collision collision)
	{
		impactPS.transform.rotation = Quaternion.Euler(-90, 0, 0);
		impactPS.Play();

		model.SetActive(false);
		rb.isKinematic = true;
		rb.velocity = Vector3.zero;
		AudioManager.Instance.PlayOnce(impactSFX);

		LeanTween.delayedCall(impactPS.main.duration, () => Destroy(gameObject));
	}

	private void OnCollisionEnter(Collision collision)
	{
		OnCollision(collision);
	}
}

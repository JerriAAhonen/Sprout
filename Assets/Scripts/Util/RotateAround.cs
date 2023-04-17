using UnityEngine;

public class RotateAround : MonoBehaviour
{
	[SerializeField] private Vector3 rotationDelta;

	private void Update()
	{
		transform.Rotate(rotationDelta * Time.deltaTime);
	}
}

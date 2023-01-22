namespace Mapbox.Examples
{
	using UnityEngine;

	public class RotateOnYTargetTransform : MonoBehaviour
	{
		[SerializeField]
		private Transform _targetTransform;

		private void Update()
		{
			transform.eulerAngles = new Vector3(transform.eulerAngles.x, _targetTransform.eulerAngles.y, transform.eulerAngles.z);
		}
	}
}

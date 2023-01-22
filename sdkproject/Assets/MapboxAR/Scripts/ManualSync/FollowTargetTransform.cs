namespace Mapbox.Examples
{
	using UnityEngine;

	public class FollowTargetTransform : MonoBehaviour
	{
		[SerializeField]
		private Transform _targetTransform;

		private void Update()
		{
			transform.position = new Vector3(_targetTransform.position.x, transform.position.y, _targetTransform.position.z);
		}
	}
}


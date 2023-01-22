namespace Mapbox.Unity.Ar
{
	using UnityEngine;

	public class LerpAlignmentStrategy : AbstractAlignmentStrategy
	{
		[SerializeField]
		private float _followFactor;

		private Vector3 _targetPosition;
		private Quaternion _targetRotation = Quaternion.identity;
		private bool _isAlignmentAvailable = false;

		public override void OnAlignmentAvailable(Alignment alignment)
		{
			_targetPosition = alignment.Position;
			_targetRotation = Quaternion.Euler(0, alignment.Rotation, 0);
			_isAlignmentAvailable = true;
		}

		// FIXME: this should be in a coroutine, which is activated in Align.
		private void Update()
		{
			if (_isAlignmentAvailable)
			{
				var t = _followFactor * Time.deltaTime;
				_transform.SetPositionAndRotation(
					Vector3.Lerp(_transform.localPosition, _targetPosition, t),
					Quaternion.Lerp(_transform.localRotation, _targetRotation, t));
				_isAlignmentAvailable = false;
			}

		}
	}
}

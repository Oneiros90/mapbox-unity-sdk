using UnityEngine.XR.ARFoundation;

namespace Mapbox.Unity.Ar.Utilities
{
	using UnityEngine;

	[RequireComponent(typeof(LineRenderer))]
	public class PlotRoute : MonoBehaviour
	{
		[SerializeField]
		private Transform _target;

		[SerializeField]
		private Color _color;

		[SerializeField]
		private float _height;

		[SerializeField]
		private float _lineWidth = .2f;

		[SerializeField]
		private float _updateInterval;

		[SerializeField]
		private float _minDistance;

		private LineRenderer _lineRenderer;
		private float _elapsedTime;
		private int _currentIndex;
		private float _sqDistance;
		private Vector3 _lastPosition;

		private readonly ARPlaneManager _planeManager = new();

#if !UNITY_EDITOR
		bool _isStable = false;
#endif

		private void Awake()
		{
			// HACK: this needs to move somewhere else (marshal).
			_planeManager.planesChanged += AddAnchor;

			_lineRenderer = GetComponent<LineRenderer>();
			_lineRenderer.startColor = _color;
			_lineRenderer.endColor = _color;
			_lineRenderer.widthMultiplier = _lineWidth;
			_sqDistance = _minDistance * _minDistance;
		}

		private void AddAnchor(ARPlanesChangedEventArgs evt)
		{
			_planeManager.planesChanged -= AddAnchor;
			AddNode(_target.localPosition);
		}

		public void AdjustLineWidth(bool isMapMode)
		{
			var width = isMapMode ? 1f : .1f;
			_lineRenderer.widthMultiplier = width;
		}

		private void Update()
		{
#if !UNITY_EDITOR
			if (!_isStable)
			{
				return;
			}
#endif

			_elapsedTime += Time.deltaTime;
			var offset = _target.localPosition - _lastPosition;
			offset.y = 0;

			if (_elapsedTime > _updateInterval && offset.sqrMagnitude > _sqDistance)
			{
				_elapsedTime = 0f;
				AddNode(_target.localPosition);
			}
		}

		private void AddNode(Vector3 position)
		{
			if (_height > 0)
			{
				position.y = _height;
			}

			_currentIndex++;
			_lineRenderer.positionCount = _currentIndex;
			_lineRenderer.SetPosition(_currentIndex - 1, position);
			_lastPosition = position;
		}
	}
}

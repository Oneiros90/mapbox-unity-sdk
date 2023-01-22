using UnityEngine.XR.ARFoundation;

namespace Mapbox.Examples
{
	using UnityEngine;

	public class UpdateMapPosByARPlaneY : MonoBehaviour
	{
		[SerializeField]
		private Transform _mapRoot;

		private readonly ARPlaneManager _planeManager = new();

		private void Start()
		{
			_planeManager.planesChanged += UpdateMapPosOnY;
			_planeManager.planesChanged += UpdateMapPosOnY;
		}

		private void UpdateMapPosOnY(ARPlanesChangedEventArgs evt)
		{
			foreach (var added in evt.added)
			{
				var pos = _mapRoot.position;
				_mapRoot.position = new Vector3(pos.x, added.center.y, pos.z);
			}
		}
	}
}

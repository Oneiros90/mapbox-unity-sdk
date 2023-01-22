using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace UnityARInterface
{
	public class PlaceMapOnARPlane : MonoBehaviour
	{
		[SerializeField]
		private Transform _mapTransform;

		private void Start()
		{
			ARPlaneHandler.returnARPlane += PlaceMap;
			ARPlaneHandler.resetARPlane += ResetPlane;
		}

		private void PlaceMap(ARPlane plane)
		{
			if (!_mapTransform.gameObject.activeSelf)
			{
				_mapTransform.gameObject.SetActive(true);
			}

			_mapTransform.position = plane.center;
		}

		private void ResetPlane()
		{
			_mapTransform.gameObject.SetActive(false);
		}

		private void OnDisable()
		{
			ARPlaneHandler.returnARPlane -= PlaceMap;
		}
	}
}

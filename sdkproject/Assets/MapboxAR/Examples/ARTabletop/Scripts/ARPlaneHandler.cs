using UnityEngine;
using System;
using UnityEngine.XR.ARFoundation;

namespace UnityARInterface
{
	public class ARPlaneHandler : MonoBehaviour
	{
		public static Action resetARPlane;
		public static Action<ARPlane> returnARPlane;
		private ARPlane _cachedARPlane;

		private readonly ARPlaneManager _planeManager = new();

		private void Start()
		{
			_planeManager.planesChanged += UpdateARPlane;
			_planeManager.planesChanged += UpdateARPlane;
		}

		private void UpdateARPlane(ARPlanesChangedEventArgs evt)
		{
			foreach (var added in evt.added)
				UpdateARPlane(added);

			foreach (var added in evt.updated)
				UpdateARPlane(added);
		}

		private void UpdateARPlane(ARPlane arPlane)
		{
			if (_cachedARPlane == null)
				_cachedARPlane = arPlane;

			if (arPlane == _cachedARPlane)
				returnARPlane(_cachedARPlane);
		}
	}
}

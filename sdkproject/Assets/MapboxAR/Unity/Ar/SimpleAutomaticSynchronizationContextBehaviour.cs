using UnityEngine.XR.ARFoundation;

namespace Mapbox.Unity.Ar
{
	using Map;
	using Location;
	using Utils;
	using UnityEngine;
	using System;

	public class SimpleAutomaticSynchronizationContextBehaviour : MonoBehaviour, ISynchronizationContext
	{
		[SerializeField]
		private Transform _arPositionReference;

		[SerializeField]
		private AbstractMap _map;

		[SerializeField]
		private bool _useAutomaticSynchronizationBias;

		[SerializeField]
		private AbstractAlignmentStrategy _alignmentStrategy;

		[SerializeField]
		private float _synchronizationBias = 1f;

		[SerializeField]
		private float _arTrustRange = 10f;

		[SerializeField]
		private float _minimumDeltaDistance = 2f;

		[SerializeField]
		private float _minimumDesiredAccuracy = 5f;

		private SimpleAutomaticSynchronizationContext _synchronizationContext;

		private float _lastHeading;
		private float _lastHeight;

		// TODO: move to "base" class SimpleAutomaticSynchronizationContext
		// keep it here for now as map position is also calculated here
		//private KalmanLatLong _kalman = new KalmanLatLong(3); // 3:very fast walking

		private ILocationProvider _locationProvider;

		private readonly ARPlaneManager _planeManager = new();

		public event Action<Alignment> OnAlignmentAvailable = delegate { };

		public ILocationProvider LocationProvider
		{
			private get
			{
				if (_locationProvider == null)
				{
#if UNITY_EDITOR
					Debug.LogWarningFormat("SimpleAutomaticSynchronizationContextBehaviour, isRemoteConnected:{0}",
						UnityEditor.EditorApplication.isRemoteConnected);
					_locationProvider = UnityEditor.EditorApplication.isRemoteConnected
						? LocationProviderFactory.Instance.DefaultLocationProvider
						: LocationProviderFactory.Instance.EditorLocationProvider;
#else
					_locationProvider = LocationProviderFactory.Instance.DefaultLocationProvider;
#endif
				}

				return _locationProvider;
			}
			set
			{
				if (_locationProvider != null)
				{
					_locationProvider.OnLocationUpdated -= LocationProvider_OnLocationUpdated;
				}

				_locationProvider = value;
				_locationProvider.OnLocationUpdated += LocationProvider_OnLocationUpdated;
			}
		}


		private void Awake()
		{
			_alignmentStrategy.Register(this);
			_synchronizationContext = new SimpleAutomaticSynchronizationContext();
			_synchronizationContext.MinimumDeltaDistance = _minimumDeltaDistance;
			_synchronizationContext.ArTrustRange = _arTrustRange;
			_synchronizationContext.UseAutomaticSynchronizationBias = _useAutomaticSynchronizationBias;
			_synchronizationContext.SynchronizationBias = _synchronizationBias;
			_synchronizationContext.OnAlignmentAvailable += SynchronizationContext_OnAlignmentAvailable;
			_map.OnInitialized += Map_OnInitialized;

			// TODO: not available in ARInterface yet?!
			//UnityARSessionNativeInterface.ARSessionTrackingChangedEvent += UnityARSessionNativeInterface_ARSessionTrackingChanged;
			_planeManager.planesChanged += PlaneAddedHandler;
		}


		private void OnDestroy()
		{
			_alignmentStrategy.Unregister(this);
			LocationProvider.OnLocationUpdated -= LocationProvider_OnLocationUpdated;
			_planeManager.planesChanged -= PlaneAddedHandler;
		}


		private void Map_OnInitialized()
		{
			_map.OnInitialized -= Map_OnInitialized;

			// We don't want location updates until we have a map, otherwise our conversion will fail.
			LocationProvider.OnLocationUpdated += LocationProvider_OnLocationUpdated;
		}


		private void PlaneAddedHandler(ARPlanesChangedEventArgs evt)
		{
			foreach (var added in evt.added)
				_lastHeight = added.center.y;
		}

		private void LocationProvider_OnLocationUpdated(Location location)
		{
			if (location.IsLocationUpdated || location.IsUserHeadingUpdated)
			{
				// With this line, we can control accuracy of Gps updates.
				// Be aware that we only get location information if it previously met
				// the conditions of DeviceLocationProvider:
				// * desired accuracy in meters
				// * and update distance in meters
				if (location.Accuracy > _minimumDesiredAccuracy)
				{
					Unity.Utilities.Console.Instance.Log(
						"Gps update ignored due to bad accuracy: " +
						$"{location.Accuracy:0.0} > {_minimumDesiredAccuracy:0.0}"
						, "red"
					);
				}
				else
				{
					var latitudeLongitude = location.LatitudeLongitude;
					Unity.Utilities.Console.Instance.Log(
						$"Location[{UnixTimestampUtils.From(location.Timestamp):yyyyMMdd-HHmmss}]: " +
						$"{latitudeLongitude.x},{latitudeLongitude.y}\tAccuracy: {location.Accuracy}\t" +
						$"Heading: {location.UserHeading}"
						, "lightblue"
					);

					var position = _map.GeoToWorldPosition(latitudeLongitude, false);
					position.y = _map.Root.position.y;
					_synchronizationContext.AddSynchronizationNodes(location, position,
						_arPositionReference.localPosition);
				}
			}
		}


		private void SynchronizationContext_OnAlignmentAvailable(Alignment alignment)
		{
			var position = alignment.Position;
			position.y = _lastHeight;
			alignment.Position = position;
			OnAlignmentAvailable(alignment);
		}
	}
}

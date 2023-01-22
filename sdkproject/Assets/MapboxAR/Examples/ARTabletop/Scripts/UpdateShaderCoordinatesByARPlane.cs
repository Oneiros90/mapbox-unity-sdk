using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace UnityARInterface
{
	public class UpdateShaderCoordinatesByARPlane : MonoBehaviour
	{
		private static readonly int Origin = Shader.PropertyToID("_Origin");
		private static readonly int BoxRotation = Shader.PropertyToID("_BoxRotation");
		private static readonly int BoxSize = Shader.PropertyToID("_BoxSize");

		private Quaternion _rotation;
		private Vector3 _localScale, _position;

		private void Start()
		{
			ARPlaneHandler.returnARPlane += CheckCoordinates;
			ARPlaneHandler.resetARPlane += ResetShaderValues;
		}

		private void CheckCoordinates(ARPlane plane)
		{
			_position = plane.center;
			_rotation = Quaternion.Inverse(plane.transform.rotation);
			_localScale = new Vector3(plane.extents.x, 10, plane.extents.y);

			UpdateShaderValues(_position, _localScale, _rotation);
		}

		private void UpdateShaderValues(Vector3 position, Vector3 localScale, Quaternion rotation)
		{
			Shader.SetGlobalVector(Origin, new Vector4(
				position.x,
				position.y,
				position.z,
				0f));
			Shader.SetGlobalVector(BoxRotation, new Vector4(
				rotation.eulerAngles.x,
				rotation.eulerAngles.y,
				rotation.eulerAngles.z,
				0f));
			Shader.SetGlobalVector(BoxSize, new Vector4(
				localScale.x,
				localScale.y,
				localScale.z,
				0f));
		}

		private void ResetShaderValues()
		{
			var vZero = new Vector3(0, 0, 0);
			var qZero = new Quaternion(0, 0, 0, 0);

			UpdateShaderValues(vZero, vZero, qZero);
		}

		private void OnDisable()
		{
			ARPlaneHandler.returnARPlane -= CheckCoordinates;
		}
	}
}

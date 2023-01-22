using UnityEngine.SceneManagement;

namespace Mapbox.Unity.Utilities.DebugTools
{
	using UnityEngine;
	using UnityEngine.UI;

	public class BuildExamplesNavigator : MonoBehaviour
	{
		[Header("Data")]
		[SerializeField]
		private ScenesList _scenesList;

		[Header("Prefabs")]
		[SerializeField]
		private LoadSceneEntry _entryPrefab;

		[Header("References")]
		[SerializeField]
		private RectTransform _entriesContainer;

		[SerializeField]
		private Button _backButton;

		private void Awake()
		{
			foreach (var scene in _scenesList.SceneList)
			{
				if (!scene)
					return;

				var entry = Instantiate(_entryPrefab, _entriesContainer, true);
				entry.name = scene.Name;

				entry.Image = GetImageSprite(scene);
				entry.Text = scene.Name;
				entry.ClickAction = () => SceneManager.LoadScene(scene.ScenePath);
			}

			var mainSceneName = SceneManager.GetActiveScene().name;
			_backButton.onClick.AddListener(() => SceneManager.LoadScene(mainSceneName));
		}

		private static Sprite GetImageSprite(SceneData sceneData)
		{
			if (sceneData == null)
				return null;

			var sceneImage = sceneData.Image;
			if (sceneImage == null)
				return null;

			var w = sceneImage.width;
			var h = sceneImage.height;
			return Sprite.Create(sceneImage, new Rect(0, 0, w, h), new Vector2(w / 2f, h / 2f));
		}
	}
}

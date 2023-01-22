using UnityEngine.Events;

namespace Mapbox.Unity.Utilities.DebugTools
{
	using UnityEngine;
	using UnityEngine.UI;
	using UnityEngine.SceneManagement;

	public class LoadSceneEntry : MonoBehaviour
	{
		[SerializeField]
		private Image _image;

		[SerializeField]
		private Button _button;

		[SerializeField]
		private Text _text;

		private UnityAction _action;

		public Sprite Image
		{
			get => _image ? _image.sprite : null;
			set
			{
				if (_image)
					_image.sprite = value;
				_image.enabled = _image.sprite;
			}
		}

		public UnityAction ClickAction
		{
			get => _action;
			set
			{
				_action = value;
				_button.onClick.RemoveAllListeners();
				_button.onClick.AddListener(_action);
			}
		}

		public string Text
		{
			get => _text ? _text.text : string.Empty;
			set
			{
				if (_text)
					_text.text = value;
			}
		}
	}
}

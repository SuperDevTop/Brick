using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InputFieldEventHandle : MonoBehaviour,ISelectHandler,IDeselectHandler {
	public Sprite normalSpriteIcon;
	public Sprite focusSpriteIcon;
	public Sprite errorSpriteIcon;
	public Sprite normalSpriteField;
	public Sprite errorSpriteField;
	private Image imgIcon;
	private Image imgField;

	void Start() {
		imgIcon = transform.Find ("Icon").GetComponent<Image> ();
		imgField = gameObject.GetComponent<Image> ();
	}

	public void OnSelect (BaseEventData eventData) {	 
		imgIcon.sprite = focusSpriteIcon;
	}

	public void OnDeselect (BaseEventData eventData) {
		imgIcon.sprite = normalSpriteIcon;
		imgField.sprite = normalSpriteField;
	}

	public void OnError() {
		imgField.sprite = errorSpriteField;
		imgIcon.sprite = errorSpriteIcon;
	}
}

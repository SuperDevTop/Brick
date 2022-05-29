using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UILocalization : MonoBehaviour {
	private bool _isShowPopup;
	public GameObject LocalizationPopup;
	public Button btnLocalization;
	public List<Sprite> listSprite;
	
	void Start() {
		_isShowPopup = false;
		LocalizationPopup.SetActive(false);
	}
	
	public void OnShowLocalization(){
		if(!_isShowPopup) {
			LocalizationPopup.SetActive(true);
			_isShowPopup = true;
			btnLocalization.gameObject.GetComponent<Image>().sprite = listSprite[1];
		} else {
			LocalizationPopup.SetActive(false);
			_isShowPopup = false;
			btnLocalization.gameObject.GetComponent<Image>().sprite = listSprite[0];
		}
	}
}

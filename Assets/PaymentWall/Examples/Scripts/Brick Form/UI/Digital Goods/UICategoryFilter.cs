using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UICategoryFilter : MonoBehaviour {
	public GameObject popupCategory;
	public GameObject imgCatAll;
	public GameObject imgCatConsume;
	public GameObject imgCatUpgrade;
	public Text txtCategory;
	public Image btnPopupImage;
	public Sprite arrowUp;
	public Sprite arrowDown;
	private bool _isShow;

	public ProductsArea digitalArea;

	void Start(){
		_isShow = false;
		popupCategory.SetActive(false);
		imgCatAll.SetActive(true);
	}
	public void OpenCategoryFilterPopup() {
		if(_isShow){
			popupCategory.SetActive(false);
			_isShow = false;
			btnPopupImage.sprite = arrowDown;
		} else {
			popupCategory.SetActive(true);
			_isShow = true;
			btnPopupImage.sprite = arrowUp;
		}
	}

	public void OnSwitchCategory(Button pSender){
		CATEGORY_TYPE catType;
		switch(pSender.name){
		case "Button All":
			catType = CATEGORY_TYPE.ALL;
			break;
		case "Button Consume":
			catType = CATEGORY_TYPE.CONSUMABLE;
			break;
		case "Button Upgrade":
			catType = CATEGORY_TYPE.UPGRADES;
			break;
		default:
			catType = CATEGORY_TYPE.ALL;
			break;
		}
		txtCategory.text = pSender.transform.Find("Text").GetComponent<Text>().text;
		SetActiveIndicator(pSender.gameObject);
		digitalArea.SwitchCategory(catType);
		digitalArea.ReloadArea();
		OpenCategoryFilterPopup();
	}

	private void SetActiveIndicator(GameObject btnObject){
		imgCatAll.SetActive(false);
		imgCatConsume.SetActive(false);
		imgCatUpgrade.SetActive(false);
		btnObject.transform.Find("ImgActive").gameObject.SetActive(true);
	}
}

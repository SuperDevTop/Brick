using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UITypeFilter : MonoBehaviour {
	public GameObject popupType;
	public Text txtItemType;
	public GameObject imgTypeAll;
	public GameObject imgTypeHealth;
	public GameObject imgTypeMana;
	public GameObject imgTypeSorcery;
	public GameObject imgTypeWeapons;
	public GameObject imgTypeArmor;
	public GameObject imgTypeAccessories;
	public Image btnPopupImage;
	public Sprite arrowUp;
	public Sprite arrowDown;
	private bool _isShow;

	public ProductsArea digitalArea;

	void Start(){
		_isShow = false;
		popupType.SetActive(false);
		imgTypeAll.SetActive(true);
	}
	public void OpenTypeFilterPopup() {
		if(_isShow){
			popupType.SetActive(false);
			_isShow = false;
			btnPopupImage.sprite = arrowDown;
		} else {
			popupType.SetActive(true);
			_isShow = true;
			btnPopupImage.sprite = arrowUp;
		}
	}

	public void OnSwitchType(Button pSender){
		ITEM_TYPE itemType;
		switch(pSender.name){
		case "Button All":
			itemType = ITEM_TYPE.ALL;
			break;
		case "Button Health":
			itemType = ITEM_TYPE.HEALTH;
			break;
		case "Button Mana":
			itemType = ITEM_TYPE.MANA;
			break;
		case "Button Sorcery":
			itemType = ITEM_TYPE.SORCERY;
			break;
		case "Button Weapons":
			itemType = ITEM_TYPE.WEAPONS;
			break;
		case "Button Armor":
			itemType = ITEM_TYPE.ARMOR;
			break;
		case "Button Accessories":
			itemType = ITEM_TYPE.ACCESSORIES;
			break;
		default:
			itemType = ITEM_TYPE.ALL;
			break;
		}
		txtItemType.text = pSender.transform.Find("Text").GetComponent<Text>().text;
		SetActiveIndicator(pSender.gameObject);
		digitalArea.SwitchType(itemType);
		digitalArea.ReloadArea();
		OpenTypeFilterPopup();
	}
	
	private void SetActiveIndicator(GameObject btnObject){
		imgTypeAll.SetActive(false);
		imgTypeHealth.SetActive(false);
		imgTypeMana.SetActive(false);
		imgTypeSorcery.SetActive(false);
		imgTypeWeapons.SetActive(false);
		imgTypeArmor.SetActive(false);
		imgTypeAccessories.SetActive(false);
		btnObject.transform.Find("ImgActive").gameObject.SetActive(true);
	}
}

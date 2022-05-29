using UnityEngine;
using UnityEngine.UI;

public class UIProductFilter : MonoBehaviour {
	public GameObject tabDigitalGood;
	public GameObject tabVirtual;
	public GameObject tabSubscription;
	public GameObject popupFilter;
	public Image btnPopupImage;
	public Sprite arrowUp;
	public Sprite arrowDown;
	public Text txtFilter;
	public enum TAB_OPTIONS {
		DigitalGoods,
		VirtualCurrency,
		Subscription};
	public TAB_OPTIONS tabOptions;
	private bool _isShow;
	
	void Start(){
		_isShow = false;
		popupFilter.SetActive(false);
	}
	public void OpenPriceFilterPopup() {
		if(_isShow){
			popupFilter.SetActive(false);
			_isShow = false;
			btnPopupImage.sprite = arrowDown;
		} else {
			popupFilter.SetActive(true);
			_isShow = true;
			btnPopupImage.sprite = arrowUp;
		}
	}
	
	public void OnSortProduct(Button pSender){
		if(tabDigitalGood.activeSelf){
			tabDigitalGood.GetComponent<ProductsArea>().SortProduct(pSender);
		}
		if(tabVirtual.activeSelf){
			tabVirtual.GetComponent<VirtualCurrency>().SortProduct(pSender);
		}
		if(tabSubscription.activeSelf){
			tabSubscription.GetComponent<SubscriptionArea>().SortProduct(pSender);
		}
		txtFilter.text = pSender.transform.Find("Text").GetComponent<Text>().text;
		OpenPriceFilterPopup();
	}
}
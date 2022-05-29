using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ShoppingCart : MonoBehaviour {
	public GameObject checkoutPopup;
	public GameObject cartPanelUI;
	public GameObject btnCheckout;
	public GameObject btnFilter;

	public Text inputCoupon;
	public Text txtNoItem;
	public Text txtTotalMoney;
	public Text txtErrCoupon;

	public GameObject previousTab;

	void Update() {
		txtTotalMoney.text = CartManager.Instance.totalMoney.ToString("C2") + " USD";
	}

	void OnEnable() {
		if(txtErrCoupon.gameObject.activeSelf) {
			txtErrCoupon.gameObject.SetActive(false);
		}
		btnFilter.SetActive(false);

		int cartCount = CartManager.Instance.GetCartCount();
		txtNoItem.gameObject.SetActive(false);

		for (int i = 0; i < cartPanelUI.transform.childCount; i++) {
			Destroy (cartPanelUI.transform.GetChild(i).gameObject);
		}
	
		if(cartCount == 0) {
			txtNoItem.gameObject.SetActive(true);
			txtTotalMoney.text = "$0 USD";
			btnCheckout.GetComponent<Button>().interactable = false;
		} else {
			btnCheckout.GetComponent<Button>().interactable = true;

			Dictionary<int,VirtualCurrencyModel> vcDict = CartManager.Instance.GetVirtualCart();
			Dictionary<int,SubscriptionModel> subDict = CartManager.Instance.GetSubscriptionCart();

			foreach (KeyValuePair<int,VirtualCurrencyModel> pair in vcDict) {
				GameObject cNode = Instantiate(Resources.Load ("Prefabs/Items/CartItem")) as GameObject;
				Transform transCart = cNode.transform;
				Transform tParent = transform.Find("Cart List/Cart Panel");
				transCart.SetParent(tParent);
				transCart.localScale = Vector3.one;
				transCart.Find("Quantity/InputField").GetComponent<InputField>().text = pair.Value.quantity.ToString();
				transCart.Find("TextPrice").GetComponent<Text>().text = pair.Value.price.ToString("C2") + " USD";		
				transCart.Find("TextName").GetComponent<Text>().text = pair.Value.name;
				transCart.Find("TextTotalPrice").GetComponent<Text>().text = (pair.Value.quantity * pair.Value.price).ToString("C2") + " USD";//(getFinalPrice(pair.Value) * pair.Value.Quantity).ToString("C2") + " USD";
				int id = pair.Key;
				
				Button remB = transCart.Find("Button Remove").GetComponent<Button>();
				Button minB = transCart.Find("Quantity/Button Minus").GetComponent<Button>();
				Button addB = transCart.Find("Quantity/Button Plus").GetComponent<Button>();
				remB.onClick.AddListener(() => RemoveThisCartItem(id,transCart));
				minB.onClick.AddListener(() => MinusItem(id,transCart));
				addB.onClick.AddListener(() => PlusItem(id,transCart));
			}

			foreach (KeyValuePair<int,SubscriptionModel> pair in subDict) {
				GameObject cNode = Instantiate(Resources.Load ("Prefabs/Items/CartItem")) as GameObject;
				Transform transCart = cNode.transform;
				Transform tParent = transform.Find("Cart List/Cart Panel");
				transCart.SetParent(tParent);
				transCart.localScale = Vector3.one;
				transCart.Find("Quantity/InputField").GetComponent<InputField>().text = pair.Value.quantity.ToString();
				transCart.Find("TextPrice").GetComponent<Text>().text = pair.Value.price.ToString("C2") + " USD";
				transCart.Find("TextName").GetComponent<Text>().text = pair.Value.name;
				transCart.Find("TextTotalPrice").GetComponent<Text>().text = (pair.Value.quantity * pair.Value.price).ToString("C2") + " USD";//(getFinalPrice(pair.Value) * pair.Value.Quantity).ToString("C2") + " USD";

//				if(pair.Value.isSaleOff) {
//					transCart.FindChild("TextName/TextSale").gameObject.SetActive(true);
//					transCart.FindChild("TextName/TextSale").GetComponent<Text>().text = "SALE! " + pair.Value.PercentSale +"% Off";
//					transCart.FindChild("TextName").GetComponent<RectTransform>().localPosition = new Vector3(-270,-15,0);
//					transCart.FindChild("TextPrice/TextSaleValue").gameObject.SetActive(true);
//					transCart.FindChild("TextPrice/TextSaleValue").GetComponent<Text>().text = pair.Value.Price.ToString("C2") + " USD";
//					transCart.FindChild("TextPrice").GetComponent<Text>().text = getFinalPrice(pair.Value).ToString("C2") + " USD";
//				} else {
//					transCart.FindChild("TextPrice").GetComponent<Text>().text = pair.Value.Price.ToString("C2") + " USD";
//				}

//				if(pair.Value.isSubScription){
//					transCart.FindChild("TextPrice/ChargeDesc").gameObject.SetActive(true);
//					transCart.FindChild("TextPrice/ChargeDesc").GetComponent<Text>().text = pair.Value.subDescription;
//					transCart.FindChild("TextPrice").GetComponent<RectTransform>().localPosition = new Vector3(100.0f,5.0f,0);
//				}
				int id = pair.Key;

				Button remB = transCart.Find("Button Remove").GetComponent<Button>();
				Button minB = transCart.Find("Quantity/Button Minus").GetComponent<Button>();
				Button addB = transCart.Find("Quantity/Button Plus").GetComponent<Button>();
				remB.onClick.AddListener(() => RemoveThisCartItem(id,transCart));
				minB.onClick.AddListener(() => MinusItem(id,transCart));
				addB.onClick.AddListener(() => PlusItem(id,transCart));
			}
		}
	}

	public void ApplyCoupon() {
		txtErrCoupon.gameObject.SetActive(true);
		if(inputCoupon.text == "COUPON"){
			txtErrCoupon.text = "Correct coupon. Thank you!";
			txtErrCoupon.color = new Color(0,1.0f,0);
		} else {
			txtErrCoupon.text = "Please enter a valid coupon code.";
			txtErrCoupon.color = new Color(1.0f,0,0);
		}
	}

	
	public void OnBackToStore() {
		btnFilter.SetActive(true);
		gameObject.SetActive(false);
		previousTab.SetActive(true);
	}

	public void OnCheckOut() {
		checkoutPopup.SetActive(true);
	}

//	private double getFinalPrice(ProductInfo pInfo) {
//		double salePrice;
//
//		if(pInfo.isSaleOff) {
//			salePrice = pInfo.Price/100 * pInfo.PercentSale;
//			return pInfo.Price - salePrice;
//		} else {
//			return pInfo.Price;
//		}
//	}
//
	void RemoveThisCartItem(int id,Transform tParent) {
		CartManager.Instance.RemoveProduct(id);
		Destroy(tParent.gameObject);

		if(CartManager.Instance.GetCartCount() == 0) {
			btnCheckout.GetComponent<Button>().interactable = false;
		}

	}

	void MinusItem(int id,Transform tCart) {
		CartManager.Instance.MinusProduct(id);
		tCart.Find("TextTotalPrice").GetComponent<Text>().text = CartManager.Instance.GetTotalPriceOfAProduct(id).ToString("C2") + " USD";//(getFinalPrice(np) * np.Quantity).ToString("C2") + " USD";
		tCart.Find("Quantity/InputField").GetComponent<InputField>().text = CartManager.Instance.GetQuantityOfAProduct(id).ToString();
	}

	void PlusItem(int id,Transform tCart) {
		CartManager.Instance.PlusProduct(id);
		tCart.Find("TextTotalPrice").GetComponent<Text>().text = CartManager.Instance.GetTotalPriceOfAProduct(id).ToString("C2") + " USD";//(getFinalPrice(p) * p.Quantity).ToString("C2") + " USD";
		tCart.Find("Quantity/InputField").GetComponent<InputField>().text = CartManager.Instance.GetQuantityOfAProduct(id).ToString();
	}
}
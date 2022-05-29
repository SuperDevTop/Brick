using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SuccessUI : MonoBehaviour {
	public UINavigation navHandler;
	public GameObject searchButton;
	public GameObject btnCart;
	public Text txtReceipt;
	public Text txtOrderTotal;

	void OnEnable() {
		searchButton.SetActive(false);
		btnCart.SetActive(false);
		navHandler.gameObject.SetActive (false);
	}

	public void OnGoBackToShopping(Button pSender) {
		gameObject.SetActive (false);
		navHandler.gameObject.SetActive (true);
		searchButton.SetActive(true);
		btnCart.SetActive(true);
		navHandler.SwitchTab (pSender);
	}

	public void UpdateTransactionDetails(string receipt, string total) {
		txtReceipt.text = receipt;
		txtOrderTotal.text = "$" + total + ".00 USD";
	}
}

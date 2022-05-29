using UnityEngine;
using UnityEngine.UI;

public class UITextTitle : MonoBehaviour {
	public UINavigation navHandler;
	public GameObject ShoppingCartUI;
	public GameObject CheckoutUI;
	public GameObject SuccessUI;
	public GameObject FailUI;

	public void BackToHome(Button pSender) {
		ShoppingCartUI.SetActive(false);
		CheckoutUI.SetActive(false);
		SuccessUI.SetActive(false);
		FailUI.SetActive(false);
		gameObject.GetComponent<Button>().interactable = true;
		navHandler.SwitchTab(pSender);
	}
}

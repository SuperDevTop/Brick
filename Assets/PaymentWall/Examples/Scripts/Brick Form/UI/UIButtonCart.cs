using UnityEngine;
using UnityEngine.UI;

public class UIButtonCart : MonoBehaviour {
	public GameObject TabDigitalUI;
	public GameObject TabVirtualUI;
	public GameObject TabSubscriptionUI;
	public GameObject ShoppingCartUI;
	public Button BtnCart;
	
	void Start(){
		BtnCart.interactable = false;
	}
	public void GoToShoppingCart(){
		if(TabDigitalUI.activeSelf){
			TabDigitalUI.SetActive(false);
			ShoppingCartUI.GetComponent<ShoppingCart>().previousTab = TabDigitalUI;
		}
		if(TabVirtualUI.activeSelf){
			TabVirtualUI.SetActive(false);
			ShoppingCartUI.GetComponent<ShoppingCart>().previousTab = TabVirtualUI;
		}
		if(TabSubscriptionUI.activeSelf){
			TabSubscriptionUI.SetActive(false);
			ShoppingCartUI.GetComponent<ShoppingCart>().previousTab = TabSubscriptionUI;
		}
		ShoppingCartUI.SetActive(true);
	}
}

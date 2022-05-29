using UnityEngine;
using System.Collections;

public class CheckoutForm : MonoBehaviour {
	public GameObject shoppingCartUI;
	public BrickFormHandle brickForm;

	void OnEnable(){
		brickForm.ResetForm();
		double totalCashOut = CartManager.Instance.totalMoney;
		brickForm.UpdateForm("Paymentwall"," ",totalCashOut,"USD");
		brickForm.UpdateAPIkey("t_b33d46eda162a2537ee9849040b02b","t_c99c2918ee0538144fd81d7dbc40ec");
	}

	public void OnCloseCheckoutForm(){
		gameObject.SetActive(false);
		shoppingCartUI.SetActive(true);
	}
}

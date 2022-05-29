using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SimpleJSON;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Paymentwall;
using UnityEngine.EventSystems;	

public class BrickFormHandle : MonoBehaviour
{
	public GameObject shoppingCartUI;
	public GameObject successUI;
	public GameObject failUI;

	public Text inputCardNumber;
	public Text inputExpDate;
	public Text inputCVV;
	public Text inputEmail;

	public GameObject buttonPay;
	public GameObject errTextCard;
	public GameObject errTextEmail;

	private bool flagCreditCard;
	private bool flagExpDate;
	private bool flagCVV;
	private bool flagEmail;
	
	private double amount;
	private string currency;
	private bool isStoreCreditCard;

	public void UpdateForm (string merchant, string product, double amount, string currency) {
		this.amount = amount;
		this.currency = currency;
		transform.Find ("Brick Merchant Name").GetComponent<Text> ().text = merchant;
		transform.Find ("Product Name").GetComponent<Text> ().text = product;
		transform.Find ("Button Pay/Text").GetComponent<Text> ().text = "Pay " + amount.ToString ("C2") + " " + currency;
	}

	public void UpdateAPIkey (string publicKey, string secretKey) {
		PWBase.SetAppKey(publicKey);
		PWBase.SetSecretKey(secretKey);
	}

	public void OnRequestCharge(){
		StartCoroutine(OnPay());
//		OnPay();
	}

	public IEnumerator OnPay () {
		flagCreditCard = true;
		flagExpDate = true;
		flagCVV = true;
		flagEmail = true;
		errTextCard.SetActive (false);
		errTextEmail.SetActive (false);

		if(!PWUltils.IsCVVNumberValid(inputCVV.text)){
			inputCVV.transform.parent.GetComponent<InputFieldEventHandle> ().OnError ();
			flagCVV = false;
			errTextCard.GetComponent<Text>().text = "Please check CVV2 (CVC) code.";
			errTextCard.SetActive(true);
		}
		if(!PWUltils.IsExpireDateValid(inputExpDate.text)){
			inputExpDate.transform.parent.GetComponent<InputFieldEventHandle>().OnError();
			flagExpDate = false;
			errTextCard.GetComponent<Text>().text = "Please check card expiry date.";
			errTextCard.SetActive(true);
		}
		if(!PWUltils.IsCreditCardNumberValid(inputCardNumber.text)){
			inputCardNumber.transform.parent.GetComponent<InputFieldEventHandle> ().OnError ();
			flagCreditCard = false;
			errTextCard.GetComponent<Text>().text = "Please check your credit card number.";
			errTextCard.SetActive(true);
		}

		if(!PWUltils.IsEmailValid(inputEmail.text)){
			inputEmail.transform.parent.GetComponent<InputFieldEventHandle> ().OnError ();
			flagEmail = false;
			errTextEmail.SetActive (true);
		}

		if (!flagCreditCard || !flagExpDate || !flagCVV || !flagEmail) {
			yield return false;
		}

		string cardNumber = inputCardNumber.text;
		string cvv = inputCVV.text;

		string expMonth = inputExpDate.text.Substring (0, 2);
		string expYear = inputExpDate.text.Substring (3);

		if(FakeAccountController.Instance.IsAccountOnline()){
			PWOneTimeToken token = new PWOneTimeToken();
			token.Create(PWBase.GetAppKey(),cardNumber,expMonth,expYear,cvv);
			PWCharge charge = gameObject.AddComponent<PWCharge>();
			yield return StartCoroutine(charge.Create(token,inputEmail.text,this.currency,this.amount.ToString(),"fingerprint here","description here"));
			if(charge.IsSuccessful()) {
				TransactionSuccess(charge.GetReceipt(),charge.GetChargeAmount().ToString());
			} else {
				TransactionFail(charge.GetErrorMessage());
			}
		} else {
			TransactionFail("You must login before progess any purchase");
		}
	}

	public void OnInputExpDateGetClick() {
		Transform parentTf = inputExpDate.transform.parent;
		InputField parentTxt = parentTf.GetComponent<InputField> ();
		parentTxt.text = "";
	}

	public void OnInputingExpDate(string value) {
		Transform parentTf = inputExpDate.transform.parent;
		InputField parentTxt = parentTf.GetComponent<InputField> ();
		parentTxt.characterLimit = 4;
	}
	
	public void OnInputExpDateValueFinish (string value)
	{
		Transform parentTf = inputExpDate.transform.parent;
		InputField parentTxt = parentTf.GetComponent<InputField> ();
		parentTxt.characterLimit = 5;
		if (value.Length == 4) {
			parentTxt.text = value.Substring (0, 2) + "/" + value.Substring (2);
		} else if (value.Length == 3) {
			int valMonth = int.Parse (value [0].ToString ());
			if (valMonth == 1 || valMonth == 0)
				parentTxt.text = value.Substring (0, 2) + "/0" + value.Substring (2);
			else
				parentTxt.text = "0" + value.Substring (0, 1) + "/" + value.Substring (1);
		} else
			return;
	}

	public void TransactionSuccess(string receipt,string amount){
		shoppingCartUI.SetActive(false);
		transform.parent.gameObject.SetActive(false);
		CartManager.Instance.ProgessPurchase();
		successUI.GetComponent<SuccessUI>().UpdateTransactionDetails(receipt,amount);
		successUI.SetActive(true);
	}
	
	public void TransactionFail(string errMessage){
		shoppingCartUI.SetActive(false);
		transform.parent.gameObject.SetActive(false);
		failUI.SetActive(true);
		failUI.GetComponent<FailedUI>().SetErrorMessage(errMessage);
	}

	public void ResetForm(){
		buttonPay.transform.Find("Text").gameObject.SetActive(true);
		buttonPay.GetComponent<Button>().interactable = true;
	}
}

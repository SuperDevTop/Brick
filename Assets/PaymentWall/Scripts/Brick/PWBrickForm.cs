using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SimpleJSON;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Paymentwall
{
	public class PWBrickForm : MonoBehaviour {
		public Text txtCardNumber;
		public Text txtExpDate;
		public Text txtCVV;
		public Text txtEmail;
		public GameObject buttonPay;
		public GameObject helpCVCPopup;
		public GameObject helpSCCPopup;
		public GameObject txtErrorCard;
		public GameObject txtErrorEmail;
		private bool flagCreditCard;
		private bool flagExpDate;
		private bool flagCVV;
		private bool flagEmail;
		private string publicKey;
		private string privateKey;
		private double amount;
		private string currency;
		private bool isStoreCreditCard;

		public void UpdateFormInformation (string merchant, string product, double amount, string currency) {
			this.amount = amount;
			this.currency = currency;
			transform.Find ("Container/Panel/Brick Merchant Name").GetComponent<Text> ().text = merchant;
			transform.Find ("Container/Panel/Product Name").GetComponent<Text> ().text = product;
			transform.Find ("Container/Panel/Button Pay/Text").GetComponent<Text> ().text = "Pay " + amount.ToString ("C2") + " " + currency;
		}

		public void UpdateAPIkey (string publicKey, string secretKey) {
			this.publicKey = publicKey;
			this.privateKey = secretKey;
		}

		public void OnPay () {
			flagCreditCard = true;
			flagExpDate = true;
			flagCVV = true;
			flagEmail = true;
			txtErrorCard.SetActive (false);
			txtErrorEmail.SetActive (false);

			if (!PWUltils.IsCVVNumberValid (txtCVV.text)) {
				txtCVV.transform.parent.GetComponent<InputFieldEventHandle> ().OnError ();
				flagCVV = false;
				txtErrorCard.GetComponent<Text>().text = "Please check CVV2 (CVC) code.";
				txtErrorCard.SetActive (true);
			}
			if (!PWUltils.IsExpireDateValid (txtExpDate.text)) {
				txtExpDate.transform.parent.GetComponent<InputFieldEventHandle> ().OnError ();
				flagExpDate = false;
				txtErrorCard.GetComponent<Text>().text = "Please check card expiry date.";
				txtErrorCard.SetActive (true);
			}
			if (!PWUltils.IsCreditCardNumberValid (txtCardNumber.text)) {
				txtCardNumber.transform.parent.GetComponent<InputFieldEventHandle> ().OnError ();
				flagCreditCard = false;
				txtErrorCard.GetComponent<Text>().text = "Please check your credit card number.";
				txtErrorCard.SetActive (true);
			}

			if (!PWUltils.IsEmailValid (txtEmail.text)) {
				txtEmail.transform.parent.GetComponent<InputFieldEventHandle> ().OnError ();
				flagEmail = false;
				txtErrorEmail.SetActive (true);
			}

			if (!flagCreditCard || !flagExpDate || !flagCVV || !flagEmail) {
				return;
			}

			string cardNumber = txtCardNumber.text;
			string cvv = txtCVV.text;

			string expMonth = txtExpDate.text.Substring (0, 2);
			string expYear = txtExpDate.text.Substring (3);

			RequestOneTimeToken (ResponseOneTimeToken, cardNumber, expMonth, expYear, cvv);
		}

		public void OnInputExpDateGetClick() {
			Transform parentTf = txtExpDate.transform.parent;
			InputField parentTxt = parentTf.GetComponent<InputField> ();
			parentTxt.text = "";
		}
		
		public void OnInputingExpDate(string value) {
			Transform parentTf = txtExpDate.transform.parent;
			InputField parentTxt = parentTf.GetComponent<InputField> ();
			parentTxt.characterLimit = 4;
		}

		public void OnInputExpDateValueFinish (string value) {
			Transform parentTf = txtExpDate.transform.parent;
			InputField parentTxt = parentTf.GetComponent<InputField> ();
			if (value.Length == 4) {
				parentTxt.text = value.Substring (0, 2) + "/" + value.Substring (2);
			} else if (value.Length == 3) {
				int valMonth = int.Parse (value [0].ToString ());
				if (valMonth == 1 || valMonth == 0) {
					parentTxt.text = value.Substring (0, 2) + "/0" + value.Substring (2);
				} else {
					parentTxt.text = "0" + value.Substring (0, 1) + "/" + value.Substring (1);
				}
			} else {
				return;
			}
		}

		public delegate IEnumerator OnRequest (WWW www);

		private void RequestOneTimeToken (OnRequest request, string cardNumber, string expMonth, string expYear, string cvv) {
			WWWForm form = new WWWForm ();
			form.AddField ("public_key", this.publicKey);
			form.AddField ("card[number]", cardNumber);
			form.AddField ("card[exp_month]", expMonth);
			form.AddField ("card[exp_year]", "20" + expYear);
			form.AddField ("card[cvv]", cvv);
			WWW www = new WWW (PWBase.TOKEN_URL, form);
			StartCoroutine (request (www));
		}

		private IEnumerator ResponseOneTimeToken (WWW www) {
			yield return www;

			if (www.error == null) {
				string json = www.text;
				JSONNode response = JSON.Parse (json);
				if (response != null) {
					if (response ["type"] != "Error") {
						string token = response ["token"];
						buttonPay.transform.Find ("Text").GetComponent<Text> ().text = ".....";
						RequestCharge (ResponseCharge, token);
                        AfterPayment.Instance.isPaid = true;
					} else {
						txtErrorEmail.SetActive(true);
						txtErrorEmail.GetComponent<Text>().text = response ["error"];
					}
				} else {
					txtErrorEmail.SetActive(true);
					txtErrorEmail.GetComponent<Text>().text = www.error;
				}
			} else {
				txtErrorEmail.SetActive(true);
				txtErrorEmail.GetComponent<Text>().text = www.error;
			}
		}

		private void RequestCharge (OnRequest request, string token) {
			WWWForm form = new WWWForm ();
			var headers = form.headers;
			headers ["X-ApiKey"] = this.privateKey;
			form.AddField ("token", token, Encoding.UTF8);
			form.AddField ("amount", this.amount.ToString ());
			form.AddField ("currency", this.currency);
			form.AddField ("email", txtEmail.text);
			form.AddField ("fingerprint", "your fingerprint here");
			form.AddField ("description", "your description here");

			WWW www = new WWW (PWBase.GetChargeURL (), form.data, headers);
			StartCoroutine (request (www));
		}

		private IEnumerator ResponseCharge (WWW www) {
			yield return www;
		
			if (www.error == null) {
				string json = www.text;
				JSONNode response = JSON.Parse (json);
				if (response != null) {
					string objMessage = response ["object"];
					string diagMess;
					if (objMessage.Equals ("charge")) {
						Debug.Log ("Thank you for payment");
						buttonPay.transform.Find ("Text").gameObject.SetActive (false);
						buttonPay.GetComponent<Button> ().interactable = false;
						if (isStoreCreditCard) {
							Debug.Log ("Store your credit card here with token : " + response["card"]["token"]);
						}
					} else {
						diagMess = "There's a problem with your payment";
						buttonPay.transform.Find ("Text").GetComponent<Text> ().text = diagMess;
					}
				} else {
					txtErrorEmail.SetActive(true);
					txtErrorEmail.GetComponent<Text>().text = www.error;
				}
			} else {
				txtErrorEmail.SetActive(true);
				txtErrorEmail.GetComponent<Text>().text = www.error;
			}
		}

		public void ToggleHelpCVC (bool isOn) {
			helpCVCPopup.SetActive (isOn);
		}

		public void ToggleStoreCC (bool isOn) {
			isStoreCreditCard = isOn;
		}

		public void ToggleHelpSCC (bool isOn) {
			helpSCCPopup.SetActive (isOn);
		}

		public void ResetForm () {
			buttonPay.transform.Find ("Text").gameObject.SetActive (true);
			buttonPay.GetComponent<Button> ().interactable = true;
		}

		public void OnCloseForm() {
			Destroy (gameObject);
		}
	}
}

using UnityEngine;
using System.Collections;
using SimpleJSON;
using Paymentwall;
using System.Collections.Generic;
using System;

public class PWCharge : MonoBehaviour {
	private bool isTransactionSuccessful;
	private string email;
	private string currency;
	private string amount;
	private string fingerprint;
	private string description;
	private string cardToken;
	private string errorMessage;
	private string receipt;

	public bool isRequestFinished;

	public IEnumerator Create(PWOneTimeToken tokenModel,string email,string currency ,string amount ,string fingerprint,string description ) {
		this.email = email;
		this.currency = currency;
		this.amount = amount;
		this.fingerprint = fingerprint;
		this.description = description;

		string cardNumber = tokenModel.GetCardNumber ();
		string expMonth = tokenModel.GetExpMonth ();
		string expYear = tokenModel.getExpYear ();
		string cvv = tokenModel.getCVV ();
		isRequestFinished = false;
		yield return StartCoroutine(RequestOneTimeToken(tokenModel.GetPublicKey() ,cardNumber, expMonth, expYear, cvv));
	}

	private void SetChargeStatus(bool status) {
		this.isTransactionSuccessful = status;
	}

	public bool IsSuccessful() {
		return this.isTransactionSuccessful;
	}

	private IEnumerator RequestOneTimeToken (string publicKey,string cardNumber, string expMonth, string expYear, string cvv) {
		WWWForm form = new WWWForm ();
		form.AddField ("public_key", publicKey);
		form.AddField ("card[number]", cardNumber);
		form.AddField ("card[exp_month]", expMonth);
		form.AddField ("card[exp_year]", "20" + expYear);
		form.AddField ("card[cvv]", cvv);
		WWW www = new WWW (PWBase.TOKEN_URL, form);
		yield return www;
		
		if (www.error == null) {
			string json = www.text;
			JSONNode response = JSON.Parse (json);
			if (response != null) {
				if (response ["type"] != "Error") {
					string token = response["token"];
					yield return StartCoroutine(RequestCharge(token, this.email, this.currency, this.amount, this.fingerprint, this.description));
				} else {
					this.errorMessage = response ["error"];
				}
			} else {
				this.errorMessage = "Can't reach payment server";
			}
		} else {
			Debug.Log (www.error);
			this.errorMessage = www.error;
		}
	}

	private IEnumerator RequestCharge (string token,string email,string currency,string amount,string fingerprint,string description) {
		WWWForm form = new WWWForm ();
		Dictionary<string,string> headers = form.headers;
		headers ["X-ApiKey"] = PWBase.GetSecretKey();
		form.AddField ("token", token);
		form.AddField ("amount", amount.ToString ());
		form.AddField ("currency", currency);
		form.AddField ("email", email);
		form.AddField ("fingerprint", fingerprint);
		form.AddField ("description", description);
		WWW www = new WWW (PWBase.GetChargeURL(), form.data, headers);
		yield return www;
		
		if (www.error == null) {
			string json = www.text;
			JSONNode response = JSON.Parse (json);
			if (response != null) {
				string objMessage = response ["object"];
				if (objMessage.Equals ("charge")) {
					Debug.Log (json);
					SetChargeStatus(true);
					this.cardToken = response["card"]["token"];
					this.receipt = response["id"];
				} else {
					SetChargeStatus(false);
					this.errorMessage = response ["error"];
				}
			} else {
				this.errorMessage = "Can't reach payment server";
			}
		} else {
			Debug.Log (www.error);
			this.errorMessage = www.error;
		}

		isRequestFinished = true;
	}

	public string GetCardToken() {
		if (this.isTransactionSuccessful) {
			return this.cardToken;
		} else {
			return null;
		}
	}

	public double GetChargeAmount() {
		if(this.isTransactionSuccessful) {
			return double.Parse(this.amount);
		} else {
			return 0;
		}
	}

	public string GetReceipt() {
		if(this.isTransactionSuccessful) {
			return this.receipt;
		} else {
			return null;
		}
	}

	public string GetErrorMessage() {
		return this.errorMessage;
	}
}
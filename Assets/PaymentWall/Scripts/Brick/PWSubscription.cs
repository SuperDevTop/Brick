using UnityEngine;
using System.Collections;
using SimpleJSON;
using Paymentwall;

public class PWSubscription : MonoBehaviour {
	private bool isSuccessful;
	private string subId;
	private string email;
	private string currency;
	private string amount;
	private string fingerprint;
	private string description;
	private string cardToken;
	private string plan;
	private string period;
	private string duration;
	
	public void Create(PWOneTimeToken tokenModel,string email,string currency ,string amount ,string fingerprint,string description,string plan,string period,string duration) {
		this.email = email;
		this.currency = currency;
		this.amount = amount;
		this.fingerprint = fingerprint;
		this.description = description;
		this.plan = plan;
		this.period = period;
		this.duration = duration;
		
		string cardNumber = tokenModel.GetCardNumber ();
		string expMonth = tokenModel.GetExpMonth ();
		string expYear = tokenModel.getExpYear ();
		string cvv = tokenModel.getCVV ();
		StartCoroutine(RequestOneTimeToken(tokenModel.GetPublicKey() ,cardNumber, expMonth, expYear, cvv));
	}

	public void Cancel(string subscriptionId) {
		StartCoroutine(CancelSubscription(subscriptionId));
	}
	
	private void SetSubStatus(bool status) {
		this.isSuccessful = status;
	}
	
	public bool IsSuccessful() {
		return this.isSuccessful;
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
					StartCoroutine(RequestSubscription(token, this.email, this.currency, 
					                             this.amount, this.fingerprint, this.description,
					                             this.plan,this.period,this.duration));
				} else {
					Debug.Log (response ["error"]);
				}
			} else {
				Debug.Log ("Can't connect to Paymentwall server");
			}
		} else {
			Debug.Log (www.error);
		}
	}
	
	private IEnumerator RequestSubscription (string token,string email,string currency,string amount,
	                                   string fingerprint,string description,string plan,
	                                   string period,string duration)
	{
		WWWForm form = new WWWForm ();
		var headers = form.headers;
		headers ["X-ApiKey"] = PWBase.GetSecretKey();
		form.AddField ("token", token);
		form.AddField ("amount", amount.ToString ());
		form.AddField ("currency", currency);
		form.AddField ("email", email);
		form.AddField ("fingerprint", fingerprint);
		form.AddField ("description", description);
		form.AddField ("plan", plan);
		form.AddField ("period", period);
		form.AddField ("period_duration", duration);

		WWW www = new WWW (PWBase.GetSubscriptionURL(), form.data, headers);
		yield return www;
		
		if (www.error == null) {
			string json = www.text;
			JSONNode response = JSON.Parse (json);
			if (response != null) {
				string objMessage = response ["object"];
				if (objMessage.Equals ("subscription")) {
					SetSubStatus(true);
					this.cardToken = response["card"]["token"];
					this.subId = response["id"];

				} else {
					Debug.Log (json);
					SetSubStatus(false);
				}
			} else {
				Debug.Log ("Can't react Paymentwall server");
			}
		} else {
			Debug.Log (www.error);
		}
	}

	private IEnumerator CancelSubscription (string subscriptionId)
	{	
		WWW www = new WWW (PWBase.GetCancelSubscriptionURL() + subscriptionId);
		yield return www;
		
		if (www.error == null) {
			string json = www.text;
			JSONNode response = JSON.Parse (json);
			if (response != null) {
				string objMessage = response ["object"];
				if (objMessage.Equals ("subscription")) {
					Debug.Log (json);
					SetSubStatus(false);
					this.cardToken = "";
					this.subId = "";
				} else {
					Debug.Log (json);
				}
			} else {
				Debug.Log ("Can't react Paymentwall server");
			}
		} else {
			Debug.Log (www.error);
		}
	}
	
	public string GetSubscriptionId() {
		return this.subId;
	}

	public string GetCardToken() {
		return this.cardToken;
	}
}

using System.Collections;
using UnityEngine;
using SimpleJSON;

public class PWOneTimeToken{
	private string publicKey;
	private string cardNumber;
	private string expMonth;
	private string expYear;
	private string cvv;
	private string token;

	public string Create(string publicKey,string cardNumber,string expMonth,string expYear,string cvv) {
		this.publicKey = publicKey;
		this.cardNumber = cardNumber;
		this.expMonth = expMonth;
		this.expYear = expYear;
		this.cvv = cvv;

		return this.token;
	}
	
	public string GetPublicKey() {
		return this.publicKey;
	}

	public string GetCardNumber() {
		return this.cardNumber;
	}

	public string GetExpMonth() {
		return this.expMonth;
	}

	public string getExpYear() {
		return this.expYear;
	}

	public string getCVV() {
		return this.cvv;
	}

	private void setToken(string token) {
		this.token = token;
	}

	public string getToken() {
		return this.token;
	}
}
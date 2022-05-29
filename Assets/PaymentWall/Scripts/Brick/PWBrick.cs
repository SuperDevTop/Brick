using UnityEngine;
using System.Collections;

namespace Paymentwall{
	public class PWBrick{
		private double amount;
		private string currency;
		private string merchantName;
		private string productName;

		public PWBrick(double amount,string currency,string merchantName,string productName){
			this.amount = amount;
			this.currency = currency;
			this.merchantName = merchantName;
			this.productName = productName;
		}

		public void ShowPaymentForm(){
			GameObject go = GameObject.Instantiate (Resources.Load ("PWPrefabs/Brick")) as GameObject;
			go.GetComponent<PWBrickForm> ().UpdateFormInformation (this.merchantName, this.productName, this.amount, this.currency);
			go.GetComponent<PWBrickForm> ().UpdateAPIkey (PWBase.GetAppKey(), PWBase.GetSecretKey());
		}
	}
}
using UnityEngine;
using System.Collections;
using Paymentwall;
using SimpleJSON;
using System.Collections.Generic;

public class WidgetTab : MonoBehaviour {
	public Canvas canvas;

	// Use this for initialization
	void Start () {
		string json = ((TextAsset)Resources.Load ("Data/widget")).text;
		JSONNode node = JSON.Parse(json);

		int type = int.Parse(node["apiType"]);
		string appKey = node["appKey"]; // available in your Paymentwall merchant area
		string secretKey = node["secretKey"]; // available in your Paymentwall merchant area
		PWBase.SetApiType(type);
		PWBase.SetAppKey(appKey); 
		PWBase.SetSecretKey(secretKey); 
		
		List<PWProduct> productList = new List<PWProduct>();
		PWProduct product = new PWProduct(
			"product301", // id of the product in your system
			9.99f, // price
			"USD", // currency code
			"Gold Membership", // product name
			PWProduct.TYPE_SUBSCRIPTION, // this is a time-based product; for one-time products, use Paymentwall_Product.TYPE_FIXED and omit the following 3 parameters
			1, // time duration
			PWProduct.PERIOD_TYPE_YEAR, // year
			true // recurring
			);
		productList.Add(product);
		PWWidget widget = new PWWidget(
			"user40012", // id of the end-user who's making the payment
			"p1_1", // widget code, e.g. p1; can be picked inside of your merchant account
			productList,
			new Dictionary<string, string>() {{"email", "user@hostname.com"}} // additional parameters
		);
		
		PWUnityWidget unity = new PWUnityWidget (widget);
		StartCoroutine (unity.callWidgetWebView (gameObject,canvas)); // call this function to show Widget
	}
}

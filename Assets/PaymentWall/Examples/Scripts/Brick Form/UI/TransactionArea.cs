using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class TransactionArea : MonoBehaviour {
	public List<TransInfo> transactionList;
	
	void OnEnable(){
		for (int i = 0; i < transform.childCount; i++) {
			Destroy (transform.GetChild(i).gameObject);
		}

		transactionList = new List<TransInfo>();
		Dictionary<int, VirtualCurrencyModel> dictVirtual = CartManager.Instance.GetVirtualCart();
		foreach(KeyValuePair<int,VirtualCurrencyModel> pair in dictVirtual) {
			TransInfo temp = new TransInfo();
			temp.ItemName = pair.Value.name;
			temp.Price = pair.Value.price;
			temp.Quantity = pair.Value.quantity;
			temp.TotalPrice = 0;
			temp.DiscountPercent = 0;
//			if(pair.Value.isSaleOff) {
//				double finalPrice = pair.Value.Price - (pair.Value.Price/100 * pair.Value.PercentSale);
//				temp.TotalPrice = finalPrice * temp.Quantity;
//				temp.DiscountPrice = temp.Price * temp.Quantity;
//				temp.DiscountPercent = pair.Value.PercentSale;
//			} else {
				temp.TotalPrice = temp.Price * temp.Quantity;
//			}
			transactionList.Add(temp);
		}
		Dictionary<int, SubscriptionModel> dictSub = CartManager.Instance.GetSubscriptionCart();
		foreach(KeyValuePair<int,SubscriptionModel> pair in dictSub) {
			TransInfo temp = new TransInfo();
			temp.ItemName = pair.Value.name;
			temp.Price = pair.Value.price;
			temp.Quantity = pair.Value.quantity;
			temp.TotalPrice = 0;
			temp.DiscountPercent = 0;
			//			if(pair.Value.isSaleOff) {
			//				double finalPrice = pair.Value.Price - (pair.Value.Price/100 * pair.Value.PercentSale);
			//				temp.TotalPrice = finalPrice * temp.Quantity;
			//				temp.DiscountPrice = temp.Price * temp.Quantity;
			//				temp.DiscountPercent = pair.Value.PercentSale;
			//			} else {
			temp.TotalPrice = temp.Price * temp.Quantity;
			//			}
			transactionList.Add(temp);
		}

		CartManager.Instance.ClearCart();

		foreach (TransInfo item in transactionList) {
			GameObject cNode = Instantiate(Resources.Load ("Prefabs/Items/TransItem")) as GameObject;
			Transform transCart = cNode.transform;
			transCart.SetParent(transform);
			transCart.localScale = Vector3.one;
			
			transCart.Find("TextName").GetComponent<Text>().text = item.ItemName;
			transCart.Find("TextPrice").GetComponent<Text>().text = item.Price.ToString("C2") + " USD";
			transCart.Find("TextQuantity").GetComponent<Text>().text = item.Quantity.ToString();
			transCart.Find("TextTotalPrice").GetComponent<Text>().text = item.TotalPrice.ToString("C2") + " USD";
			if(item.DiscountPrice != 0) {
				transCart.Find("TextTotalPrice/TextSaleValue").gameObject.SetActive(true);
				transCart.Find("TextTotalPrice/TextSaleValue").GetComponent<Text>().text = item.DiscountPrice.ToString("C2") + " USD";
				transCart.Find("TextName/TextSalePercent").gameObject.SetActive(true);
				transCart.Find("TextName/TextSalePercent").GetComponent<Text>().text = "SALE! " + item.DiscountPercent + "% Off";
			}
		}
	}
}

public class TransInfo{
	public string ItemName;
	public double Price;
	public int Quantity;
	public double TotalPrice;
	public double DiscountPrice;
	public int DiscountPercent;
}

using UnityEngine;
using SimpleJSON;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class SubscriptionArea: MonoBehaviour {
	private const string cartStr = " items ";

	public GameObject tabSubscriptionUI;
	public GameObject detailsUI;
	public GameObject failUI;

	// Use this for initialization
	void Start () {
		ReloadArea();
	}

	private void ReloadArea() {
		RemoveChildProduct();
		List<SubscriptionModel> listSub = SubscriptionController.Instance.GetListSubscriptionModels();
		for (int i = 0; i < listSub.Count; i++) {
			GameObject pNode = Instantiate(Resources.Load ("Prefabs/Items/Subscription")) as GameObject;
			Transform transProduct = pNode.transform;
			transProduct.SetParent(transform);
			transProduct.localScale = Vector3.one;
			transProduct.Find("Name").GetComponent<Text>().text = listSub[i].name;
			transProduct.Find("TextPrice/ChargeDescription").GetComponent<Text>().text = listSub[i].chargeDescription;
			transProduct.Find("TextPrice").GetComponent<Text>().text = listSub[i].price + " USD";

			Button b = transProduct.Find("Button Add To Cart").GetComponent<Button>();
			int id = listSub[i].id;
			b.onClick.AddListener(() => AddToCart(id));
		}
	}

	private void SortAreaByPrice(bool isLowFirst){
		RemoveChildProduct();
		List<SubscriptionModel> listSub = SubscriptionController.Instance.GetListSubscriptionModels();
		if(isLowFirst){
			listSub = listSub.OrderBy(o=>o.price).ToList();
		} else {
			listSub = listSub.OrderByDescending(o=>o.price).ToList();
		}
		for (int i = 0; i < listSub.Count; i++) {
			GameObject pNode = Instantiate(Resources.Load ("Prefabs/Items/Subscription")) as GameObject;
			Transform transProduct = pNode.transform;
			transProduct.SetParent(transform);
			transProduct.localScale = Vector3.one;
			transProduct.Find("Name").GetComponent<Text>().text = listSub[i].name;
			transProduct.Find("TextPrice/ChargeDescription").GetComponent<Text>().text = listSub[i].chargeDescription;
			transProduct.Find("TextPrice").GetComponent<Text>().text = listSub[i].price + " USD";
			
			Button b = transProduct.Find("Button Add To Cart").GetComponent<Button>();
			int id = listSub[i].id;
			b.onClick.AddListener(() => AddToCart(id));
		}
	}
	
	private void SortByFree(){
		RemoveChildProduct();
		List<SubscriptionModel> listSub = SubscriptionController.Instance.GetListSubscriptionModels();
		for (int i = 0; i < listSub.Count; i++) {
			if(listSub[i].price == 0){
				GameObject pNode = Instantiate(Resources.Load ("Prefabs/Items/Subscription")) as GameObject;
				Transform transProduct = pNode.transform;
				transProduct.SetParent(transform);
				transProduct.localScale = Vector3.one;
				transProduct.Find("Name").GetComponent<Text>().text = listSub[i].name;
				transProduct.Find("TextPrice/ChargeDescription").GetComponent<Text>().text = listSub[i].chargeDescription;
				transProduct.Find("TextPrice").GetComponent<Text>().text = listSub[i].price + " USD";
				
				Button b = transProduct.Find("Button Add To Cart").GetComponent<Button>();
				int id = listSub[i].id;
				b.onClick.AddListener(() => AddToCart(id));
			}
		}
	}

	private double getSaleOffPrice(double oldPrice, int percentSaleOff) {
		double salePrice;
		salePrice = oldPrice/100 * percentSaleOff;
		return oldPrice - salePrice;
	}

	private void RemoveChildProduct() {
		for (int i = 0; i < transform.childCount; i++) {
			Destroy(transform.GetChild(i).gameObject);
		}
	}

	public void SortProduct(Button pSender){
		switch(pSender.name){
		case "Button Best Selling":
			ReloadArea();
			break;
		case "Button Lower Price":
			SortAreaByPrice(true);
			break;
		case "Button Bigger Price":
			SortAreaByPrice(false);
			break;
		case "Button Free":
			SortByFree();
			break;
		default:break;
		}
	}

	private void AddToCart(int id){
		SubscriptionModel subModel = SubscriptionController.Instance.GetModelById(id);
		CartManager.Instance.AddToCart(subModel);
	}
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class VirtualCurrency : MonoBehaviour {
	public GameObject detailsUI;
	public GameObject tabVirtualUI;
	public GameObject failUI;
	private const string bonusStr = "+ {0} bonus Shards";
	private const string priceStr = "{0} USD";
	// Use this for initialization
	void Start () {
		ReloadArea();
	}
	
	private void ReloadArea() {
		RemoveChildProduct();
		List<VirtualCurrencyModel> listVirtual = VirtualCurrencyController.Instance.GetListVirtualCurrencyModel();
		for (int i = 0; i < listVirtual.Count; i++) {
			GameObject pNode = Instantiate(Resources.Load ("Prefabs/Items/Virtual Currency")) as GameObject;
			Transform transProduct = pNode.transform;
			transProduct.SetParent(transform);
			transProduct.localScale = Vector3.one;
			transProduct.Find("Layout/Value").GetComponent<Text>().text = listVirtual[i].name;
			if(listVirtual[i].bonusValue != 0) {
				transProduct.Find("Bonus Shard").GetComponent<Text>().text = string.Format(bonusStr,listVirtual[i].bonusValue);
			}
			transProduct.Find("TextPrice").GetComponent<Text>().text = string.Format (priceStr,listVirtual[i].price);

			Button b = transProduct.Find("Button Add To Cart").GetComponent<Button>();
			int id = listVirtual[i].id;
			b.onClick.AddListener(() => AddToCart(id));
		}
	}

	private void SortAreaByPrice(bool isLowFirst){
		RemoveChildProduct();
		List<VirtualCurrencyModel> listVirtual = VirtualCurrencyController.Instance.GetListVirtualCurrencyModel();
		if(isLowFirst){
			listVirtual = listVirtual.OrderBy(o=>o.price).ToList();
		} else {
			listVirtual = listVirtual.OrderByDescending(o=>o.price).ToList();
		}
		for (int i = 0; i < listVirtual.Count; i++) {
			GameObject pNode = Instantiate(Resources.Load ("Prefabs/Items/Virtual Currency")) as GameObject;
			Transform transProduct = pNode.transform;
			transProduct.SetParent(transform);
			transProduct.localScale = Vector3.one;
			transProduct.Find("Layout/Value").GetComponent<Text>().text = listVirtual[i].name;
			if(listVirtual[i].bonusValue != 0) {
				transProduct.Find("Bonus Shard").GetComponent<Text>().text = string.Format(bonusStr,listVirtual[i].bonusValue);
			}
			transProduct.Find("TextPrice").GetComponent<Text>().text = string.Format (priceStr,listVirtual[i].price);
			
			Button b = transProduct.Find("Button Add To Cart").GetComponent<Button>();
			int id = listVirtual[i].id;
			b.onClick.AddListener(() => AddToCart(id));
		}
	}
	
	private void SortByFree(){
		RemoveChildProduct();
		List<VirtualCurrencyModel> listVirtual = VirtualCurrencyController.Instance.GetListVirtualCurrencyModel();
		for (int i = 0; i < listVirtual.Count; i++) {
			if(listVirtual[i].price == 0){
				GameObject pNode = Instantiate(Resources.Load ("Prefabs/Items/Virtual Currency")) as GameObject;
				Transform transProduct = pNode.transform;
				transProduct.SetParent(transform);
				transProduct.localScale = Vector3.one;
				transProduct.Find("Layout/Value").GetComponent<Text>().text = listVirtual[i].name;
				if(listVirtual[i].bonusValue != 0) {
					transProduct.Find("Bonus Shard").GetComponent<Text>().text = string.Format(bonusStr,listVirtual[i].bonusValue);
				}
				transProduct.Find("TextPrice").GetComponent<Text>().text = string.Format (priceStr,listVirtual[i].price);
				
				Button b = transProduct.Find("Button Add To Cart").GetComponent<Button>();
				int id = listVirtual[i].id;
				b.onClick.AddListener(() => AddToCart(id));
			}
		}
	}

	private void AddToCart(int id){
		VirtualCurrencyModel vcModel = VirtualCurrencyController.Instance.GetModelById(id);
		CartManager.Instance.AddToCart(vcModel);
	}

	private double getSaleOffPrice(double oldPrice, int percentSaleOff) {
		double salePrice;
		salePrice = oldPrice/100 * percentSaleOff;
		return oldPrice - salePrice;
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
	
	private void RemoveChildProduct() {
		for (int i = 0; i < transform.childCount; i++) {
			Destroy(transform.GetChild(i).gameObject);
		}
	}
}

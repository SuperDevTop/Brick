using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class CartManager : MonoBehaviour {
	public Button btnCart;
	public Text txtCart;
	public Image imgCart;
	public UIShardsBalance balanceUI;

	private static CartManager _instance;
	public static CartManager Instance {
		get {
			if (_instance == null) {
				GameObject dtMan = GameObject.Find ("PW_Controller") as GameObject;
				if (dtMan == null) {
					dtMan = GameObject.Instantiate (Resources.Load ("Prefabs/PW_Controller")) as GameObject;
				}
				_instance = dtMan.GetComponent<CartManager> ();
			}
			return _instance;
		}
	}

	private const string noItemStr = "Let's start add cart";
	private const string hasItemStr = "{0} items ({1:C2} USD)";

	private Dictionary<int,VirtualCurrencyModel> _listVirtual = new Dictionary<int, VirtualCurrencyModel>();
	private Dictionary<int,SubscriptionModel> _listSubscription = new Dictionary<int,SubscriptionModel> ();
	public double totalMoney;

	public Dictionary<int,VirtualCurrencyModel> GetVirtualCart() {
		return _listVirtual;
    }

	public Dictionary<int, SubscriptionModel> GetSubscriptionCart() {
		return _listSubscription;
	}

    public void AddToCart(VirtualCurrencyModel vcModel) {
		if(_listVirtual.ContainsKey(vcModel.id)){
			_listVirtual[vcModel.id].quantity++;
		} else {
			_listVirtual.Add(vcModel.id,vcModel);
		}
		UpdateTotalMoney();
    }

	public void AddToCart(SubscriptionModel subModel) {
		if(_listSubscription.ContainsKey(subModel.id)){
			_listSubscription[subModel.id].quantity++;
		} else {
			_listSubscription.Add(subModel.id,subModel);
		}
		UpdateTotalMoney();
	}

	private void UpdateTotalMoney() {
		totalMoney = 0;
		foreach(var item in _listVirtual){
			totalMoney += item.Value.price * item.Value.quantity;
		}
		foreach(var item in _listSubscription){
			totalMoney += item.Value.price * item.Value.quantity;
		}
		UpdateButtonCart();
	}

	public int GetCartCount() {
		return _listVirtual.Count + _listSubscription.Count;
	}

	private void UpdateButtonCart() {
		if(_listVirtual.Count != 0 || _listSubscription.Count != 0) {
			btnCart.interactable = true;
			txtCart.text = string.Format(hasItemStr,GetCartCount(),totalMoney);
		} else if(_listVirtual.Count == 0 && _listSubscription.Count == 0){
			btnCart.interactable = false;
			txtCart.text = noItemStr;
		}
	}

	public double GetTotalPriceOfAProduct(int key){
		if(_listVirtual.ContainsKey(key)) {
			return _listVirtual[key].price * _listVirtual[key].quantity;
		} else if(_listSubscription.ContainsKey(key)) {
			return _listSubscription[key].price * _listSubscription[key].quantity;
		}
		return 0;
	}

	public int GetQuantityOfAProduct(int key){
		if(_listVirtual.ContainsKey(key)) {
			return _listVirtual[key].quantity;
		} else if(_listSubscription.ContainsKey(key)) {
			return _listSubscription[key].quantity;
		}
		return 0;
	}

	public void ProgessPurchase() {
		int shardToAdd = 0;
		foreach(var item in _listVirtual){
			shardToAdd += (item.Value.shardValue + item.Value.bonusValue) * item.Value.quantity;
		}
		FakeAccountController.Instance.IncreaseShard(shardToAdd);
		balanceUI.UpdateTextBalance();
	}

    public void RemoveProduct(int key) {
		if(_listVirtual.ContainsKey(key)){
			double subMoney = _listVirtual[key].price * _listVirtual[key].quantity;
			totalMoney -= subMoney;
			_listVirtual.Remove(key);
		} else if(_listSubscription.ContainsKey(key)) {
			double subMoney = _listSubscription[key].price * _listSubscription[key].quantity;
			totalMoney -= subMoney;
			_listSubscription.Remove(key);
		}
		UpdateButtonCart();
    }	

    public void MinusProduct(int key) {
		if(_listVirtual.ContainsKey(key)) {
			if(_listVirtual[key].quantity > 1) {
				_listVirtual[key].quantity--;
				totalMoney -= _listVirtual[key].price;
			}
		} else if(_listSubscription.ContainsKey(key)){
			if(_listSubscription[key].quantity > 1) {
				_listSubscription[key].quantity--;
				totalMoney -= _listSubscription[key].price;
			}
		}
    }

    public void PlusProduct(int key) {
		if(_listVirtual.ContainsKey(key)) {
			_listVirtual[key].quantity += 1;
			totalMoney += _listVirtual[key].price;
		} else if(_listSubscription.ContainsKey(key)) {
			_listSubscription[key].quantity += 1;
			totalMoney += _listSubscription[key].price;
		}
    }

	public void ClearCart() {
		_listVirtual.Clear();
		_listSubscription.Clear ();
		totalMoney = 0;
		UpdateButtonCart();
	}
}

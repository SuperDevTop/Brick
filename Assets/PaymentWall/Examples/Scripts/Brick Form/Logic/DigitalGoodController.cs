using UnityEngine;
using System.Collections.Generic;
using SimpleJSON;

public class DigitalGoodController : MonoBehaviour {
	private static DigitalGoodController _instance;
	public static DigitalGoodController Instance {
		get{
			if(_instance == null) {
				GameObject dgManager = GameObject.Find ("PW_Controller") as GameObject;
				if(dgManager == null) {
					dgManager = GameObject.Instantiate(Resources.Load ("Prefabs/PW_Controller")) as GameObject;
				}
				_instance = dgManager.GetComponent<DigitalGoodController>();
	   		 }	
	   		 return _instance;
		}
	}
	
	private Dictionary<int,DigitalGoodModel> _listDigitalGood = new Dictionary<int,DigitalGoodModel>();
	
	void Awake () {
		string json = ((TextAsset)Resources.Load("Data/digitals")).text;
		JSONNode node = JSON.Parse(json);
		for(int i=0;i<node.Count;i++){
			DigitalGoodModel dModel = new DigitalGoodModel();
			dModel.id = node[i]["Id"].AsInt;
			dModel.name = node[i]["ProductName"];
			dModel.description = node[i]["Description"];
			dModel.isSale = node[i]["IsSale"].AsBool;
			dModel.sprName = node[i]["ImgIndex"].AsInt;
			dModel.category = (CATEGORY_TYPE)node[i]["Category"].AsInt;
			dModel.itemType = (ITEM_TYPE)node[i]["Type"].AsInt;
			dModel.shards = node[i]["Shards"].AsInt;
			_listDigitalGood.Add(dModel.id,dModel);
		}
	}
	
	public bool PurchaseDigitalGoods(int id){
		bool isSuccessPurchase = false;
		if(_listDigitalGood.ContainsKey(id)) {
			DigitalGoodModel dgModel = _listDigitalGood[id];
			isSuccessPurchase = FakeAccountController.Instance.DecreaseShard(dgModel.shards);
		}
		return isSuccessPurchase;
	}
	
	public List<DigitalGoodModel> GetDigitalGoods(CATEGORY_TYPE catType,ITEM_TYPE itemType) {
		List<DigitalGoodModel> listResult = new List<DigitalGoodModel>();
		foreach(KeyValuePair<int,DigitalGoodModel> model in _listDigitalGood){
			if(catType == CATEGORY_TYPE.ALL){
				if(itemType == ITEM_TYPE.ALL){
					listResult.Add(model.Value);
				} else {
					if(model.Value.itemType == itemType){
						listResult.Add (model.Value);
					}
				}
			} else if(model.Value.category == catType) {
				if(itemType == ITEM_TYPE.ALL) {
					listResult.Add (model.Value);
				} else if(model.Value.itemType == itemType) {
					listResult.Add(model.Value);
				}
			}
		}
		return listResult;
	}
	
	public List<ITEM_TYPE> GetTypeByCategory(CATEGORY_TYPE catType){
		List<ITEM_TYPE> listResult = new List<ITEM_TYPE>();
		switch(catType){
			case CATEGORY_TYPE.CONSUMABLE:
				listResult.Add(ITEM_TYPE.HEALTH);
				listResult.Add(ITEM_TYPE.MANA);
				listResult.Add(ITEM_TYPE.SORCERY);
				break;
			case CATEGORY_TYPE.UPGRADES:
				listResult.Add(ITEM_TYPE.WEAPONS);
				listResult.Add(ITEM_TYPE.ARMOR);
				listResult.Add(ITEM_TYPE.ACCESSORIES);
				break;
			default:break;
		}
		return listResult;
	}
}

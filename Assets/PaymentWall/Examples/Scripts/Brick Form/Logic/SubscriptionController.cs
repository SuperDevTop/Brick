using UnityEngine;
using System.Collections.Generic;
using SimpleJSON;

public class SubscriptionController : MonoBehaviour {
	private static SubscriptionController _instance;
	public static SubscriptionController Instance {
		get{
			if(_instance == null) {
				GameObject vcManager = GameObject.Find ("PW_Controller") as GameObject;
				if(vcManager == null) {
					vcManager = GameObject.Instantiate(Resources.Load ("Prefabs/PW_Controller")) as GameObject;
				}
				_instance = vcManager.GetComponent<SubscriptionController>();
	   		 }	
	   		 return _instance;
		}
	}
	
	private Dictionary<int,SubscriptionModel> _listSubscription = new Dictionary<int,SubscriptionModel>();
	
	void Start () {
		string json = ((TextAsset)Resources.Load("Data/subscription")).text;
		JSONNode node = JSON.Parse(json);
		for(int i=0;i<node.Count;i++) {
			SubscriptionModel sModel = new SubscriptionModel();
			sModel.id = node[i]["Id"].AsInt;
			sModel.name = node[i]["ProductName"];
			sModel.chargeDescription = node[i]["ChargeDesc"];
			sModel.price = node[i]["Price"].AsDouble;
			sModel.quantity = 1;
			_listSubscription.Add(sModel.id,sModel);
		}
	}
	
	public List<SubscriptionModel> GetListSubscriptionModels() {
		List<SubscriptionModel> listResult = new List<SubscriptionModel>();
		foreach (var item in _listSubscription) {
			listResult.Add(item.Value);
		}
		return listResult;
	}

	public SubscriptionModel GetModelById(int id) {
		return _listSubscription[id];
	}
}

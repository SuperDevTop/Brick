using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public enum LANGUAGE_FLAG {
	EN = 0,
	JP,
	CN,
	DE,
	SP,
	IT
}

public class GlobalSettings : MonoBehaviour {
	private static GlobalSettings _instance;
	private Dictionary<string,string> dictLocal;
	private string targetColumn = "EN";

	public static GlobalSettings Instance {
		get{
			if(_instance == null) {
				GameObject gSettings = GameObject.Find ("PW_Settings") as GameObject;
				if(gSettings == null) {
					gSettings = GameObject.Instantiate(Resources.Load ("Prefabs/PW_Settings")) as GameObject;
				}
				_instance = gSettings.GetComponent<GlobalSettings>();
			}
			return _instance;
		}
	}

	public void SwitchLanguage(LANGUAGE_FLAG currentLanguage) {
		switch(currentLanguage) {
		case LANGUAGE_FLAG.EN:
			targetColumn = "EN";
			break;
		case LANGUAGE_FLAG.JP:
			targetColumn = "JP";
			break;
		case LANGUAGE_FLAG.CN:
			targetColumn = "CN";
			break;
		case LANGUAGE_FLAG.DE:
			targetColumn = "DE";
			break;
		case LANGUAGE_FLAG.SP:
			targetColumn = "SP";
			break;
		case LANGUAGE_FLAG.IT:
			targetColumn = "IT";
			break;
		default:
			targetColumn = "EN";
			break;
		}
		string json = ((TextAsset)Resources.Load("Data/localization")).text;
		JSONNode node = JSON.Parse(json);
		dictLocal = new Dictionary<string, string>();
		foreach (var key in node.Keys) {
			dictLocal.Add(key,node[key][targetColumn]);
		}
	}

	public string GetLocalText(string key) {
		return dictLocal[key];
	}
}

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SimpleJSON;

public class FailedUI : MonoBehaviour {
	public UINavigation navHandler;
	public GameObject searchButton;
	public GameObject btnCart;
	public Text txtErrorNotification;

	void OnEnable() {
		searchButton.SetActive(false);
		btnCart.SetActive(false);
		navHandler.gameObject.SetActive (false);
	}

	public void SetErrorMessage(string message){
		if(message!=null){
			txtErrorNotification.text = message;
		}else{
			txtErrorNotification.text = "Something wrong happen!!!";
		}
	}

	public void TryAgain(Button pSender){
		gameObject.SetActive(false);
		navHandler.gameObject.SetActive(true);
		searchButton.SetActive(true);
		btnCart.SetActive(true);
		navHandler.SwitchTab(pSender);
	}
}

using UnityEngine;
using UnityEngine.UI;

public class UIOutOfShardPopup : MonoBehaviour {
	public UINavigation navHandler;

	public void OnClose() {
		gameObject.SetActive (false);
	}

	public void BuyMoreShard(Button pSender) {
		OnClose();
		navHandler.SwitchTab(pSender);
	}
}

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIShardsBalance : MonoBehaviour {
	public GameObject popupOutOfShard;
	public Text txtShard;

	void Update() {
		txtShard.text = FakeAccountController.Instance.GetShard().ToString();
	}

	public void UpdateTextBalance() {
		txtShard.text = FakeAccountController.Instance.GetShard().ToString();
	}

	public void OnClickWhenHaveNoShard() {
		if(FakeAccountController.Instance.GetShard() == 0) {
			popupOutOfShard.SetActive(true);
		}
	}
}

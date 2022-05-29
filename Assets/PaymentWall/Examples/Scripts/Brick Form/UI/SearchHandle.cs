using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class SearchHandle : MonoBehaviour {
	public GameObject objInputSearch;
	public InputField inputSearch;
	private bool isOpenSearch;

	public GameObject digitalField;
	public GameObject virtualField;
	public GameObject subscriptionField;

	void OnEnable() {
		isOpenSearch = false;
		objInputSearch.SetActive(false);
	}

	void OnDisable() {
		isOpenSearch = false;
		objInputSearch.SetActive(false);
	}

	public void SwitchTab() {
		if(isOpenSearch) {
			inputSearch.text = "";
			objInputSearch.SetActive(false);
			isOpenSearch = false;

			if(digitalField.activeSelf) {
				ResetField(digitalField);
			}
			
			if(virtualField.activeSelf) {
				ResetField(virtualField);
			}
			
			if(subscriptionField.activeSelf) {
				ResetField(subscriptionField);
			}
		}
	}

	public void OnClickSearch() {
		if(isOpenSearch) {
			inputSearch.text = "";
			objInputSearch.SetActive(false);
			isOpenSearch = false;
			
			if(digitalField.activeSelf) {
				ResetField(digitalField);
			}
			
			if(virtualField.activeSelf) {
				ResetField(virtualField);
			}
			
			if(subscriptionField.activeSelf) {
				ResetField(subscriptionField);
			}
		} else {
			objInputSearch.SetActive(true);
			isOpenSearch = true;
		}
	}

	public void OnSearch() {
		if(digitalField.activeSelf) {
			//ProgressList(digitalField);
		}

		if(virtualField.activeSelf) {
			//ProgressList(virtualField);
		}

		if(subscriptionField.activeSelf) {
			//ProgressList(subscriptionField);
		}
	}

	private void ProgressList(GameObject listContainer) {
		for (int i = 0; i < listContainer.transform.childCount; i++) {
			Transform tChild = listContainer.transform.GetChild(i);
			string pName = tChild.Find("TextName").GetComponent<Text>().text;
			if(!pName.Contains(inputSearch.text)) {
				tChild.gameObject.SetActive(false);
			} else {
				continue;
			}
		}
	}

	private void ResetField(GameObject listContainer){
		for (int i = 0; i < listContainer.transform.childCount; i++) {
			Transform tChild = listContainer.transform.GetChild(i);
			tChild.gameObject.SetActive(true);
		}
	}
}

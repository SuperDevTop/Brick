using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent (typeof(Text)) ]
public class TextLocalization : MonoBehaviour {
	public Text targetText;
	public string key;

	void Start () {
		targetText.text = GlobalSettings.Instance.GetLocalText(key);
	}
}

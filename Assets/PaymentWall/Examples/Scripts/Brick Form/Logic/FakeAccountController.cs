using UnityEngine;
using System.Collections;

public class FakeAccountController : MonoBehaviour {
	private static FakeAccountController _instance;

	public static FakeAccountController Instance {
		get {
			if (_instance == null) {
				GameObject dgManager = GameObject.Find ("PW_Controller") as GameObject;
				if (dgManager == null) {
					dgManager = GameObject.Instantiate (Resources.Load ("Prefabs/PW_Controller")) as GameObject;
				}
				_instance = dgManager.GetComponent<FakeAccountController> ();
			}	
			return _instance;
		}
	}
	
	public const string accountName = "Jimmy Nowel";
	private int _shardAmount;
	private bool _isLoggedIn;
	
	void Start() {
		_shardAmount = 5;
		_isLoggedIn = true;
	}

	public bool IsAccountOnline() {
		return _isLoggedIn;
	}
	
	public void Login() {
		_isLoggedIn = true;
	}
	
	public void Logout() {
		_isLoggedIn = false;
	}

	public int GetShard() {
		return _shardAmount;
	}
	
	public bool IncreaseShard(int amount) {
		if (amount <= 0) {
			return false;
		}
		
		if (_isLoggedIn) {
			_shardAmount += amount;
			return true;
		} else {
			return false;
		}
	}
	
	public bool DecreaseShard(int amount) {
		if (amount <= 0) {
			return false;
		}
		
		if (_isLoggedIn) {
			int valueAfter = _shardAmount - amount;
			if (valueAfter < 0) {
				return false;
			} else {
				_shardAmount -= amount;
				return true;	
			}
		} else {
			return false;	
		}
	}
}

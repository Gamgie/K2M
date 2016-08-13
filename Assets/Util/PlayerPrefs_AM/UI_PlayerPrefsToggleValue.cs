using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

[RequireComponent(typeof(Toggle))]
public class UI_PlayerPrefsToggleValue : MonoBehaviour {

	public string key;
	public bool loadAtStart;
	
	private Toggle _toggle;

	// Use this for initialization
	void Awake () {
		_toggle = GetComponent<Toggle>();
		
		if(loadAtStart)
		{
			Load ();
		}
	}
	
	public void Save()
	{
		PlayerPrefs_AM.SetBool(key, _toggle.isOn);
	}
	
	public bool Load()
	{
		//_toggle.isOn = !_toggle.isOn; // Here to ensure OnValueChanged is called.
		_toggle.isOn = PlayerPrefs_AM.GetBool(key);
		return _toggle.isOn;
	}
}

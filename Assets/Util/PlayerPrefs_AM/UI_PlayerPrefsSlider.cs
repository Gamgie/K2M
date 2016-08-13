using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Slider))]
public class UI_PlayerPrefsSlider : MonoBehaviour {

	public string key;
	public bool loadAtStart;
	public bool saveOnDestroy;

	private Slider _slider;
	// Use this for initialization
	void Awake () {
		_slider = GetComponent<Slider>();

		if(loadAtStart)
		{
			Load ();
		}
	}

	public void Save()
	{
		PlayerPrefs_AM.SetFloat(key,_slider.value);
	}

	public float Load()
	{
		_slider.value = PlayerPrefs_AM.GetFloat(key, _slider.value);
		return _slider.value;
	}

	void OnDestroy()
	{
		if(saveOnDestroy)
		{
			Save();
		}
	}
}

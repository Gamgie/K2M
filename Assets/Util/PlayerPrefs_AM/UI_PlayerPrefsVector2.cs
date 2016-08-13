using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI_PlayerPrefsVector2 : MonoBehaviour {

	public bool loadAtStart;
    public Vector2 defaultValue = Vector2.zero;
	public string key;
	public InputField XInputField;
	public InputField YInputField;

	void Awake()
	{
		if(loadAtStart)
		{
			Load (defaultValue);
		}
	}

	public Vector2 GetVector2()
	{
		return new Vector2(float.Parse(XInputField.text),float.Parse(YInputField.text));
	}

	public void SetVector2(Vector2 v)
	{
		XInputField.text = v.x.ToString();
		YInputField.text = v.y.ToString();
	}

	public void Save()
	{
		PlayerPrefs_AM.SetVector3(key, new Vector3(float.Parse(XInputField.text),float.Parse(YInputField.text),0));
	}

	public Vector2 Load(Vector2 defaultValue)
	{
		Vector3 result = PlayerPrefs_AM.GetVector3(key, defaultValue);
		SetVector2(result);
		return result;
	}
}

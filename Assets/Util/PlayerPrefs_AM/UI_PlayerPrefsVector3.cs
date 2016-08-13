using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI_PlayerPrefsVector3 : MonoBehaviour {

	public enum TransformType {POSITION, ROTATION};

	public bool loadAtStart;
    public Vector3 defaultValue = Vector3.zero;
	public TransformType transformType = TransformType.POSITION;
	public string key;
	public InputField XInputField;
	public InputField YInputField;
	public InputField ZInputField;

     string _tempKey;

    void Start()
	{
		if(loadAtStart)
		{
			Load (defaultValue);
		}

        _tempKey = key + "_temp";
	}

	public Vector3 GetVector3()
	{
		return new Vector3(float.Parse(XInputField.text),float.Parse(YInputField.text),float.Parse(ZInputField.text));
	}

    public void SetVector3(Vector3 v, bool isTemp)
    {
        SetVector3InputField(v);
        if(isTemp)
        {
            SaveTemp();
        }
        else
        {
            Save();
        }
    }

	void SetVector3InputField(Vector3 v)
	{
		XInputField.text = v.x.ToString();
		YInputField.text = v.y.ToString();
		ZInputField.text = v.z.ToString();
	}

	public void Save()
	{
		PlayerPrefs_AM.SetVector3(key, new Vector3(float.Parse(XInputField.text),float.Parse(YInputField.text),float.Parse(ZInputField.text)),true);
	}

    public void SaveTemp()
    {
        PlayerPrefs_AM.SetVector3(_tempKey, new Vector3(float.Parse(XInputField.text), float.Parse(YInputField.text), float.Parse(ZInputField.text)),true);
    }

	public Vector3 Load(Vector3 defaultValue)
	{
		Vector3 result = PlayerPrefs_AM.GetVector3(key, defaultValue);
		SetVector3InputField(result);
		return result;
	}

    public void ReloadValue()
    {
        SetVector3InputField(PlayerPrefs_AM.GetVector3(key, defaultValue));
        PlayerPrefs_AM.DeleteKey(_tempKey);
        PlayerPrefUpdateBroadcast.Instance.UpdatePlayerPrefs(key);
    }
}

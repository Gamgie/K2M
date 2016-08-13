using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(InputField))]
public class UI_PlayerPrefsInputField : MonoBehaviour {

	public enum InputFieldType {INT, FLOAT, STRING}

	public string key;
	public bool loadAtStart;
    public string defaultValue;
	public InputFieldType inputFieldType;

	private InputField _inputField;

	void Awake () {
		_inputField = GetComponent<InputField>();
		
		if(loadAtStart)
		{
			string put = "";
			Load (out put);
		}
	}
	
	public void Save()
	{
		switch(inputFieldType)
		{
		case InputFieldType.INT:
			PlayerPrefs_AM.SetInt(key, int.Parse(_inputField.text));
			return;
		case InputFieldType.FLOAT:
			PlayerPrefs_AM.SetFloat(key, float.Parse(_inputField.text));
			return;
		case InputFieldType.STRING:
			PlayerPrefs_AM.SetString(key, _inputField.text);
			return;
		default:
			return;
		}
	}

    public float GetFloat()
    {
        if (inputFieldType == InputFieldType.FLOAT)
            return float.Parse(_inputField.text);
        else
            return float.Parse(defaultValue);
    }

    public float GetInt()
    {
        if (inputFieldType == InputFieldType.INT)
            return int.Parse(_inputField.text);
        else
            return int.Parse(defaultValue);
    }

    public string GetString()
    {
        if (inputFieldType == InputFieldType.STRING)
            return _inputField.text;
        else
            return defaultValue;
    }

    // Note pour plus tard : on ne peut pas surcharger en c# une fonction avec un retour différent..

    // Load data according to key and return it.
    public void Load(out string output)
	{
		UpdateField();
		output = _inputField.text;
	}

    // Load data according to key and return it.
	public void Load(out float output)
	{
		UpdateField();
		output = float.Parse(_inputField.text);
	}

    // Load data according to key and return it.
    public void Load(out int output)
	{
		UpdateField();
		output = int.Parse(_inputField.text);
	}

    // Load data and update field.
    public void Load()
    {
        UpdateField();
    }

	private void UpdateField()
	{
		switch(inputFieldType)
		{
		case InputFieldType.INT:
			_inputField.text = PlayerPrefs_AM.GetInt(key, int.Parse(defaultValue)).ToString();
			return;
		case InputFieldType.FLOAT:
			_inputField.text = PlayerPrefs_AM.GetFloat(key, float.Parse(defaultValue)).ToString();
			return;
		case InputFieldType.STRING:
			_inputField.text = PlayerPrefs_AM.GetString(key, defaultValue);
			return;
		default:
			return;
		}
	}
}

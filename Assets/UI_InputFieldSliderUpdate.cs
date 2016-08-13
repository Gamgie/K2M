using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(InputField))]
public class UI_InputFieldSliderUpdate : MonoBehaviour {

	public void UpdateInputField(string value)
    {
        GetComponent<InputField>().text = value;
    }

    public void UpdateInputField(float value)
    {
        GetComponent<InputField>().text = ""+value;
    }

    public void UpdateInputField(int value)
    {
        GetComponent<InputField>().text = "" + value;
    }
}

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UI_ToggleVisibilityButton : MonoBehaviour {

    public GameObject visibilityTarget;
    public Color activationColor;

    private Color _initialNormalColor;

    void Awake()
    {
        _initialNormalColor = GetComponent<Button>().image.color;
    }

    public void ToggleVisibility()
    {
        if(visibilityTarget)
        {
            visibilityTarget.SetActive(!visibilityTarget.activeSelf);

            if(visibilityTarget.activeSelf)
            {
                GetComponent<Button>().image.color = activationColor;
            }
            else
            {
                GetComponent<Button>().image.color = _initialNormalColor;
            }
        }
    }
}

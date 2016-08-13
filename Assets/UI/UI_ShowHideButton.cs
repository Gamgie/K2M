using UnityEngine;
using System.Collections;

public class UI_ShowHideButton : MonoBehaviour {

    public GameObject panel;
    public bool hideOnStart;

    void Start()
    {
        if (hideOnStart)
            ShowHide(false);
    }

    public void ShowHide()
    {
        panel.SetActive(!panel.activeSelf);
    }

    public void ShowHide(bool active)
    {
        panel.SetActive(active);
    }
}

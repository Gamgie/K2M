using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Button))]
public class UI_MenuTabTabButton : MonoBehaviour
{
    public GameObject panel;
    public UI_MenuTabManager tabMgr;

    private Button _button;

    public void Start()
    {
        if (tabMgr == null)
        tabMgr = transform.parent.GetComponent<UI_MenuTabManager>();

        _button = GetComponent<Button>();
        _button.onClick.AddListener(ShowPanel);
    }

    public void ShowPanel()
    {
        if (panel != null)
        {
            if ((tabMgr.lockIfOpen && tabMgr.openTab == null) || !tabMgr.lockIfOpen)
            {
                panel.SetActive(true);
                if (tabMgr.changeColor)
                    this.GetComponent<Image>().color = tabMgr.colorOn;

                tabMgr.OpenTab(gameObject);
            }
        }
    }

    public void HidePanel()
    {
        if (panel != null)
        {
            panel.SetActive(false);
            if (tabMgr.changeColor)
                this.GetComponent<Image>().color = tabMgr.colorOff;
        }
    }
}

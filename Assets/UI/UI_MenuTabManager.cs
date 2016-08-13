using UnityEngine;
using System.Collections;

public class UI_MenuTabManager : MonoBehaviour {

    public GameObject openTab;

    public GameObject tabOnStart;

    public bool lockIfOpen;
    public bool changeColor;
    public Color colorOn;
    public Color colorOff;

	// Use this for initialization
	void Start () {
	    if (tabOnStart!= null)
        {
           UI_MenuTabTabButton tabButtonScpt = tabOnStart.GetComponent<UI_MenuTabTabButton>();
            if (tabButtonScpt != null)
                tabButtonScpt.ShowPanel();
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OpenTab(GameObject newTab)
    {
        if (openTab != null && openTab != newTab)
        {
            UI_MenuTabTabButton tabButtonScpt = openTab.GetComponent<UI_MenuTabTabButton>();
            if (tabButtonScpt != null)
                tabButtonScpt.HidePanel();
        }
        
        openTab = newTab;
    }
    
    public void CloseTab()
    {
        if (openTab != null)
        {
            UI_MenuTabTabButton tabButtonScpt = openTab.GetComponent<UI_MenuTabTabButton>();
            if (tabButtonScpt != null)
                tabButtonScpt.HidePanel();
            openTab = null;
        }
    }
}

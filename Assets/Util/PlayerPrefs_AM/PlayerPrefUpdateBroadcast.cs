using UnityEngine;
using System.Collections;

public class PlayerPrefUpdateBroadcast : MonoBehaviour {
    
    // Event Handler
    public delegate void OnPlayerPrefsUpdatedEvent(string playerPrefKey);
    public event OnPlayerPrefsUpdatedEvent OnPlayerPrefsUpdated;

    private static PlayerPrefUpdateBroadcast _instance;

    public static PlayerPrefUpdateBroadcast Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = GameObject.FindObjectOfType<PlayerPrefUpdateBroadcast>();
            }

            return _instance;
        }
    }

    void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
        }
        else if(_instance != this)
        {
            Destroy(this.gameObject);
        }
    }

	public void UpdatePlayerPrefs(string keyToUpdate)
    {
        OnPlayerPrefsUpdated(keyToUpdate);
    }
}

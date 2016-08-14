using UnityEngine;

[RequireComponent(typeof(Transform))]
public class PlayerPrefsTransform : MonoBehaviour {

	public string positionKey;
	public string rotationKey;
	public bool loadAtStart;
	public bool saveOnDestroy = true;

	void Start()
	{
		if(loadAtStart)
		{
			Load();
		}
	}

	public void Save()
	{
		PlayerPrefs_AM.SetVector3(positionKey,transform.position);
		PlayerPrefs_AM.SetVector3(rotationKey, transform.rotation.eulerAngles);
	}

	public void Load()
	{
		transform.position = PlayerPrefs_AM.GetVector3(positionKey,transform.position);
		transform.eulerAngles = PlayerPrefs_AM.GetVector3(rotationKey, transform.rotation.eulerAngles);
	}

	void OnDestroy()
	{
		if(saveOnDestroy)
		{
			Save();
		}
	}

}

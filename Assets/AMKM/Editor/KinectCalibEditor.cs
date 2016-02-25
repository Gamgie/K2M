using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(KinectCalib))]
[CanEditMultipleObjects]
public class KinectPCLEditor : Editor {

    KinectCalib k;
	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public override void OnInspectorGUI()
    {
        if(k == null) k = (KinectCalib)this.target;

        base.DrawDefaultInspector();

        if (GUILayout.Button("Save"))
        {
            k.saveConfig();
        }

        if (GUILayout.Button("Load"))
        {
            k.loadConfig();
        }
    }
}

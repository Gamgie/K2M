using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(OffCenterProjection))]
public class OffCenterProjectionEditor : Editor
{


    OffCenterProjection ocp;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void OnInspectorGUI()
    {
        if (ocp == null) ocp = (OffCenterProjection)this.target;

        base.DrawDefaultInspector();

        if (GUILayout.Button("Save"))
        {
            ocp.saveConfig();
        }

        if (GUILayout.Button("Load"))
        {
            ocp.loadConfig();
        }

        if (GUILayout.Button("Disable"))
        {
            ocp.disableAndReset();
        }
    }
}
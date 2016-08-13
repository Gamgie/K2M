using UnityEngine;
using System.Collections;

public class KinectCalibrationPrecision : MonoBehaviour {

    public float positionMinStep;
    public float positionMaxStep;
    public float rotationMinStep;
    public float rotationMaxStep;
    public UI_PlayerPrefsVector3 positionPanel;
    public UI_PlayerPrefsVector3 rotationPanel;

    private string _playerPrefKey;

    public void UpdatePlayerPrefKey(string playerPrefKey)
    {
        _playerPrefKey = playerPrefKey;
    }

    public void MaxMoveAxis(string axisName)
    {
        MoveAxis(axisName, true);
    }

    public void MinMoveAxis(string axisName)
    {
        MoveAxis(axisName, false);
    }

    void MoveAxis(string axisName, bool isMaxMove)
    {
        Vector3 newValue = Vector3.zero; //PlayerPrefs_AM.GetVector3(_playerPrefKey);
        float step = 0; 

        if(_playerPrefKey == positionPanel.key)
        {
            newValue = positionPanel.GetVector3();
            step = isMaxMove ? positionMaxStep : positionMinStep;
        }
        else if(_playerPrefKey == rotationPanel.key)
        {
            newValue = rotationPanel.GetVector3();
            step = isMaxMove ? rotationMaxStep : rotationMinStep;
        }

        if (axisName == "x" || axisName == "X")
        {
            newValue.x += step;
        }
        if (axisName == "-x" || axisName == "-X")
        {
            newValue.x -= step;
        }
        else if (axisName == "y" || axisName == "Y")
        {
            newValue.y += step;
        }
        else if (axisName == "-y" || axisName == "-Y")
        {
            newValue.y -= step;
        }
        else if(axisName == "z" || axisName == "Z")
        {
            newValue.z += step;
        }
        else if (axisName == "-z" || axisName == "-Z")
        {
            newValue.z -= step;
        }

        if (_playerPrefKey == positionPanel.key)
        {
            positionPanel.SetVector3(newValue, true);
        }
        else if (_playerPrefKey == rotationPanel.key)
        {
            rotationPanel.SetVector3(newValue, true);
        }
    }
}

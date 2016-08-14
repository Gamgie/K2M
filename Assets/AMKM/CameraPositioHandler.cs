using UnityEngine;
using System.Collections;
using DG.Tweening;

public class CameraPositioHandler : MonoBehaviour {

    public enum CameraPositions
    {
        undef,
        front,
        side,
        up,
        custom
    }

    public GameObject frontObjectRef;
    public GameObject sideObjectRef;
    public GameObject upObjectRef;
    public GameObject customObjectRef;
    public float movementDuration;
    public float orthoSize = 5f;

    private Camera _mainCamera;
    private float _orthoSize;
    private CameraPositions _cameraPositionState = CameraPositions.undef;

    void Awake()
    {
        _mainCamera = GetComponent<Camera>();
        _orthoSize = PlayerPrefs_AM.GetFloat("orthoSize", 5.0f);
    }

    void Start()
    {
        MoveCameraTo(0);
    }

    public void MoveCameraTo(int positionIndex)
    {
        Vector3 positionTarget = Vector3.zero;
        Vector3 rotationTarget = Vector3.zero;

        _orthoSize = _mainCamera.orthographicSize;

        // save front position if we move camera 
        if(_cameraPositionState == CameraPositions.front && transform.position != frontObjectRef.transform.position)
        {
            PlayerPrefs_AM.SetVector3("Front View Position", transform.position);
            PlayerPrefs_AM.SetVector3("Front View Rotation", transform.rotation.eulerAngles);
            frontObjectRef.GetComponent<PlayerPrefsTransform>().Load();
            Debug.Log("Front view saved");
        }

        switch (positionIndex)
        {
            case 0:
                positionTarget = frontObjectRef.transform.position;
                rotationTarget = frontObjectRef.transform.rotation.eulerAngles;
                _mainCamera.orthographic = false;
                _cameraPositionState = CameraPositions.front;
                break;
            case 1:
                positionTarget = sideObjectRef.transform.position;
                rotationTarget = sideObjectRef.transform.rotation.eulerAngles;
                _mainCamera.orthographic = true;
                _cameraPositionState = CameraPositions.side;
                break;
            case 2:
                positionTarget = upObjectRef.transform.position;
                rotationTarget = upObjectRef.transform.rotation.eulerAngles;
                _mainCamera.orthographic = true;
                _cameraPositionState = CameraPositions.up;
                break;
            case 3:
                positionTarget = customObjectRef.transform.position;
                rotationTarget = customObjectRef.transform.rotation.eulerAngles;
                _mainCamera.orthographic = false;
                _cameraPositionState = CameraPositions.custom;
                break;
            default:
                break;
        }

        _mainCamera.transform.DOMove(positionTarget, movementDuration);
        _mainCamera.transform.DORotate(rotationTarget, movementDuration);
        _mainCamera.orthographicSize = _orthoSize;
    }

    void OnDestroy()
    {
        PlayerPrefs_AM.SetFloat("orthoSize", _orthoSize);

        // save front position
        if (_cameraPositionState == CameraPositions.front && transform.position != frontObjectRef.transform.position)
        {
            PlayerPrefs_AM.SetVector3("Front View Position", transform.position);
            PlayerPrefs_AM.SetVector3("Front View Rotation", transform.rotation.eulerAngles);
            Debug.Log("Front view saved");
        }
    }
}

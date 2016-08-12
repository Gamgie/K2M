using UnityEngine;
using System.Collections;
using DG.Tweening;

public class CameraPositioHandler : MonoBehaviour {

    public GameObject frontObjectRef;
    public GameObject sideObjectRef;
    public GameObject upObjectRef;
    public GameObject customObjectRef;
    public float movementDuration;
    public float orthoSize;

    private Camera _mainCamera;

    void Awake()
    {
        _mainCamera = GetComponent<Camera>();
    }

    public void MoveCameraTo(int positionIndex)
    {
        Vector3 positionTarget = Vector3.zero;
        Vector3 rotationTarget = Vector3.zero;

        _mainCamera.orthographicSize = orthoSize;

        switch (positionIndex)
        {
            case 0:
                positionTarget = frontObjectRef.transform.position;
                rotationTarget = frontObjectRef.transform.rotation.eulerAngles;
                _mainCamera.orthographic = false;
                break;
            case 1:
                positionTarget = sideObjectRef.transform.position;
                rotationTarget = sideObjectRef.transform.rotation.eulerAngles;
                _mainCamera.orthographic = true;
                break;
            case 2:
                positionTarget = upObjectRef.transform.position;
                rotationTarget = upObjectRef.transform.rotation.eulerAngles;
                _mainCamera.orthographic = true;
                break;
            case 3:
                positionTarget = customObjectRef.transform.position;
                rotationTarget = customObjectRef.transform.rotation.eulerAngles;
                _mainCamera.orthographic = false;
                break;
            default:
                break;
        }

        _mainCamera.transform.DOMove(positionTarget, movementDuration);
        _mainCamera.transform.DORotate(rotationTarget, movementDuration);
    }
}

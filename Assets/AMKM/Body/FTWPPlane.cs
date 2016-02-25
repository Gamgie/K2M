using UnityEngine;
using System.Collections;

public class FTWPPlane : MonoBehaviour {


    [Range(0, 1)]
    public float speed = .8f;

	// Use this for initialization
	void Start () {
        transform.rotation = Quaternion.Euler(0, -90, 90);
	}
	
	// Update is called once per frame
	void Update () {
        transform.localPosition = Vector3.Lerp(transform.localPosition,FTWPClient.planePosition,speed);
	}
}

using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class OffCenterProjection : MonoBehaviour {
	
	
	public float left = -0.2f;
	private float right  = 0.2f;
	public float top  = 0.2f;
	public float bottom = -0.2f;
	public float factor = 1.0f;

	public bool disable;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}
	
	void LateUpdate () {
		if (disable) {
			GetComponent<Camera>().ResetProjectionMatrix();
			enabled = false;
			disable =false;
			return;
		}

    Camera cam  = GetComponent<Camera>();
    cam.aspect = .6f;
    right = -left;
    Matrix4x4 m  = PerspectiveOffCenter(
        left/100, right/100, bottom/100, top/100,
        cam.nearClipPlane, cam.farClipPlane );
    cam.projectionMatrix = m;
}

	static Matrix4x4 PerspectiveOffCenter(
	   float left, float right,float bottom,float top,float near ,float far ) 
	{        
	    float x =  (float)(2.0 * near) / (right - left);
	    float y =  (float)(2.0 * near) / (top - bottom);
	    float a =  (float)(right + left) / (right - left);
	    float b =  (float)(top + bottom) / (top - bottom);
	    float c = (float)-(far + near) / (far - near);
	    float d = (float)-(2.0 * far * near) / (far - near);
	    float e = (float)-1.0;
	
	    Matrix4x4 m = new Matrix4x4();
	    m[0,0] = x;  m[0,1] = 0;  m[0,2] = a;  m[0,3] = 0;
	    m[1,0] = 0;  m[1,1] = y;  m[1,2] = b;  m[1,3] = 0;
	    m[2,0] = 0;  m[2,1] = 0;  m[2,2] = c;  m[2,3] = d;
	    m[3,0] = 0;  m[3,1] = 0;  m[3,2] = e;  m[3,3] = 0;
	    return m;
	}
}

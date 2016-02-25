using UnityEngine;
using System.Collections;
using SaveIt;


[ExecuteInEditMode]
public class OffCenterProjection : MonoBehaviour {
	
	
	public float left = -0.2f;
	private float right  = 0.2f;
	public float top  = 0.2f;
	public float bottom = -0.2f;
	public float factor = 1.0f;

    public bool loadConfigOnStart;
    public bool saveConfigOnExit;

    // Use this for initialization
    void Start () {
        if (loadConfigOnStart) loadConfig();
    }
	
	// Update is called once per frame
	void Update () {

	}
	
	void LateUpdate () {

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


    void OnDestroy()
    {
        if (saveConfigOnExit) saveConfig();
    }

    public void saveConfig()
    {
        SaveContext saveContext = SaveContext.ToFile("offCenterCamera");
        saveContext.Save<float>(left, "left");
        saveContext.Save<float>(top, "top");
        saveContext.Save<float>(bottom, "bottom");
        saveContext.Save<Vector3>(transform.position, "position");
        saveContext.Save<Quaternion>(transform.rotation, "rotation");
        saveContext.Flush();
    }

    public void loadConfig()
    {
        LoadContext loadContext = LoadContext.FromFile("offCenterCamera");
        left = loadContext.Load<float>("left");
        top = loadContext.Load<float>("top");
        bottom = loadContext.Load<float>("bottom");
        transform.position = loadContext.Load<Vector3>("position");
        transform.rotation = loadContext.Load<Quaternion>("rotation");
    }

    public void disableAndReset()
    {
        GetComponent<Camera>().ResetProjectionMatrix();
        enabled = false;
    }
}

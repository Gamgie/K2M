using UnityEngine;

[System.Serializable]
public class KPCL {

    [System.Serializable]
    public class Vector_3
    {
        public float x;
        public float y;
        public float z;
        public Vector_3(float x,float y,float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            
        }
      
    }

    public bool isFirst = false;

    public Vector_3[] points;

}



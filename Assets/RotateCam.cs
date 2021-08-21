using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCam : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float t=Time.deltaTime*10f;
        //transform.position=new Vector3(Mathf.Cos(t),0.0f,Mathf.Sin(t));
        transform.Rotate(0,t,0);
    }
}

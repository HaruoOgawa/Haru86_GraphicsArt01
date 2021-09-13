using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPUFlowers_CameraAnim : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.gameObject.transform.Rotate(0,1.0f*Time.deltaTime,0,Space.World);
    }
}

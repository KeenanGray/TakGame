using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngledCameraController : MonoBehaviour
{
    public float speed;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    transform.RotateAround(transform.parent.position,Vector3.up,speed*Input.GetAxis("Horizontal") * Time.deltaTime);
    }
}

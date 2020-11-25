using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngledCameraController : MonoBehaviour
{
    public float speed;
    
    Transform table;
    // Start is called before the first frame update
    void Start()
    {
        table=GameObject.Find ("GameBoard").transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(table.position,Vector3.up,speed*Input.GetAxis("Horizontal") * Time.deltaTime);
    }
}

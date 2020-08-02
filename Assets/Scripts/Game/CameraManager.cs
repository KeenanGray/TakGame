using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    Camera[] Cameras;
    int CurrentCam = 1;
    
    KeyCode ChangeCameraKey = KeyCode.Space;

    // Start is called before the first frame update
    void Start()
    {
        Cameras = GameObject.Find("Camera Container").GetComponentsInChildren<Camera>();

        Cameras[0].gameObject.SetActive(true);
        Cameras[1].gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(ChangeCameraKey))
        {
            Cameras[CurrentCam].gameObject.SetActive(false);
            switch (CurrentCam)
            {
                case 0:
                    CurrentCam = 1;
                    break;
                case 1:
                    CurrentCam = 0;
                    break;
            }
            Cameras[CurrentCam].gameObject.SetActive(true);
        }
    }
}

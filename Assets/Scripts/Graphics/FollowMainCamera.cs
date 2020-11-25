using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class FollowMainCamera : MonoBehaviour
{
    Transform mainCameraTransform;
    // Start is called before the first frame update
    void Awake()
    {
        mainCameraTransform = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (mainCameraTransform == null)
        {
            mainCameraTransform = Camera.main.transform;
            return;
        }

        transform.position = mainCameraTransform.position;
        transform.eulerAngles = mainCameraTransform.eulerAngles;
    }
}

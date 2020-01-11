using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : MonoBehaviour
{
    RaycastHit hit;

    [SerializeField]
    IntReference CurrentPlayer;

    [SerializeField]
    Material[] StoneMats = new Material[2];

    Material Selected;
    [HideInInspector]
    public Material Deselected;

    // Start is called before the first frame update
    void Start()
    {
        Selected = StoneMats[0];
        Deselected = StoneMats[1];
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 100.0f))
        {
            print("hitting " + hit.collider.name);
            if (hit.collider.gameObject == gameObject)
            {
                if (transform.CompareTag(CurrentPlayer.Value.ToString()))
                    GetComponentInChildren<MeshRenderer>().material = Selected;
            }
            else
            {
                GetComponentInChildren<MeshRenderer>().material = Deselected;
            }
        }
        else
        {
            GetComponentInChildren<MeshRenderer>().material = Deselected;
        }

    }
}

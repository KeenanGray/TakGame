using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakGameManager : MonoBehaviour
{
    RaycastHit hit;
    GameObject Selected;

    [SerializeField]
    IntReference CurrentPlayer;

    GameObject PlayerOnePool;
    GameObject PlayerTwoPool;


    // Start is called before the first frame update
    void Start()
    {
        PlayerOnePool = GameObject.Find("PlayerOnePool");
        PlayerTwoPool = GameObject.Find("PlayerTwoPool");
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 100.0f))
        {
            if (hit.collider.transform.parent.CompareTag("Space"))
            {
                Selected = hit.collider.transform.parent.gameObject;
            }
        }
        else
        {
            Selected = null;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (Selected != null)
            {
                MakeMove(CurrentPlayer.Value);
            }
            else
            {
                print("No selected");
            }
        }
    }

    void MakeMove(int p)
    {
        print("Making Move");
        //can't place on a square that already has a child
        //squares have a quad below them so we compare against 1
        if (Selected.transform.childCount != 1)
        {
            return;
        }

        if (p == 0)
        {
            var stone = PlayerOnePool.transform.GetChild(0);
            stone.SetParent(Selected.transform);

            stone.localEulerAngles = new Vector3(0, 0, 0);
            stone.localPosition = new Vector3(0, 0, 0);
            stone.tag = "0";

            CurrentPlayer.Value = 1;
        }

        else if (p == 1)
        {
            var stone = PlayerTwoPool.transform.GetChild(0);
            stone.SetParent(Selected.transform);

            stone.localEulerAngles = new Vector3(0, 0, 0);
            stone.localPosition = new Vector3(0, 0, 0);
            stone.tag = "1";

            CurrentPlayer.Value = 0;
        }

    }
}

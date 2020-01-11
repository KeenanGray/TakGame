using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ArrangeStones : MonoBehaviour
{
    int number = 10;
    bool needToAdd = true;

    GameObject StonesPool;
    public Material PlayerOneMat;
    public Material PlayerTwoMat;

    [Range(0.0001f, 0.05f)]
    public float offset = 0;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (StonesPool == null)
            StonesPool = GameObject.Find("Stones");

        needToAdd = true;

        if (Board.OnBoardSizeChanagedDelegate != null)
        {
            foreach (Delegate del in Board.OnBoardSizeChanagedDelegate.GetInvocationList())
            {
                if (del.Method.Name == "ArrangeTilesOnBoardSizeChange")
                {
                    needToAdd = false;
                }

                if (needToAdd)
                {
                    Board.OnBoardSizeChanagedDelegate += ArrangeTilesOnBoardSizeChange;
                    needToAdd = false;
                }
                else
                {

                }
            }

        }
    }

    void ArrangeTilesOnBoardSizeChange(int boardSize)
    {
        switch (boardSize)
        {
            case 3:
                number = 10;
                break;
            case 4:
                number = 15;
                break;
            case 5:
                number = 21;
                break;
            case 6:
                number = 30;
                break;
            case 7:
                number = 40;
                break;
            case 8:
                number = 50;
                break;
            default:
                Debug.LogError("this number does not make sense");
                break;
        }

        //put all the stones away in the object pool
        var pool = GameObject.Find("Stones").transform;

        var PlayerOnePool = GameObject.Find("PlayerOnePool").transform;
        var PlayerTwoPool = GameObject.Find("PlayerTwoPool").transform;

        foreach (Transform t in PlayerOnePool.transform.GetComponentsInChildren<Transform>())
        {
            if (t != PlayerOnePool.transform)
            {
                t.SetParent(pool);
                t.localPosition = new Vector3(0, 0, 0);
                t.localEulerAngles = new Vector3(0, 0, 0);
                t.tag = "Untagged";
            }
        }

        foreach (Transform t in PlayerTwoPool.transform.GetComponentsInChildren<Transform>())
        {
            if (t != PlayerTwoPool.transform)
            {
                t.SetParent(pool);
                t.localPosition = new Vector3(0, 0, 0);
            }
        }


        int i = 0;
        int Max = 30;

        var init = 0f;
        while (i < number)
        {
            var t1 = StonesPool.transform.GetChild(0).transform;
            t1.SetParent(PlayerOnePool);
            t1.GetComponent<MeshRenderer>().material = PlayerOneMat;
            t1.GetComponent<Stone>().Deselected = PlayerOneMat;


            //only move it if we have room in the tray
            if (i < Max / 2)
            {
                t1.localPosition = new Vector3(0, -init, 0f);
                init += offset;
            }
            else
            {
                t1.localPosition = new Vector3(0, 0, 0f);
            }

            t1.localEulerAngles = new Vector3(0, 0, 0);
            i++;
        }
        i = 0;
        while (i < number)
        {
            var t2 = StonesPool.transform.GetChild(1).transform;
            t2.SetParent(PlayerTwoPool);
            t2.GetComponent<MeshRenderer>().material = PlayerTwoMat;
            t2.GetComponent<Stone>().Deselected = PlayerTwoMat;


            //only move it if we have room in the tray
            if (i < Max / 2)
            {
                t2.localPosition = new Vector3(0, -init, 0f);
                init += offset;
            }
            else
            {
                t2.localPosition = new Vector3(0, 0, 0f);
            }

            t2.localEulerAngles = new Vector3(0, 0, 0);

            i++;
        }

    }
}

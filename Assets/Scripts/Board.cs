using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Board : MonoBehaviour
{
    public enum Size
    {
        three,
        four,
        five,
        six,
        seven,
        eight
    };
    [SerializeField]
    float offset = 0.042f;

    [SerializeField]
    Size s_BoardSize;
    int i_BoardSize;

    [SerializeField]
    IntReference iref_BoardSize;

    List<GameObject> Hidden;

    private Size lastSize;

    public delegate void OnBoardSizeChanged(int BoardSize);
    public static OnBoardSizeChanged OnBoardSizeChanagedDelegate;

    // Start is called before the first frame update
    void Start()
    {
        i_BoardSize = (int)s_BoardSize + 3;
        OnBoardSizeChanagedDelegate += UpdateBoard;

        offset = 0.042f;
        Hidden = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        i_BoardSize = (int)s_BoardSize + 3;
        iref_BoardSize.Value = i_BoardSize;

        if (OnBoardSizeChanagedDelegate == null)
            OnBoardSizeChanagedDelegate += UpdateBoard;

        if ((lastSize != s_BoardSize && OnBoardSizeChanagedDelegate != null))
        {
            lastSize = s_BoardSize;
            OnBoardSizeChanagedDelegate(i_BoardSize);
        }

    }

    void UpdateBoard(int size)
    {
        //move children onto "squares" object pool
        var pool = GameObject.Find("SquaresPool").transform;
        var board = GameObject.Find("BoardSelectors").transform;

        foreach (Transform t in board.transform.GetComponentsInChildren<Transform>())
        {
            if (t.CompareTag("Space"))
            {
                t.SetParent(pool);
                t.localPosition = new Vector3(0, 0, 0);
            }
        }
        int i = 0;

        //Add the children we need
        while (i < Mathf.Pow(size, 2))
        {
            i++;
            var t = pool.GetChild(0).transform;
            t.SetParent(board);
            t.transform.localPosition = new Vector3(0, 0, 0.0f);
        }

        //arrange the children baybee
        float x = 0;
        float y = 0;
        int k = 0;
        var numSqares = Mathf.Pow(size, 2);

        var width = -(size - 1) * offset;
        var height = -(size - 1) * offset;

        Vector3 StartPos = new Vector3(width / 2, 0, height / 2);

        for (int j = 0; j < numSqares; j++)
        {
            board.transform.GetChild(j).transform.localPosition = new Vector3(x, 0, y);

            if (j % (size) == 0 && j > 0)
            {
                y += offset;
                k = 0;
            }
            x = offset * k;
            k++;
            board.transform.GetChild(j).transform.localPosition = StartPos + new Vector3(x, 0, y);
        }

        //Hide larger objects for big game boards
        if (i_BoardSize > 6)
        {
            HideLargeObjects();
        }
        else
        {
            ShowLargeObjects();
        }
    }

    private void OnDestroy()
    {
        ShowLargeObjects();
    }

    void HideLargeObjects()
    {
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("HideIfLarge"))
        {
            Hidden.Add(go);
            go.SetActive(false);
        }

    }
    void ShowLargeObjects()
    {
        foreach (GameObject go in Hidden)
        {
            if (go != null)
                go.SetActive(true);
        }
    }
}


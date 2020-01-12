﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakGameManager : MonoBehaviour
{
    RaycastHit hit;
    GameObject Selected;

    [SerializeField]
    IntReference CurrentPlayer;
    [SerializeField]
    IntReference BoardSize;
    [SerializeField]
    GameObject board;
    [SerializeField]
    BoolReference Picking;

    GameObject PlayerOnePool;
    GameObject PlayerTwoPool;

    GameObject[] Picked;
    GameObject lastParent;

    int movesLeft;

    // Start is called before the first frame update
    void Start()
    {
        PlayerOnePool = GameObject.Find("PlayerOnePool");

        PlayerTwoPool = GameObject.Find("PlayerTwoPool");
        Picked = new GameObject[5];

        Picking.Value = false;
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
            if (hit.collider.transform.CompareTag("0") || hit.collider.transform.CompareTag("1"))
            {
                Selected = hit.collider.transform.gameObject;
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
                movesLeft = 0;
                for (int i = 0; i < Picked.Length; i++)
                {
                    if (Picked[i] != null)
                    {
                        if (lastParent != null)
                        {
                            Picked[i].transform.SetParent(lastParent.transform);
                            lastParent = null;
                        }
                        var yPos = 0f;
                        float stoneOffset = 0.006f;
                        yPos = (Picked[i].transform.parent.childCount - 2) * stoneOffset;
                        Picked[i].transform.localPosition = new Vector3(0, yPos, 0);
                        Picked[i].GetComponent<Highlights>().SetShouldRaycast(true);

                        Picked[i] = null;
                    }
                }
                EndPieceMove();
            }
        }
    }

    void MakeMove(int p)
    {
        if (Picking.Value)
        {
            //Here is what we do while stones are selected

            //if we pick the same stone, (Stone on the bottom) we reset the move
            //TODO:

            //If we pick a space or another stone, we move
            if (Selected.CompareTag("0") || Selected.CompareTag("1") || Selected.CompareTag("Space"))
            {
                Transform t;

                //we parent the stone to the "square space" that is there. in order to find this we have to identify whether we clicked on a space or a stone
                try
                {
                    t = Selected.GetComponent<Square>().transform;
                }
                catch
                {
                    t = Selected.GetComponentInParent<Square>().transform;
                }

                //Set the parent of the bottom piece
                var index = movesLeft - 1;
                if (Picked[index] != null)
                {
                    Picked[index].transform.SetParent(t);

                    //Put the stone on top of the other stone
                    var yPos = 0f;
                    float stoneOffset = 0.006f;
                    yPos = (Picked[index].transform.parent.childCount - 2) * stoneOffset;
                    Picked[index].transform.localPosition = new Vector3(0, yPos, 0);

                    movesLeft--;
                    print("Moves left " + movesLeft);


                }

                if (IsMoveOver())
                {
                    EndPieceMove();
                    ChangePlayer(p);
                }
                return;
            }
            return;
        }




        //check if we clicked a stone
        if ((Selected.CompareTag("0") || Selected.CompareTag("1")) && !Picking.Value)
        {
            var currentSqr = Selected.transform.parent;
            print(currentSqr);
            //for the stone and each stone above it in the stack - turn off highlights
            for (int stone = 1; stone < currentSqr.childCount; stone++)
            {
                currentSqr.GetChild(stone).GetComponent<Highlights>().SetShouldRaycast(false);
            }
            var cur = 0;
            for (int stone = Selected.transform.GetSiblingIndex(); stone < Selected.transform.parent.childCount; stone += 1)
            {
                Picking.Value = true;
                lastParent = Selected.gameObject;
                //There is no errory checking here that the differnece will be less than 5 but *Shrug*
                movesLeft++;
                Picked[cur] = Selected.transform.parent.GetChild(stone).gameObject;
                Picked[cur].transform.localPosition += new Vector3(0, .05f, 0);
                cur++;
            }
            print("picked " + Picked[0] + ", " + Picked[1] + ", " + Picked[2] + ", " + Picked[3] + ", " + Picked[4]);




            //Figure out board index and get spaces we can move to
            var self = Selected.transform.parent.GetSiblingIndex();

            //for each square in the board
            for (int i = 0; i < board.transform.childCount; i++)
            {
                //turn off the highlights on all squares but neighbor squares
                board.transform.GetChild(i).GetComponent<Highlights>().SetShouldRaycast(false);
                if (i == (self + 1) || i == (self - 1) || i == (self + BoardSize.Value) || i == (self - BoardSize.Value))
                {
                    var n = self;
                    for (int num = 0; num < 4; num++)
                    {
                        switch (num)
                        {
                            case 0:
                                n = self - 1;
                                break;
                            case 1:
                                n = self + 1;
                                break;
                            case 2:
                                n = self - BoardSize.Value;
                                break;
                            case 3:
                                n = self + BoardSize.Value;
                                break;
                        }
                        if (n < board.transform.childCount && n > 0)
                        {
                            var currentNeighbor = board.transform.GetChild(n);
                            if (currentNeighbor != null)
                            {
                                //exclude the neighboring squares that have pieces on top of them
                                if (currentNeighbor.childCount == 1)
                                    currentNeighbor.GetComponent<Highlights>().SetShouldRaycast(true);
                                else
                                {
                                    //set each sibling of this object to raycast instead
                                    for (int sibling = 1; sibling < currentNeighbor.childCount; sibling++)
                                    {
                                        currentNeighbor.GetChild(sibling).GetComponent<Highlights>().SetShouldRaycast(true);
                                    }
                                }
                            }
                        }
                    }

                }
                else
                {

                }
            }
            return;
        }




        //we are placing on a selected space
        if (Selected.CompareTag("Space") && !Picking.Value)
        {
            //can't place on a square that already has a child
            //squares have a quad below them so we compare against 1
            if (Selected.transform.childCount != 1)
            {
                return;
            }
            if (p == 0)
            {
                PlaceOnTile(PlayerOnePool, 0);
            }
            else if (p == 1)
            {
                PlaceOnTile(PlayerTwoPool, 1);
            }
            ChangePlayer(p);
            return;
        }

    }

    void ChangePlayer(int p)
    {
        if (p == 0)
        {
            CurrentPlayer.Value = 1;
        }
        else if (p == 1)
        {
            CurrentPlayer.Value = 0;
        }

        //When we change the player reset what can and can't raycast
        for (int i = 0; i < board.transform.childCount; i++)
        {
            //for every square in the board
            //if the top is the square - allow raycast to that quad
            if (board.transform.childCount == 1)
                board.GetComponentInChildren<Highlights>().SetShouldRaycast(true);
            //check the top piece
            var currentSqr = board.transform.GetChild(i);
            var topPiece = currentSqr.transform.GetChild(currentSqr.transform.childCount - 1);

            if (!topPiece.CompareTag(CurrentPlayer.Value.ToString()))
            {
                //if the top is the player - allow raycast(to all siblings, but not the quad)
                for (int sibling = 1; sibling < currentSqr.transform.childCount; sibling++)
                {
                    currentSqr.GetChild(sibling).GetComponent<Highlights>().SetShouldRaycast(false);
                }
            }
            //else do not allow raycast
            if (topPiece.CompareTag(CurrentPlayer.Value.ToString()))
            {
                for (int sibling = 1; sibling < currentSqr.transform.childCount; sibling++)
                {
                    currentSqr.GetChild(sibling).GetComponent<Highlights>().SetShouldRaycast(true);
                }
            }
        }
    }


    void PlaceOnTile(GameObject source, int p)
    {
        if (source.transform.childCount < 0) //when we are out of pieces, we can't place on tiles anymore
            return;

        var stone = source.transform.GetChild(0);
        stone.SetParent(Selected.transform);

        stone.localEulerAngles = new Vector3(0, 0, 0);
        stone.localPosition = new Vector3(0, 0, 0);
        stone.tag = p.ToString();

        ChangePlayer(p);
    }

    void EndPieceMove()
    {
        Picking.Value = false;
        for (int i = 0; i < board.transform.childCount; i++)
        {
            board.transform.GetChild(i).GetComponent<Highlights>().SetShouldRaycast(true);
        }
        for (int i = 0; i < Picked.Length; i++)
            Picked[i] = null;//set the moved stone to null since we no longer have it selected.

    }

    private void OnApplicationQuit()
    {
    }

    bool IsMoveOver()
    {

        return (movesLeft == 0); //down here all picked objects are null so turn is done
    }
}


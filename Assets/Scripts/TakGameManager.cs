using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tak
{
    public class TakGameManager : MonoBehaviour
    {
        RaycastHit hit;
        GameObject Selected;


        [SerializeField]
        IntReference BoardSize;
        [SerializeField]
        GameObject board;
        [SerializeField]
        BoolReference Picking;

        [SerializeField]
        IntReference CurrentPlayer;

        GameObject[] Picked;
        GameObject LiftedStone;

        int movesLeft;

        public static LayerMask inputMask;

        public enum Direction
        {
            North,
            East,
            West,
            South
        }

        // Start is called before the first frame update
        void Start()
        {

            Picked = new GameObject[5];

            Picking.Value = false;

            inputMask = (1) | (1 << 4) | (1 << 5) | (1 << 8) | (1 << 10);
            //        print(Convert.ToString(inputMask.value, 2));
        }

        // Update is called once per frame
        void Update()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 100.0f, inputMask))
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
                    UndoMove();
                }
            }
        }

        int StackIndex = 0;
        void MakeMove(int p)
        {
            if (Picking.Value)
            {
                //Here is what we do while stones are selected
                Direction dir;

                //If we pick a space or another stone, we move
                if (Selected.CompareTag("0") || Selected.CompareTag("1") || Selected.CompareTag("Space"))
                {

                    dir = CalculateDir(LiftedStone, Selected);
                    SetTargetSquares(Selected, dir);


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
                    if (Picked[StackIndex] != null)
                    {
                        //Put the stone on top of the other stone
                        Picked[StackIndex].transform.SetParent(t);
                        var yPos = 0f;
                        float stoneOffset = 0.006f;
                        yPos = (Picked[StackIndex].transform.parent.childCount - 2) * stoneOffset;
                        Picked[StackIndex].transform.localPosition = new Vector3(0, yPos, 0);
                        Picked[StackIndex].GetComponent<Highlights>().SetShouldRaycast(true);
                        StackIndex++;
                        movesLeft--;
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
                //for the stone and each stone above it in the stack - turn off highlights
                for (int stone = 1; stone < currentSqr.childCount; stone++)
                {
                    currentSqr.GetChild(stone).GetComponent<Highlights>().SetShouldRaycast(false);
                }
                var cur = 0;
                for (int stone = Selected.transform.GetSiblingIndex(); stone < Selected.transform.parent.childCount; stone += 1)
                {

                    Picking.Value = true;
                    LiftedStone = Selected.transform.parent.gameObject;
                    //There is no errory checking here that the differnece will be less than 5 but *Shrug*
                    movesLeft++;
                    Picked[cur] = Selected.transform.parent.GetChild(stone).gameObject;
                    Picked[cur].transform.localPosition += new Vector3(0, .05f, 0);
                    cur++;
                }

                //Figure out board index and get spaces we can move to
                var self = Selected.transform.parent.GetSiblingIndex();

                //for each square in the board
                for (int i = 0; i < board.transform.childCount; i++)
                {
                    //turn off the highlights on all squares
                    //except the squares we can place on (neighbors).
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
                    //PlaceOnTile(PlayerOnePool, 0);
                }
                else if (p == 1)
                {
                    //   PlaceOnTile(PlayerTwoPool, 1);
                }
                ChangePlayer(p);
                return;
            }

        }

        private Direction CalculateDir(object liftedSquare, GameObject selected)
        {
            print("A " + LiftedStone.transform.GetSiblingIndex() + ", B " + selected.transform.GetSiblingIndex());
            return Direction.North;
        }

        void UndoMove()
        {
            if (LiftedStone == null || !Picking.Value)
                return;

            //put back any pieces we may have moved
            for (int i = 0; i < Picked.Length; i++)
            {
                if (Picked[i] == null)
                    continue;

                Picked[i].transform.SetParent(LiftedStone.transform);
                var target = Picked[i].transform.parent.childCount + i;
                Picked[i].transform.SetSiblingIndex(target);

                LiftedStone.GetComponent<Square>().SendMessage("RedoHeights");
            }
            movesLeft = 0;
            EndPieceMove();
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
            ResetRaycasts();
        }

        void ResetRaycasts()
        {
            //When we change the player reset what can and can't raycast
            for (int i = 0; i < board.transform.childCount; i++)
            {
                //for every square in the board
                //if the top is the square - allow raycast to that quad
                if (board.transform.GetChild(i).transform.childCount == 1)
                    board.transform.GetChild(i).GetComponentInChildren<Highlights>().SetShouldRaycast(true);
                else
                    board.transform.GetChild(i).GetComponentInChildren<Highlights>().SetShouldRaycast(false);

                //check the top piece
                var currentSqr = board.transform.GetChild(i);
                var topPiece = currentSqr.transform.GetChild(currentSqr.transform.childCount - 1);

                //else do not allow raycast on objects in stack
                if (!topPiece.CompareTag(CurrentPlayer.Value.ToString()))
                {
                    for (int sibling = 1; sibling < currentSqr.transform.childCount; sibling++)
                    {
                        currentSqr.GetChild(sibling).GetComponent<Highlights>().SetShouldRaycast(false);
                    }
                }
                //if the top is the player - allow raycast(to all siblings, but not the quad), down to five
                if (topPiece.CompareTag(CurrentPlayer.Value.ToString()))
                {
                    var target = Mathf.Max(currentSqr.transform.childCount - 6, 0);
                    for (int sibling = currentSqr.transform.childCount - 1; sibling > target; sibling--)
                    {
                        if (currentSqr.GetChild(sibling) != null)
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
            ResetRaycasts();
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
            StackIndex = 0;
            ResetRaycasts();
        }

        void SetTargetSquares(GameObject square, Direction dir)
        {

        }

        private void OnApplicationQuit()
        {
        }

        bool IsMoveOver()
        {

            return (movesLeft == 0); //down here all picked objects are null so turn is done
        }
    }
}
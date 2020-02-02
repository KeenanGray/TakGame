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
        GameObject board;

        [SerializeField]
        BoolReference Picking = null;

        [SerializeField]
        IntReference CurrentPlayer = null;

        GameObject[] Picked;
        GameObject LiftedFromSquare;
        Transform ogSquare;

        int movesLeft;

        public static LayerMask inputMask;
        Direction lastDir;

        public enum Direction
        {
            None,
            North,
            East,
            West,
            South
        }

        // Start is called before the first frame update
        void Start()
        {
            lastDir = Direction.None;
            Picked = new GameObject[5];

            Picking.Value = false;

            inputMask = (1) | (1 << 4) | (1 << 5) | (1 << 8) | (1 << 10);
            //        print(Convert.ToString(inputMask.value, 2));
            board = GameObject.Find("GameBoard");

            ResetRaycasts();
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
            var boardSpaces = board.transform.Find("BoardSpaces");

            if (Picking.Value)
            {
                //Here is what we do while stones are selected
                Direction dir;

                //If we pick a space or another stone, we move
                if (Selected.CompareTag("0") || Selected.CompareTag("1") || Selected.CompareTag("Space"))
                {
                    dir = CalculateDir(LiftedFromSquare, Selected);
                    lastDir = dir;
                    SetTargetSquares(Selected, dir);
                    LiftedFromSquare = Selected;

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
                    LiftedFromSquare = Selected.transform.parent.gameObject;
                    ogSquare = Selected.transform.parent;

                    //There is no errory checking here that the differnece will be less than 5 but *Shrug*
                    movesLeft++;
                    if (stone < Selected.transform.parent.childCount && stone > 0)
                    {
                        Picked[cur] = Selected.transform.parent.GetChild(stone).gameObject;
                        Picked[cur].transform.localPosition += new Vector3(0, .05f, 0);
                        cur++;
                    }
                }

                //Figure out board index and get spaces we can move to
                var self = Selected.transform.parent.GetSiblingIndex();

                var size = board.GetComponent<TakBoard>().GetBoardSizeProperty().getSize();

                //for each square in the board
                for (int i = 0; i < boardSpaces.transform.childCount; i++)
                {
                    //turn off the highlights on all squares
                    //except the squares we can place on (neighbors).
                    boardSpaces.transform.GetChild(i).GetComponent<Highlights>().SetShouldRaycast(false);
                    if (i == (self + 1) || i == (self - 1) || i == (self + size) || i == (self - size))
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
                                    n = self - size;
                                    break;
                                case 3:
                                    n = self + size;
                                    break;
                            }
                            if (n < boardSpaces.transform.childCount && n > 0)
                            {
                                var currentNeighbor = boardSpaces.transform.GetChild(n);
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
                    PlaceOnTile(board.GetComponent<TakBoard>().GetPiece(), 0);
                }
                else if (p == 1)
                {
                    PlaceOnTile(board.GetComponent<TakBoard>().GetPiece(), 1);
                }
                ChangePlayer(p);
                return;
            }

        }

        private Direction CalculateDir(GameObject liftedSquare, GameObject selected)
        {
            Transform t = selected.transform;
            var boardWidth = board.GetComponent<TakBoard>().GetBoardSizeProperty().getSize();

            if (!selected.CompareTag("Space"))
                t = selected.transform.parent;

            LiftedFromSquare = t.gameObject;

            var diff = liftedSquare.transform.GetSiblingIndex() - t.GetSiblingIndex();

            if (diff == -boardWidth)
            {
                return Direction.North;
            }
            if (diff == -1)
            {
                return Direction.East;
            }
            if (diff == boardWidth)
            {
                return Direction.South;
            }
            if (diff == 1)
            {
                return Direction.West;
            }
            //  Debug.LogError("Problem here");
            if (lastDir != Direction.None)
                return lastDir;
            else
            {
                Debug.LogError("Problem");
                return Direction.None;
            }
        }

        void UndoMove()
        {
            if (LiftedFromSquare == null || !Picking.Value)
                return;

            //put back any pieces we may have moved
            for (int i = 0; i < Picked.Length; i++)
            {
                if (Picked[i] == null)
                    continue;

                Picked[i].transform.SetParent(ogSquare);
                var target = Picked[i].transform.parent.childCount + i;
                Picked[i].transform.SetSiblingIndex(target);

                ogSquare.GetComponent<Square>().SendMessage("RedoHeights");
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
            var boardSpaces = board.transform.Find("BoardSpaces");
            //When we change the player reset what can and can't raycast
            for (int i = 0; i < boardSpaces.transform.childCount; i++)
            {
                //for every square in the board
                //if the top is the square - allow raycast to that quad
                if (boardSpaces.transform.GetChild(i).childCount == 1)
                    boardSpaces.transform.GetChild(i).GetComponentInChildren<Highlights>().SetShouldRaycast(true);
                else
                    boardSpaces.transform.GetChild(i).GetComponentInChildren<Highlights>().SetShouldRaycast(false);

                //check the top piece
                var curSquare = boardSpaces.transform.GetChild(i);

                if (curSquare.transform.childCount == 1)
                    continue;

                var topStone = curSquare.transform.GetChild(curSquare.transform.childCount - 1);

                //else do not allow raycast on objects in stack
                if (!topStone.CompareTag(CurrentPlayer.Value.ToString()))
                {
                    for (int sibling = 1; sibling < curSquare.transform.childCount; sibling++)
                    {
                        curSquare.GetChild(sibling).GetComponent<Highlights>().SetShouldRaycast(false);
                    }
                }

                if (Picking.Value)
                {
                    Debug.Log("PICKING");
                }
                else
                {
                    //if the top is the player - allow raycast(to all siblings, but not the quad), down to five
                    if (topStone.CompareTag(CurrentPlayer.Value.ToString()))
                    {
                        var target = Mathf.Max(curSquare.transform.childCount - 6, 0);
                        for (int sibling = curSquare.transform.childCount - 1; sibling > target; sibling--)
                        {
                            if (curSquare.GetChild(sibling) != null)
                            {
                                curSquare.GetChild(sibling).GetComponent<Highlights>().SetShouldRaycast(true);
                            }
                        }
                    }
                }
            }
        }

        void PlaceOnTile(GameObject stone, int p)
        {
            stone.transform.SetParent(Selected.transform);

            stone.transform.localEulerAngles = new Vector3(0, 0, 0);
            stone.transform.localPosition = new Vector3(0, 0, 0);
            stone.transform.tag = p.ToString();

            ChangePlayer(p);
        }

        void EndPieceMove()
        {
            var boardSpaces = board.transform.Find("BoardSpaces");

            Picking.Value = false;
            for (int i = 0; i < board.transform.childCount; i++)
            {
                boardSpaces.transform.GetChild(i).GetComponent<Highlights>().SetShouldRaycast(true);
            }
            for (int i = 0; i < Picked.Length; i++)
                Picked[i] = null;//set the moved stone to null since we no longer have it selected.
            StackIndex = 0;
            ResetRaycasts();
        }

        void SetTargetSquares(GameObject selected, Direction dir)
        {
            Transform square = selected.transform;

            if (!selected.CompareTag("Space"))
                square = selected.transform.parent;

            var boardWidth = board.GetComponent<TakBoard>().GetBoardSizeProperty().getSize();

            //turn off other squares

            foreach (Highlights h in GameObject.FindObjectsOfType<Highlights>())
            {
                h.SetShouldRaycast(false);
            }

            //get each neighbor square and set them raycastable
            var parent = square.parent;
            var index = square.GetSiblingIndex();

            int nextChild = -1;
            int prevChild = -1;
            switch (dir)
            {
                case Direction.North:
                    nextChild = index + boardWidth;
                    prevChild = index - boardWidth;
                    break;
                case Direction.East:
                    nextChild = index + 1;
                    prevChild = index - 1;

                    break;
                case Direction.South:
                    nextChild = index - boardWidth;
                    prevChild = index + boardWidth;
                    break;
                case Direction.West:
                    nextChild = index - 1;
                    prevChild = index + 1;
                    break;
            }

            if (nextChild >= 0 && nextChild < parent.childCount)
            {
                // parent.GetChild(nextChild).GetChild(childCount - 1).GetComponent<Highlights>().SetShouldRaycast(true);

                foreach (Highlights h in parent.GetChild(nextChild).GetComponentsInChildren<Highlights>())
                {
                    parent.GetChild(nextChild).GetComponent<Highlights>().SetShouldRaycast(true);
                    h.SetShouldRaycast(true);
                }
            }
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
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Tak
{
    public class TakGameManager : MonoBehaviour
    {
        RaycastHit hit;
        GameObject Selected;

        [SerializeField]
        GameObject board;

        public Palette palette;

        delegate void TilePickedUpEvent();
        TilePickedUpEvent onTilePickedUp = null;

        delegate void TurnFinished();
        TurnFinished onTurnFinished = null;

        [SerializeField]
        TurnData TurnData = null;

        GameObject LiftedFromSquare;
        Transform ogSquare;

        int movesLeft;

        public static LayerMask inputMask;
        Direction lastDir;

        public TextMeshProUGUI winText;

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
            TurnData.Init();
            TurnData.Picking.Value = false;

            inputMask = (1) | (1 << 4) | (1 << 5) | (1 << 8) | (1 << 10);
            board = GameObject.Find("GameBoard");

            ResetRaycasts();

            TurnData.CurrentPlayer.Value = 0;

            onTurnFinished += () =>
            {
                TurnData.PlaceType = Stonetype.FlatStone;
            };

            onTurnFinished += CheckForWinner;
        }

        // Update is called once per frame
        void Update()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Input.GetMouseButtonDown(0))
            {
                if (Selected != null)
                {
                    MakeMove(TurnData.CurrentPlayer.Value);
                }
                else
                {
                    UndoMove();
                }
            }

            if (Physics.Raycast(ray, out hit, 10.0f, inputMask))
            {
                if (hit.Equals(null) || hit.collider.transform.parent == null)
                {
                    Selected = null;
                    return;
                }

                if (hit.collider.CompareTag("Untagged"))
                {
                    Selected = null;
                    return;
                }

                //Checks if the collider is a "space"
                if (hit.collider.transform.parent.CompareTag("Space"))
                {
                    Selected = hit.collider.transform.parent.gameObject;
                }
                //or if the collider is a stone belonging to a player
                if (hit.collider.transform.CompareTag("0") || hit.collider.transform.CompareTag("1"))
                {
                    Selected = hit.collider.transform.parent.gameObject;
                }
            }
            else
            {
                Selected = null;
            }

        }

        int StackIndex = 0;
        void MakeMove(int p)
        {
            var boardSpaces = board.transform.Find("BoardSpaces");

            if (TurnData.Picking.Value)
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
                    if (TurnData.Picked[StackIndex] != null)
                    {
                        //Put the stone on top of the other stone
                        TurnData.Picked[StackIndex].transform.SetParent(t);
                        var yPos = 0f;
                        float stoneOffset = 0.006f;
                        yPos = (TurnData.Picked[StackIndex].transform.parent.childCount - 2) * stoneOffset;
                        TurnData.Picked[StackIndex].transform.localPosition = new Vector3(0, yPos, 0);
                        TurnData.Picked[StackIndex].GetComponent<Highlights>().SetShouldRaycast(true);
                        StackIndex++;
                        movesLeft--;
                    }

                    if (IsMoveOver())
                    {
                        EndPieceMove();
                        ChangePlayer(p);
                        onTurnFinished.Invoke();

                    }
                    return;
                }
                return;
            }

            //check if we clicked a stone
            if ((Selected.CompareTag("0") || Selected.CompareTag("1")) && !TurnData.Picking.Value)
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
                    //Set that we are moving a piece
                    TurnData.Picking.Value = true;

                    if (onTilePickedUp != null)
                        onTilePickedUp();

                    LiftedFromSquare = Selected.transform.parent.gameObject;
                    ogSquare = Selected.transform.parent;

                    //Pick up all the stones in the stack.
                    movesLeft++;
                    if (stone < Selected.transform.parent.childCount && stone > 0)
                    {
                        TurnData.Picked[cur] = Selected.transform.parent.GetChild(stone).gameObject;
                        TurnData.Picked[cur].transform.localPosition += new Vector3(0, .05f, 0);
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
                    boardSpaces.transform.GetChild(i).GetComponent<Highlights>().SetShouldRaycast(false);

                    // + Children
                    foreach (Highlights hl in boardSpaces.transform.GetChild(i).GetComponentsInChildren<Highlights>())
                    {
                        hl.SetShouldRaycast(false);
                    }

                    //Go in and turn on the squares we
                    //can place on (neighbors).
                    int n1 = self + 1;
                    int n2 = self - 1;
                    int n3 = self + size;
                    int n4 = self - size;
                    //A literal edge case!
                    if (self % size == 0)
                        n2 = 0;

                    if (i == n1 || i == n2 || i == n3 || i == n4)
                    {
                        var n = self;
                        for (int num = 0; num < 4; num++)
                        {
                            switch (num)
                            {
                                case 0:
                                    n = n1;
                                    break;
                                case 1:
                                    n = n2;
                                    break;
                                case 2:
                                    n = n3;
                                    break;
                                case 3:
                                    n = n4;
                                    break;
                            }
                            if (n < boardSpaces.transform.childCount && n >= 0)
                            {
                                var currentNeighbor = boardSpaces.transform.GetChild(n);
                                if (currentNeighbor != null)
                                {
                                    //exclude the neighboring squares that have pieces on top of them
                                    if (currentNeighbor.childCount == 1)
                                        currentNeighbor.GetComponent<Highlights>().SetShouldRaycast(true);
                                    else
                                    {
                                        //set just the top stone to raycast
                                        for (int sibling = 1; sibling < currentNeighbor.childCount; sibling++)
                                        {
                                            //check if the top piece is a flatstone, otherwise we can't place here.
                                            try
                                            {
                                                //child (childcount-1) is the top stone
                                                if (currentNeighbor.GetChild(currentNeighbor.childCount - 1).GetComponent<Stone>().Stonetype == Stonetype.FlatStone
                                                || (currentNeighbor.GetChild(currentNeighbor.childCount - 1).GetComponent<Stone>().Stonetype == Stonetype.StandingStone
                                                && Selected.GetComponent<Stone>().Stonetype == Stonetype.CapStone && Selected.transform.GetComponentsInChildren<Stone>().Length == 1))
                                                {
                                                    //long if statement above checks if neighbors are flatstones, that a neighbor is a standing stone and we have selected a capstone
                                                    currentNeighbor.GetChild(currentNeighbor.childCount - 1).GetComponentInChildren<Highlights>().SetShouldRaycast(true);
                                                }
                                            }
                                            catch (Exception e)
                                            {
                                                if (e.GetType() == typeof(NullReferenceException))
                                                {
                                                }
                                            }
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
            if (Selected.CompareTag("Space") && !TurnData.Picking.Value)
            {
                //Set the piece type
                Stonetype t = TurnData.PlaceType;

                if (t == Stonetype.CapStone)
                {
                    //deduct capstone supply from the player
                    TurnData.CapStones[TurnData.CurrentPlayer.Value] = 0;
                }
                //can't place on a square that already has a child
                //squares have a quad below them so we compare against index 1
                if (Selected.transform.childCount != 1)
                {
                    return;
                }
                if (p == 0)
                {
                    PlaceOnTile(board.GetComponent<TakBoard>().GetPiece(), 0, t);
                }
                else if (p == 1)
                {
                    PlaceOnTile(board.GetComponent<TakBoard>().GetPiece(), 1, t);
                }
                ChangePlayer(p);
                onTurnFinished.Invoke();


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
            if (LiftedFromSquare == null || !TurnData.Picking.Value)
                return;

            //put back any pieces we may have moved
            for (int i = 0; i < TurnData.Picked.Length; i++)
            {
                if (TurnData.Picked[i] == null)
                    continue;

                TurnData.Picked[i].transform.SetParent(ogSquare);
                var target = TurnData.Picked[i].transform.parent.childCount + i;
                TurnData.Picked[i].transform.SetSiblingIndex(target);

                ogSquare.GetComponent<Square>().SendMessage("RedoHeights");
            }
            movesLeft = 0;
            EndPieceMove();
        }

        void ChangePlayer(int p)
        {
            if (p == 0)
                TurnData.CurrentPlayer.Value = 1;
            else if (p == 1)
                TurnData.CurrentPlayer.Value = 0;

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
                if (!topStone.CompareTag(TurnData.CurrentPlayer.Value.ToString()))
                {
                    for (int sibling = 1; sibling < curSquare.transform.childCount; sibling++)
                    {
                        curSquare.GetChild(sibling).GetComponentInChildren<Highlights>().SetShouldRaycast(false);
                    }
                }

                if (TurnData.Picking.Value)
                {
                    Debug.Log("PICKING");
                }
                else
                {
                    //if the top is the player - allow raycast(to all siblings, but not the quad), down to five
                    if (topStone.CompareTag(TurnData.CurrentPlayer.Value.ToString()))
                    {
                        var target = Mathf.Max(curSquare.transform.childCount - 6, 0);
                        for (int sibling = curSquare.transform.childCount - 1; sibling > target; sibling--)
                        {
                            if (curSquare.GetChild(sibling) != null)
                            {
                                curSquare.GetChild(sibling).GetComponentInChildren<Highlights>().SetShouldRaycast(true);
                            }
                        }
                    }
                }
            }
        }

        void PlaceOnTile(GameObject stone, int p, Stonetype type)
        {
            stone.transform.tag = p.ToString();
            stone.transform.SetParent(Selected.transform);
            stone.GetComponentInChildren<Stone>().Stonetype = type;

            stone.transform.localEulerAngles = new Vector3(0, 0, 0);

            stone.transform.localPosition = new Vector3(0, 0, 0);
            stone.transform.GetChild(0).tag = p.ToString();
        }

        void EndPieceMove()
        {
            var boardSpaces = board.transform.Find("BoardSpaces");

            TurnData.Picking.Value = false;
            for (int i = 0; i < board.transform.childCount; i++)
            {
                boardSpaces.transform.GetChild(i).GetComponent<Highlights>().SetShouldRaycast(true);
            }
            for (int i = 0; i < TurnData.Picked.Length; i++)
                TurnData.Picked[i] = null;//set the moved stone to null since we no longer have it selected.
            StackIndex = 0;

            //change the selected piece back to flatstone for convenience
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
            var prnt = square.parent;
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

            if (nextChild >= 0 && nextChild < prnt.childCount)
            {
                foreach (Highlights h in prnt.GetChild(nextChild).GetComponentsInChildren<Highlights>())
                {
                    prnt.GetChild(nextChild).GetComponent<Highlights>().SetShouldRaycast(true);
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



        void CheckForWinner()
        {
            winText.text = "Connected";
            winText.enabled = false;

            TakBoard takBoard = board.GetComponent<TakBoard>();
            foreach (GameObject s in takBoard.TopEdge)
            {
                //see if square connect to bottom edge through recursion
                if (SolveRoad(s, -1, takBoard.TopEdge, takBoard.BottomEdge))
                {
                    winText.enabled = true;
                    break;
                }
            }

            foreach (GameObject s in takBoard.LeftEdge)
            {
                //see if square connects to bottom edge through recursion
                if (SolveRoad(s, -1, takBoard.LeftEdge, takBoard.RightEdge))
                {
                    winText.enabled = true;
                    break;
                }
            }
        }

        //source is the integer of the square we are coming from
        //-1 on first call
        List<int> Checked;
        bool SolveRoad(GameObject edgeSquare, int source, GameObject[] edge1, GameObject[] edge2)
        {
            bool returnval = false;

            int size = board.GetComponent<TakBoard>().BoardSize.getSize();
            int index = edgeSquare.transform.GetSiblingIndex();

            if (source == -1)
                Checked = new List<int>();
            else
            {
                if (Checked.Contains(index))
                {
                    print(index + " Checked already");
                    returnval = false;
                }

            }
            Checked.Add(index);

            if (IsSquareOnEdge(index, edge2))
                return true;

            Square s = board.transform.GetChild(0).GetChild(index).GetComponent<Square>();

            //no need to check the space if no stone is
            if (s.transform.childCount < 2)
                return false; //no piece on the square, return here

            Stone TopStone = s.transform.GetChild(s.transform.childCount - 1).GetComponent<Stone>();

            int index1 = index + 1;
            int index2 = index - 1;
            int index3 = index + size;
            int index4 = index - size;
            //A literal edge case!
            if (index % size == 0)
                index2 = 0;

            int totalSpaces = board.transform.GetChild(0).childCount;

            //for each neighbor
            Square neighborSquare = null;
            Stone neighborStone = null;
            if (index1 > 0 && index1 < totalSpaces && !IsSquareOnEdge(index1, edge1) && index1 != source && !Checked.Contains(index1))
            {
                neighborSquare = board.transform.GetChild(0).GetChild(index1).GetComponent<Square>();

                if (neighborSquare.transform.childCount > 1)
                {
                    neighborStone = neighborSquare.transform.GetChild(neighborSquare.transform.childCount - 1).GetComponent<Stone>();
                    if (neighborStone.CompareTag(TopStone.tag) && neighborStone.Stonetype != Stonetype.StandingStone)
                    {
                        returnval = IsSquareOnEdge(index1, edge2);
                        if (returnval)
                            return true;
                        else
                            returnval = SolveRoad(board.transform.GetChild(0).GetChild(index1).gameObject, index, edge1, edge2);
                        if (returnval)
                            return true;
                    }
                }
            }

            if (index2 > 0 && index2 < totalSpaces && !IsSquareOnEdge(index2, edge1) && index2 != source && !Checked.Contains(index2))
            {
                neighborSquare = board.transform.GetChild(0).GetChild(index2).GetComponent<Square>();

                if (neighborSquare.transform.childCount > 1)
                {
                    neighborStone = neighborSquare.transform.GetChild(neighborSquare.transform.childCount - 1).GetComponent<Stone>();
                    if (neighborStone.CompareTag(TopStone.tag) && neighborStone.Stonetype != Stonetype.StandingStone)
                    {
                        returnval = IsSquareOnEdge(index2, edge2);
                        if (returnval)
                            return true;
                        else
                            returnval = SolveRoad(board.transform.GetChild(0).GetChild(index2).gameObject, index, edge1, edge2);
                        if (returnval)
                            return true;
                    }
                }
            }

            if (index3 > 0 && index3 < totalSpaces && !IsSquareOnEdge(index3, edge1) && index3 != source && !Checked.Contains(index3))
            {
                neighborSquare = board.transform.GetChild(0).GetChild(index3).GetComponent<Square>();

                if (neighborSquare.transform.childCount > 1)
                {
                    neighborStone = neighborSquare.transform.GetChild(neighborSquare.transform.childCount - 1).GetComponent<Stone>();
                    if (neighborStone.CompareTag(TopStone.tag) && neighborStone.Stonetype != Stonetype.StandingStone)
                    {
                        returnval = IsSquareOnEdge(index3, edge2);
                        if (returnval)
                            return true;
                        else
                            returnval = SolveRoad(board.transform.GetChild(0).GetChild(index3).gameObject, index, edge1, edge2);
                        if (returnval)
                            return true;
                    }
                }
            }

            if (index4 > 0 && index4 < totalSpaces && !IsSquareOnEdge(index4, edge1) && index4 != source && !Checked.Contains(index4))
            {
                neighborSquare = board.transform.GetChild(0).GetChild(index4).GetComponent<Square>();

                if (neighborSquare.transform.childCount > 1)
                {
                    neighborStone = neighborSquare.transform.GetChild(neighborSquare.transform.childCount - 1).GetComponent<Stone>();
                    if (neighborStone.CompareTag(TopStone.tag) && neighborStone.Stonetype != Stonetype.StandingStone)
                    {
                        returnval = IsSquareOnEdge(index4, edge2);
                        if (returnval)
                            return true;
                        else
                            returnval = SolveRoad(board.transform.GetChild(0).GetChild(index4).gameObject, index, edge1, edge2);
                        if (returnval)
                            return true;
                    }
                }
            }

            return returnval;
        }

        bool IsSquareOnEdge(int neighborIndex, GameObject[] Edge)
        {
            foreach (GameObject go in Edge)
            {
                if (board.transform.GetChild(0).GetChild(neighborIndex) == go.transform)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
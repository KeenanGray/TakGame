using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class TurnData : ScriptableObject
{
    public IntReference CurrentPlayer = null;
    public BoolReference Picking = null;
    public GameObject[] Picked;
    public Tak.Stonetype PlaceType;

    public int[] CapStones;

    public void Init()
    {
        Picked = new GameObject[5];
        CapStones = new int[] { 1, 1 };
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Palette : ScriptableObject
{
    public ColorReference PlayerOne;
    public ColorReference PlayerTwo;
    public ColorReference Glow;
    public ColorReference SelectedHighlight;

    public ColorReference Table;
    public ColorReference Drawer;
    public ColorReference TableTop;
}

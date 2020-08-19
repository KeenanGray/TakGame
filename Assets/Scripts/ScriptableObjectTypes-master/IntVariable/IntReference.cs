using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class IntReference
{
    public bool UseConstant = true;
    public int ConstantValue = 0;
    public IntVariable Variable = null;

    public IntReference()
    {

    }
    public int Value
    {
        get { return UseConstant ? ConstantValue : Variable.Value; }
        set
        {
            if (UseConstant)
                ConstantValue = value;
            else
                Variable.Value = value;
        }
    }
}
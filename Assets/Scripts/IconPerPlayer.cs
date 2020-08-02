using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconPerPlayer : MonoBehaviour
{
    public ColorReference Color0;
    public ColorReference Color1;

    public IntReference CurPlayer;
    
    private Image CurImage;
    // Start is called before the first frame update
    void Start()
    {
        CurImage = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (CurPlayer.Value == 1)
            CurImage.color = Color1.Value;
        else if (CurPlayer.Value == 0)
            CurImage.color = Color0.Value;
    }
}

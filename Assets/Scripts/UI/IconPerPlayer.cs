using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconPerPlayer : MonoBehaviour
{
    [SerializeField]
    Palette palette = null;

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
            CurImage.color = palette.PlayerTwo.Value;
        else if (CurPlayer.Value == 0)
            CurImage.color = palette.PlayerOne.Value;
    }
}

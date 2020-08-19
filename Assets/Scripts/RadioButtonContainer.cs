using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadioButtonContainer : MonoBehaviour
{
    Button[] radioButtons;
    public Button selected;

    [SerializeField]
    ColorBlock pressed;

    [SerializeField]
    ColorBlock unpressed;

    [SerializeField]
    BoolReference IsMovingPiece;

    // Start is called before the first frame update
    void Start()
    {
        radioButtons = gameObject.GetComponentsInChildren<Button>();
        selected = radioButtons[0];

        foreach (Button b in radioButtons)
        {
            b.onClick.AddListener(delegate { ChangeRadioButton(b); });
            b.colors = unpressed;
        }
        StartCoroutine("FixColor");
        selected.colors = pressed;

        ChangeRadioButton(selected);
    }

    void ChangeRadioButton(Button button)
    {
        foreach (Button b in radioButtons)
        {
            b.colors = unpressed;
        }
        selected = button;
        selected.colors=pressed;
    }

    void Update()
    {

        foreach (Button b in radioButtons)
        {
                b.interactable = !IsMovingPiece.Value;
        }

    }

    IEnumerator FixColor()
    {
        yield return new WaitForEndOfFrame();
        selected.colors = pressed;
        yield break;
    }
}

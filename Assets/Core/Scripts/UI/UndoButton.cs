/*
Copyright SentientDragon5 2022
No unauthorized Commercial Use.
sentientdragon5gamedev@gmail.com for inquiries
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UndoButton : MonoBehaviour
{
    public GM gm;

    public Image image;
    public TextMeshProUGUI text;
    void Start()
    {
        gm = GM.inst;
    }

    void Update()
    {
        bool show = gm.undoMoves.Count > 0;
        text.enabled = show;
        image.enabled = show;
    }
}

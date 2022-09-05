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

public class UIButtonActionPreview : MonoBehaviour
{
    public GM gm;

    public int index = 0;
    public TextMeshProUGUI text;
    public TextMeshProUGUI disarmText;
    public Image image;

    private void Start()
    {
        gm = GM.inst;
    }

    private void Update()
    {
        if(gm.selectedUnit != null)
        {
            //Show
            image.enabled = true;

            if(gm.selectedAction == index)
            {
                image.color = Color.grey;
                disarmText.enabled = true;
                disarmText.text = "Click To Disarm";
            }
            else if(gm.selectedAction != -1)
            {
                image.color = Color.grey;
                disarmText.enabled = true;
                disarmText.text = "Click To Swap";
            }
            else
            {
                image.color = Color.white;
                disarmText.enabled = false;
            }

            //Set text
            text.enabled = true;
            text.text = gm.selectedUnit.Actions[index].name;
        }
        else
        {
            // Hide
            image.enabled = false;
            text.enabled = false;
            disarmText.enabled = false;
        }

    }
}

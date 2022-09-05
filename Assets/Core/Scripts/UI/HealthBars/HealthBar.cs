/*
Copyright SentientDragon5 2022
No unauthorized Commercial Use.
sentientdragon5gamedev@gmail.com for inquiries
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Unit unit;
    public Sprite[] mhpSprites;
    public Image mhpFrame;
    public Image hpBack;
    public Color green = Color.green;
    public Color red = Color.red;

    private void Start()
    {
        unit = GetComponentInParent<Unit>();
    }

    private void Update()
    {
        SetVisual();
    }

    public void SetVisual()
    {
        int mhp = unit.unitAttributes.maxHP;
        mhpFrame.sprite = mhpSprites[mhp - 1];
        hpBack.fillAmount = (float)unit.hp / (float)mhp;
        hpBack.color = unit.hp == 1 ? red : green;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Unit", menuName ="Unit")]
public class UnitSO : ScriptableObject
{
    public int baseAttack = 1;
    public int baseDefense = 1;
    public int maxHP = 4;
    public int moveDistance = 3;

    public float movementDist = 3;
    public ActionSO[] actions;
}
/*
Copyright SentientDragon5 2022
No unauthorized Commercial Use.
sentientdragon5gamedev@gmail.com for inquiries
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is an example of a derived type of action
/// </summary>

[CreateAssetMenu(fileName = "Heal", menuName = "Heal")]
public class HealActionSO : ActionSO
{
    public override void Enact(Tile tile)
    {
        if (GM.inst.TryFindUnit(tile.pos, out Unit u))
        {
            u.Heal(dmg);
        }
    }
}

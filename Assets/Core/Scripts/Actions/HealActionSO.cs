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

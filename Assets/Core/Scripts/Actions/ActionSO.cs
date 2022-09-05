/*
Copyright SentientDragon5 2022
No unauthorized Commercial Use.
sentientdragon5gamedev@gmail.com for inquiries
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// For AIs, Positive values should be used on Allies and negitives on Enemies
/// </summary>
public enum ActionType
{
    None = 0,
    /// <summary>
    /// A direct attack
    /// </summary>
    Attack = -1,
    /// <summary>
    /// A direct heal
    /// </summary>
    Heal = 1,
    PosBuff = 2,
    NegBuff = -2
}

/// <summary>
/// This is the base class of action that a unit can use
/// </summary>
[CreateAssetMenu(fileName ="Attack", menuName ="Actions/Attack")]
public class ActionSO : ScriptableObject
{
    /// <summary>
    /// For AIs, tag the type of the action and who it should be used on.
    /// </summary>
    public ActionType actionType;

    /// <summary>
    /// The base damage of the action whether that is attacking or healing
    /// </summary>
    public int dmg = 1;
    /// <summary>
    /// the base range of the action
    /// </summary>
    public int range = 1;

    /// <summary>
    /// Use the action. This should be overriden in derived classes to create different effects.
    /// </summary>
    /// <param name="tile"> The target tile, it isnt a unit because not all actions need to be on units.</param>
    public virtual void Enact(Tile tile)
    {
        if (GM.inst.TryFindUnit(tile.pos, out Unit u))
        {
            u.GetHit(dmg);
        }
    }
}

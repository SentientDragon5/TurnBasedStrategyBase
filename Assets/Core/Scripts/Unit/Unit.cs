/*
Copyright SentientDragon5 2022
No unauthorized Commercial Use.
sentientdragon5gamedev@gmail.com for inquiries
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// The class for a unit. change the unit's attributes by creating different UnitSOs then set the unit to have the SO in the inspector.
/// </summary>
public class Unit : MonoBehaviour
{
    /// <summary>
    /// The position of this unit
    /// </summary>
    public Vector2Int pos;
    /// <summary>
    /// This should only be set in the inspector
    /// </summary>
    public UnitSO unitAttributes;

    /// <summary>
    /// If it is null then the unit will be controlled by the user.
    /// </summary>
    public AIBehaviorSO aiBehavior;

    /// <summary>
    /// The team that this unit is on. Units of different teams can target them. Should be greater than 0
    /// </summary>
    public int team;

    /// <summary>
    /// For callbacks to this unit's death. you could add death VFX this way.
    /// </summary>
    public UnityEvent onDie;

    /// <summary>
    /// a pointer to this unit's movement distance (not settable)
    /// </summary>
    public float MovementDist
    {
        get => unitAttributes.movementDist;
    }
    /// <summary>
    /// a pointer to the actions array (not settable)
    /// </summary>
    public ActionSO[] Actions
    {
        get => unitAttributes.actions;
    }

    private void Start()
    {
        hp = unitAttributes.maxHP;
    }

    /// <summary>
    /// So they don't float or clip into the ground.
    /// </summary>
    public float groundOffset = 0;
    private void OnValidate()
    {
        pos = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
    }
    /// <summary>
    /// Has this unit moved?
    /// </summary>
    public bool moved = false;
    /// <summary>
    /// Has this unit used it's action?
    /// </summary>
    public bool usedAction = false;

    /// <summary>
    /// the unit's current HP, Use GetHit(dmg) to deduct health
    /// </summary>
    public int hp = 3;

    /// <summary>
    /// Use this to damage the unit
    /// </summary>
    /// <param name="dmg"></param>
    public void GetHit(int dmg)
    {
        hp -= dmg;
        hp = Mathf.Clamp(hp, 0, unitAttributes.maxHP);
        if (hp == 0)
            Die();
    }

    public void Die()
    {
        // GM will remove from the units list. DO NOT EDIT THE LIST EXTERNALLY it is possible I am iterating thru the loop right now
        onDie.Invoke();
        Destroy(gameObject, 3);// you can increase the delay of the destroy to allow for time for an animation
    }
    /// <summary>
     /// Use this to heal the unit
     /// </summary>
     /// <param name="dmg"></param>
    public void Heal(int dmg)
    {
        hp += dmg;
        hp = Mathf.Clamp(hp, 0, unitAttributes.maxHP);
    }
    /// <summary>
    /// this will use the action on the tile
    /// </summary>
    /// <param name="action">index in this unit's action array</param>
    /// <param name="tile">the target tile</param>
    public void UseAction(int action, Tile tile)
    {
        usedAction = true;
        unitAttributes.actions[action].Enact(tile);
        moved = true;//Can't move after action
    }

    /// <summary>
    /// This will use the type of action on the tile
    /// </summary>
    /// <param name="actionType">What type of action to use</param>
    /// <param name="tile">the target tile</param>
    /// <returns>whether the action was found</returns>
    public bool UseAction(ActionType actionType, Tile tile)
    {
        if (TryGetActionOfType(actionType, out ActionSO act))
        {
            usedAction = true;
            act.Enact(tile);
            moved = true;//Can't move after action
            return true;
        }
        return false;
    }
    /// <summary>
    /// This will use the action on the tile
    /// </summary>
    /// <param name="action">What action to use</param>
    /// <param name="tile">the target tile</param>
    public void UseAction(ActionSO action, Tile tile)
    {
        usedAction = true;
        action.Enact(tile);
        moved = true;//Can't move after action
    }

    /// <summary>
    /// Try to get an action of this type on this unit.
    /// </summary>
    /// <param name="actionType">the type of action</param>
    /// <param name="action">the action</param>
    /// <returns>whether there was an action</returns>
    public bool TryGetActionOfType(ActionType actionType, out ActionSO action)
    {
        action = null;
        foreach (ActionSO act in Actions)
        {
            if (act.actionType == actionType)
            {
                action = act;
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Use this to move this unit
    /// </summary>
    /// <param name="dest">the destination</param>
    public void Move(Vector2Int dest, out UndoMove undo)
    {
        moved = true;
        transform.position = new Vector3(dest.x, groundOffset, dest.y);
        undo = new UndoMove(this, pos);
        pos = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
    }

    /// <summary>
    /// Use this to move this unit without getting an undo recipt
    /// </summary>
    /// <param name="dest">the destination</param>
    public void Move(Vector2Int dest)
    {
        moved = true;
        transform.position = new Vector3(dest.x, groundOffset, dest.y);
        pos = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
    }
}
[System.Serializable]
public class UndoMove
{
    public Unit unit;
    public Vector2Int fromPos;

    public UndoMove(Unit unit, Vector2Int fromPos)
    {
        this.unit = unit;
        this.fromPos = fromPos;
    }
}
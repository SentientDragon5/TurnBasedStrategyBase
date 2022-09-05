/*
Copyright SentientDragon5 2022
No unauthorized Commercial Use.
sentientdragon5gamedev@gmail.com for inquiries
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// This is an AI behavior, you can derive from it to create different AIs for different purposes.
/// </summary>
[CreateAssetMenu(fileName = "AI Behavior", menuName = "AI/Base AI Behavior")]
public class AIBehaviorSO : ScriptableObject
{
    // Have variables to configure the AI of the unit
    #region Instance Vars
    /// <summary>
    /// the likeliness that the Ai will choose to heal an Ally
    /// </summary>
    public float healRate = 0.2f;

    /// <summary>
    /// An array of directions used for checking in those directions.
    /// </summary>
    readonly Vector2Int[] dirs = new Vector2Int[4]
    {
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right,
        Vector2Int.up
    };
    #endregion

    /// <summary>
    /// This method decides what to do and does it.
    /// </summary>
    /// <param name="me">The unit to run the AI on</param>
    /// <returns>Whether it killed another unit</returns>
    public void AIBehavior(Unit me, BoardType boardType)
    {
        List<Unit> targets = new List<Unit>();
        ActionType actionType = ActionType.None;
        if (HurtAllyWithinRange(me, out Unit allyToHelp))
        {
            Debug.Log("Headed to ally: " + allyToHelp.gameObject.name);
            targets.Add(allyToHelp);
            actionType = ActionType.Heal;
        }
        else if (EnemyInRange(me, out Unit enemy))
        {
            Debug.Log("Headed to enemy: " + enemy.gameObject.name);
            targets.Add(enemy);
            actionType = ActionType.Attack;
        }
        if (UnitsExist(me, out List<Unit> enemies, -me.team))
        {
            Debug.Log("Finding " + enemies[0] + " from afar");
            targets.AddRange(enemies);
        }

        if (targets.Count > 0)
        {
            targets = targets.OrderBy(other => Vector2.Distance(me.pos, other.pos)).ToList();

            if(!TryUseAction(me, targets[0], actionType))
            {
                // if couldn't use action at current location
                // move

                List<Tile> options = new List<Tile>();
                foreach (Tile t in GM.inst.board)
                {
                    if (GM.inst.MoveDestinationValid(t, me))
                    {
                        options.Add(t);
                    }
                }
                options = options.OrderBy(t => Vector2.Distance(t.pos, targets[0].pos)).ToList();
                if (options.Count > 0)
                {
                    me.Move(options[0].pos);
                }
                if(TryUseAction(me,targets[0], actionType))
                {
                    //Success
                }
            }

            
            return;
        }
        /*

        // Heal or attack?
        // Is there an ally with low health nearby?
        if (HurtAllyWithinRange(me, out Unit allyToHelp))
        {
            Debug.Log("Headed to ally: " + allyToHelp.gameObject.name);
            //Move to them
            Vector2Int dest = Vector2Int.zero;
            bool foundPath = true;
            foreach(Vector2Int dir in dirs)
            {
                if(GM.inst.TryFindTile(allyToHelp.pos + dir, out Tile t))
                {
                    if (GM.inst.MoveDestinationValid(t,me))
                    {
                        dest = allyToHelp.pos + dir;
                        foundPath = true;
                        break;
                    }
                }
            }

            if(foundPath)
            {
                //Continue with trying to move there
                me.Move(dest);
                if(GM.inst.TryFindTile(allyToHelp.pos, out Tile t))
                {
                    if (me.UseAction(ActionType.Heal, t))
                    {
                        // Sucessfully used the action! This unit's turn is done.
                        
                    }
                }
            }
            Debug.Log("Finding " + allyToHelp + " from near");
            return;
        }

        // Find a target
        if (EnemyInRange(me, out Unit enemy))
        {
            Debug.Log("Headed to enemy: " + enemy.gameObject.name);
            //Move to them
            Vector2Int dest = Vector2Int.zero;
            bool foundPath = true;
            foreach (Vector2Int dir in dirs)
            {
                if (GM.inst.TryFindTile(enemy.pos + dir, out Tile t))
                {
                    if (GM.inst.MoveDestinationValid(t, me))
                    {
                        dest = enemy.pos + dir;
                        Debug.Log(dest);
                        foundPath = true;
                        break;
                    }
                }
            }

            if (foundPath)
            {
                //Continue with trying to move there
                me.Move(dest);
                Debug.Log("done " + me.pos);
                if (GM.inst.TryFindTile(enemy.pos, out Tile t))
                {
                    Debug.Log("action tile : " + t.pos);
                    if (me.UseAction(ActionType.Attack, t))
                    {
                        // Sucessfully used the action! This unit's turn is done.
                        Debug.Log("Used action");
                    }
                }
            }
            Debug.Log("Finding " + enemy + " from near");
            return;
        }

        // Are there ANY enemies?
        if(UnitsExist(me, out List<Unit> enemies, -me.team))
        {
            // In theory you would rank the enemiew by their distance or something

            Vector2 offset = enemies[0].pos - me.pos;


            Vector2Int dest = Vector2Int.zero;
            bool foundPath = false;
            foreach (Vector2Int dir in dirs)
            {
                for (int dist = 0; dist < (int)me.MovementDist; dist++)
                {
                    Vector2 forecastOffset = enemies[0].pos - (me.pos + dir * dist);
                    if (forecastOffset.sqrMagnitude < offset.sqrMagnitude)
                    {
                        if (GM.inst.TryFindTile((me.pos + dir * (dist)), out Tile t))
                        {
                            Debug.Log("FOUND");
                            if (GM.inst.MoveDestinationValid(t, me))
                            {
                                dest = (me.pos + dir * dist);
                                foundPath = true;
                                break;
                            }
                        }
                    }
                }            
            }
            //next you would rank each potential path based on how close it took you to the target.

            if (foundPath)
            {
                //Continue with trying to move there
                me.Move(dest);
                
            }
            Debug.Log("Finding " + enemies[0] + " from afar");
            return;
        }
        */
        Debug.Log("Nothing to do");
        // If it hasnt returned by now then Do nothing I guess?
        // You can add your own default behavior
    }


    #region Helper Methods

    public static bool TryUseAction(Unit me, Unit target, ActionType actionType)
    {
        if (GM.inst.TryFindTile(target.pos, out Tile targetTile))
        {
            if (me.TryGetActionOfType(actionType, out ActionSO act))
            {
                if (GM.inst.ActionTargetValid(targetTile, me, act))
                {
                    me.UseAction(act, targetTile);
                    return true;
                }
            }
        }
        return false;
    }


    public static int DistToTarget(Unit me, Unit target)
    {
        if (GM.inst.TryFindTile(me.pos, out Tile meTile))
        {
            if (GM.inst.TryFindTile(target.pos, out Tile targetTile))
            {
                return targetTile.StepsTo(meTile, GM.inst.board);
            }
        }
        return -1;
    }

    /// <summary>
    /// Are there any units within the specified range?
    /// </summary>
    /// <param name="me">The unit to check from</param>
    /// <param name="range">The distance from me</param>
    /// <param name="unitsNear">the return list units nearby</param>
    /// <returns>whether unitsNear.Count > 0</returns>
    public static bool UnitsWithinRange(Unit me, float range, out List<Unit> unitsNear)
    {
        unitsNear = new List<Unit>();

        foreach(Unit u in GM.inst.unitsA)
        {
            if (u != me)
            {
                if (GM.inst.CanReach(me, u.pos, range))
                {
                    unitsNear.Add(u);
                }
            }
        }
        return unitsNear.Count > 0;
    }
    /// <summary>
    /// Are there any units of team within the specified range?
    /// </summary>
    /// <param name="me">The unit to check from</param>
    /// <param name="range">The distance from me</param>
    /// <param name="unitsNear">the return list units nearby</param>
    /// <param name="teamFilter">Positive Value will return units only of that team, Negitive will return units of NOT that team</param>
    /// <returns>whether unitsNear.Count > 0</returns>
    public static bool UnitsWithinRange(Unit me, float range, out List<Unit> unitsNear, int teamFilter)
    {
        unitsNear = new List<Unit>();

        foreach (Unit u 
            in GM.inst
            .unitsA)
        {
            if (u != me)
            {
                if (teamFilter < 0)
                {
                    if (u.team != teamFilter)
                    {
                        if (GM.inst.CanReach(me, u.pos, range))
                        {
                            unitsNear.Add(u);
                        }
                    }
                }
                else
                {
                    if (u.team == teamFilter)
                    {
                        if (GM.inst.CanReach(me, u.pos, range))
                        {
                            unitsNear.Add(u);
                        }
                    }
                }
            }
        }
        return unitsNear.Count > 0;
    }
    
    /// <summary>
    /// Finds an ally within walking distance to help
    /// </summary>
    /// <param name="me"></param>
    /// <param name="allyToHelp"></param>
    /// <returns></returns>
    public static bool HurtAllyWithinRange(Unit me, out Unit allyToHelp)
    {
        allyToHelp = null;
        if (UnitsWithinRange(me, me.MovementDist - 1, out List<Unit> allies, me.team))
        {
            foreach(Unit u in allies)
            {
                if(u.hp < u.unitAttributes.maxHP)
                {
                    allyToHelp = u;
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Finds an enemy within walking distance to fight
    /// </summary>
    /// <param name="me"></param>
    /// <param name="enemy"></param>
    /// <returns></returns>
    public static bool EnemyInRange(Unit me, out Unit enemy)
    {
        enemy = null;
        if (UnitsWithinRange(me, me.MovementDist - 1, out List<Unit> enemies, -me.team))// notice the negitive in the team filter
        {
            foreach (Unit u in enemies)
            {
                enemy = u;
                return true;
            }
        }
        return false;
    }
    
    /// <summary>
    /// Finds a unit within walking distance to fight
    /// </summary>
    /// <param name="me"></param>
    /// <param name="units"></param>
    /// <param name="teamFilter">Positive Value will return units only of that team, Negitive will return units of NOT that team</param>
    /// <returns></returns>
    public static bool UnitsExist(Unit me, out List<Unit> units, int teamFilter)
    {
        units = new List<Unit>();

        foreach (Unit u in GM.inst.unitsA)
        {
            if(u != me)
            {
                if (teamFilter < 0)
                {
                    if (u.team != teamFilter)
                    {
                        units.Add(u);
                    }
                }
                else
                {
                    if (u.team == teamFilter)
                    {
                        units.Add(u);
                    }
                }
            }           
        }
        return units.Count > 0;
    }
    #endregion
}

/// <summary>
/// the AI would have a list of these targets that it would rank based on priority.
/// You would still need to figure out how you want to assign priority.
/// </summary>
public class AITarget
{
    // here is a snipet of what it would probably look like
    // List<AITarget> targets = new List<AITarget>();
    // (Add targets)
    // targets = targets.OrderBy(t => t.priority).ToList();

    /// <summary>
    /// The unit and destination
    /// </summary>
    public Unit target;
    /// <summary>
    /// The priority of this action. Could be dependant on the unit type, proximity to ally, proximity to me, etc.
    /// </summary>
    public int priority;
    /// <summary>
    /// What would you do when you get there?
    /// </summary>
    public ActionType actionType;
}
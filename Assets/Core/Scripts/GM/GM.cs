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
/// Game Master
/// This Script runs the game and will control player input and will
/// set the visuals
/// </summary>
public class GM : MonoBehaviour
{
    #region Singleton
    /// <summary>
    /// The instance of GM3
    /// </summary>
    public static GM inst;
    private void Awake()
    {
        if (inst == null)
            inst = this;
    }
    #endregion

    #region Instance Variables
    /// <summary>
    /// The number of turns since the start of the game;
    /// </summary>
    public int turn = 0;

    /// <summary>
    /// The type of board
    /// </summary>
    public BoardType boardType;
    /// <summary>
    /// The Array of all tiles on the board.
    /// </summary>
    public List<Tile> board;
    /// <summary>
    /// The Units
    /// </summary>
    public List<Unit> unitsA;

    public Tile selectedTile;
    /// <summary>
    /// The currently selected unit.
    /// </summary>
    public Unit selectedUnit;
    /// <summary>
    /// The selected action index in the unit's actions to use
    /// </summary>
    public int selectedAction;

    /// <summary>
    /// Whether actionOptions should be set active
    /// </summary>
    public bool actionOptionsShow;
    /// <summary>
    /// The Transform of the UI that shows the options of what Actions
    /// that unit can use
    /// </summary>
    public Transform actionOptions;

    /// <summary>
    /// My admitidly a bit hacky solution because the graphic raycaster
    /// was not keeping my raycasts from not shooting if i click on the
    /// canvas.
    /// </summary>
    bool ignoreResetSelection;

    /// <summary>
    /// All the last undoMoves since the last action or turn start
    /// </summary>
    public List<UndoMove> undoMoves;

    public UnityEvent onWin;
    public UnityEvent onLose;
    public UnityEvent onEndTurn;
    #endregion

    private void Start()
    {
        ResetSelection();
        EndTurn();
    }

    /// <summary>
    /// End the turn, have all the AIs go in their respective order,
    /// then reset all units to ready
    /// </summary>
    public void EndTurn()
    {
        turn++;

        // Have AIs go
        int i = 0;
        while(i < unitsA.Count)
        {
            Unit u = unitsA[i];
            if (u != null && u.hp > 0)
            {
                if (u.aiBehavior != null)
                {
                    u.aiBehavior.AIBehavior(u, boardType);
                }

                i++;
            }
            else
            {
                unitsA.Remove(u);
            }
        }
        foreach (Unit u in unitsA)
        {
            
        }
        // Allow Units to move and take actions
        foreach (Unit u in unitsA)
        {
            u.moved = false;
            u.usedAction = false;
        }
        undoMoves.Clear();

        if (!(CheckAliveUnitsOnTeam(1) > 0))
        {
            onLose.Invoke();
            return;
        }
        if (!(CheckAliveUnitsOnTeam(2) > 0))
        {
            onWin.Invoke();
            return;
        }

        onEndTurn.Invoke();
    }

    /// <summary>
    /// This checks how many units on the team there are left alive
    /// </summary>
    /// <param name="team"></param>
    /// <returns>Number of alive units on team</returns>
    public int CheckAliveUnitsOnTeam(int team)
    {
        int aliveUnits = 0;
        foreach(Unit u in unitsA)
        {
            if (u.team == team && u.hp > 0)
            {
                aliveUnits++;
            }
        }
        return aliveUnits;
    }


    /// <summary>
    /// Try to find a Unit at a position
    /// </summary>
    /// <param name="pos">The position to check</param>
    /// <param name="found">the unit at the location (null if not found)</param>
    /// <returns>Whether a unit was found at the location</returns>
    public bool TryFindUnit(Vector2Int pos, out Unit found)
    {
        foreach (Unit d in unitsA)
        {
            if (d.pos == pos)
            {
                found = d;
                return true;
            }
        }
        found = null;
        return false;
    }
    /// <summary>
    /// Try to find a Tile at a position
    /// </summary>
    /// <param name="pos">The position to check</param>
    /// <param name="found">the tile at the location (null if not found)</param>
    /// <returns>Whether a tile was found at the location</returns>
    public bool TryFindTile(Vector2Int pos, out Tile found)
    {
        foreach (Tile d in board)
        {
            if (d.pos == pos)
            {
                found = d;
                return true;
            }
        }
        found = null;
        return false;
    }

    #region Selection Setting Methods
    /// <summary>
    /// Clear Data about what is selected
    /// </summary>
    void ResetSelection()
    {
        selectedAction = -1;
        SetSelectedTile(null);
        selectedUnit = null;
    }
    /// <summary>
    /// Set the selected tile
    /// </summary>
    /// <param name="tile"></param>
    public void SetSelectedTile(Tile tile)
    {
        selectedTile = tile;
    }
    /// <summary>
    /// Move the unit to a different tile
    /// </summary>
    /// <param name="tile">the destination</param>
    /// <param name="unit">the unit to move</param>
    public void SetMoveTile(Tile tile, Unit unit)
    {
        // Move
        unit.Move(tile.pos, out UndoMove undo);
        undoMoves.Add(undo);
        selectedTile = tile;
    }
    /// <summary>
    /// For  the UI Button to set
    /// </summary>
    /// <param name="action">The index of the action of the unit</param>
    public void SetSelectedAttack(int action)
    {
        if(selectedAction == action)
        {
            selectedAction = -1;
        }
        else
        {
            selectedAction = action;
        }
        ignoreResetSelection = true;
        //print(selectedTile.gameObject.name);
        //TryFindTile(selectedUnit.pos, out selectedTile);
    }
    /// <summary>
    /// has the unit use the action on the clicked tile.
    /// </summary>
    /// <param name="tile">The tile to use the action on</param>
    /// <param name="unit">The unit to use the action</param>
    public void SetTargetTile(Tile tile, Unit unit)
    {
        unit.UseAction(selectedAction, tile);

        undoMoves.Clear();
        ResetSelection();
    }
    
    /// <summary>
    /// Undos the move of the last unit
    /// </summary>
    public void UndoMove()
    {
        ignoreResetSelection = true;
        if (undoMoves.Count > 0)
        {
            UndoMove undo = undoMoves[undoMoves.Count-1];
            undo.unit.Move(undo.fromPos);
            undo.unit.moved = false;
            if(TryFindTile(undo.fromPos, out Tile t))
            {
                SetSelectedTile(t);
            }
            undoMoves.RemoveAt(undoMoves.Count-1);
        }
    }
    #endregion

    /// <summary>
    /// Theoreticly this could be moved from the Update Method to a calback every time the mouse is clicked.
    /// I just put it here for simplicity sake, but this is super inefficient.
    /// </summary>
    public void Update()
    {
        //Set all tile to none
        foreach (Tile tile in board)
        {
            tile.UpdatePreviewUnselected();
        }
        actionOptionsShow = false;
        
        
        if (selectedTile != null)
        {
            selectedTile.selected = true;

            if(TryFindUnit(selectedTile.pos, out Unit found))
            {
                selectedUnit = found;
            }
        }

        if(selectedUnit != null && selectedUnit.aiBehavior == null)
        {
            if (selectedAction == -1)
            {
                if (!selectedUnit.moved)
                {
                    // Asking Player to select a Destination
                    foreach (Tile tile in board)
                    {
                        tile.UpdateMovementPreview(MoveDestinationValid(tile, selectedUnit));
                    }
                }
                
            }
            else if(!selectedUnit.usedAction)
            {
                // Asking Player to select a Target
                foreach (Tile tile in board)
                {
                    tile.UpdateActionPreview(ActionTargetValid(tile, selectedUnit, selectedAction));
                }
            }

            if (!selectedUnit.usedAction)
            {
                //   Asking Player to select or deselect an Action
                actionOptionsShow = true;
            }
        }


        actionOptions.gameObject.SetActive(actionOptionsShow);

        if (ignoreResetSelection)
        {
            ignoreResetSelection = false;
            return;
        }

        ClickType hitstate = CheckClick(out Tile p);
        if (hitstate == ClickType.Hit)
        {
            bool usedClick = false;
            if(selectedUnit != null)
            {
                if (selectedTile == null)
                {
                    // Asking Player to select a tile
                    SetSelectedTile(p);
                    usedClick = true;
                }
                else if (selectedAction == -1)
                {
                    // Asking Player to select a Destination
                    if (!selectedUnit.moved)
                    {
                        if (MoveDestinationValid(p, selectedUnit))
                        {
                            SetMoveTile(p, selectedUnit);
                            usedClick = true;
                        }
                    }

                    // Asking Player to select an Action
                    // (Select on UI)
                }
                else if (!selectedUnit.usedAction)
                {
                    if (ActionTargetValid(p, selectedUnit, selectedAction))
                    {
                        SetTargetTile(p, selectedUnit);
                        usedClick = true;
                    }
                }

            }

            //If we did not use the click then set the selection to whatever was clicked
            if (!usedClick)
            {
                SetSelectedTile(p);
            }
        }
        else if (hitstate == ClickType.Missed)
        {
            //Reset State to None
            ResetSelection();
        }

        if (selectedTile != null)
        {
            selectedTile.selected = true;

            if (TryFindUnit(selectedTile.pos, out Unit found))
            {
                selectedUnit = found;
            }
        }
    }

    #region Click Checker
    /// <summary>
    /// The position of the mouse when first clicked.
    /// </summary>
    Vector3 startPos;
    /// <summary>
    /// The max distance that mouse can move before not considering the click for an input
    /// If we don't do this then we can misinterpret drags as clicks.
    /// </summary>
    float maxDist = 1f;
    /// <summary>
    /// The type of the click for the CheckClick Method
    /// </summary>
    enum ClickType
    {
        Ignored = 0,
        Hit = 1,
        Missed = 2
    }
    /// <summary>
    /// This Method shoots a ray from the screen to the mouse in world space.
    /// </summary>
    /// <param name="o">The tile that was clicked</param>
    /// <returns>Whether the Ray was not shot (player was dragging), Ray hit, or Ray missed</returns>
    ClickType CheckClick(out Tile o)
    {
        o = null;
        if (Input.GetMouseButtonDown(0))//|| Time.deltaTime > 1f)
            startPos = Input.mousePosition;
        if (Input.GetMouseButtonUp(0))
        {
            // Invalidate if player is dragging
            if (Vector3.Distance(startPos, Input.mousePosition) > maxDist)
                return 0;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.TryGetComponent(out Tile c))
                {
                    o = c;
                    return ClickType.Hit;
                }
            }
            return ClickType.Missed;
        }
        return ClickType.Ignored;
    }
    #endregion



    #region MOVEMENT ALGORITHMS

    // Go ahead and change these as needed so that movement is to your satisfaction

    /// <summary>
    /// This will return whether the destination is valid for variety of reasons.
    /// </summary>
    /// <param name="tile"></param>
    /// <param name="selected"></param>
    /// <returns></returns>
    public bool MoveDestinationValid(Tile tile, Unit selected)
    {
        if(TryFindTile(selected.pos, out Tile start))
        {
            bool withinRange = CanReach(start, tile, selected.MovementDist);//CanReach(selected, tile.pos, selected.MovementDist);//Vector2.Distance(selected.pos, tile.pos) <= selected.MovementDist;
            bool notOccupied = !TryFindUnit(tile.pos, out Unit u);
            bool obstructed = UnitInPath(selected, tile);// just an idea, the method doesnt actually do anything. 
            return withinRange && notOccupied;
        }
        return false;
    }
    /// <summary>
    /// This will find whether the provided tile is a valid target tile for the selected unit
    /// </summary>
    /// <param name="tile">the target tile </param>
    /// <param name="selected">The unit preforming the action</param>
    /// <param name="action">the index of the action</param>
    /// <returns></returns>
    public bool ActionTargetValid(Tile tile, Unit selected, int action)
    {
        if(boardType == BoardType.Grid)
        {

            bool sameRowOrColumn = selected.pos.x == tile.pos.x || selected.pos.y == tile.pos.y;
            bool withinRange = Vector2.Distance(selected.pos, tile.pos) <= selected.Actions[action].range;

            return sameRowOrColumn && withinRange;
        }
        if(boardType == BoardType.Node)
        {
            if (TryFindTile(selected.pos, out Tile start))
            {
                bool withinRange = CanReach(start, tile, selected.Actions[action].range);
                bool occupied = TryFindUnit(tile.pos, out Unit u);
                return withinRange;// && occupied;
            }
        }
        return false;

    }
    /// <summary>
    /// This will find whether the provided tile is a valid target tile for the selected unit
    /// </summary>
    /// <param name="tile">the target tile </param>
    /// <param name="selected">The unit preforming the action</param>
    /// <param name="action">the action</param>
    /// <returns></returns>
    public bool ActionTargetValid(Tile tile, Unit selected, ActionSO action)
    {
        if (boardType == BoardType.Grid)
        {

            bool sameRowOrColumn = selected.pos.x == tile.pos.x || selected.pos.y == tile.pos.y;
            bool withinRange = Vector2.Distance(selected.pos, tile.pos) <= action.range;

            return sameRowOrColumn && withinRange;
        }
        if (boardType == BoardType.Node)
        {
            if (TryFindTile(selected.pos, out Tile start))
            {
                bool withinRange = CanReach(start, tile, action.range);
                bool occupied = TryFindUnit(tile.pos, out Unit u);
                return withinRange;// && occupied;
            }
        }
        return false;

    }

    // Movement Helper Methods
    /// <summary>
    /// This method could be updated to see if the unit would be able to reach the destination and if it is within the range
    /// </summary>
    /// <param name="unit"></param>
    /// <param name="dest"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public bool CanReach(Tile start, Tile dest, float range)
    {
        return start.StepsTo(dest, board) < range;
        // Old simple distance Function
        //return Vector2.Distance(unit.pos, dest) <= range;
    }
    /// <summary>
    /// This method could be updated to see if the unit would be able to reach the destination and if it is within the range
    /// </summary>
    /// <param name="unit"></param>
    /// <param name="dest"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public bool CanReach(Unit unit, Vector2Int dest, float range)
    {
        if(TryFindTile(unit.pos, out Tile start))
        {
            if(TryFindTile(dest, out Tile end))
            {
                return start.StepsTo(end, board) < range;
            }
        }
        return false;
        // Old simple distance Function
        //return Vector2.Distance(unit.pos, dest) <= range;
    }
    /// <summary>
    /// Not Implemented - Always returns false
    /// </summary>
    /// <param name="selected"></param>
    /// <param name="destination"></param>
    /// <returns></returns>
    bool UnitInPath(Unit selected, Tile destination)
    {
        Vector2Int offset = destination.pos - selected.pos;
        // Not sure how I want to implement this algorithm. You can probably find a solution
        return false;
    }
    #endregion
}


public enum BoardType
{
    Grid,
    Node
}
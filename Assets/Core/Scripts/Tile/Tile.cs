/*
Copyright SentientDragon5 2022
No unauthorized Commercial Use.
sentientdragon5gamedev@gmail.com for inquiries
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    /// <summary>
    /// The position of this tile
    /// </summary>
    public Vector2Int pos;
    /// <summary>
    /// whether this tile is selected by the user. Only one tile should ever be se
    /// </summary>
    public bool selected;
    public bool movable;
    public bool attackable;

    private void OnValidate()
    {
        pos = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
    }

    /// <summary>
    /// Colors for types of selection
    /// </summary>
    public Color[] colors = new Color[4]
    {
        Color.grey,
        Color.white,
        Color.green,
        Color.red
    };

    public MeshRenderer selectionRenderer;
    Material mat;
    void Start()
    {
        Material m = selectionRenderer.material;
        mat = Instantiate(m);
        selectionRenderer.material = mat;
    }
    public int i = 0;
    void Update()
    {
        if (selected)
            i = 1;
        else if (movable)
            i = 2;
        else if (attackable)
            i = 3;
        else
            i = 0;

        mat.SetColor("_BaseColor", colors[i]);
        mat.SetColor("_Color", colors[i]);
    }

    public void UpdateMovementPreview(bool canMove)
    {
        movable = canMove;
        attackable = false;
    }
    public void UpdateActionPreview(bool canAction)
    {
        movable = false;
        attackable = canAction;
    }
    public void UpdatePreviewUnselected()
    {
        selected = false;
        movable = false;
        attackable = false;
    }

    /// <summary>
    /// Calculates the amount of steps from this tile to the destination tile.
    /// </summary>
    /// <param name="dest">The destination</param>
    /// <param name="board">All movable locations</param>
    /// <returns>the amount of steps from this tile to another</returns>
    public virtual int StepsTo(Tile dest, List<Tile> board)
    {
        // Add obstacle avoidance and account for that with board
        // that will takes some tile to figure out that algorithm
        return Mathf.Abs(dest.pos.x - this.pos.x) + Mathf.Abs(dest.pos.y - this.pos.y);
    }
}

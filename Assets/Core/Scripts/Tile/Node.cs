using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : Tile
{
    public List<Node> paths;
    

    private void OnDrawGizmos()
    {
        foreach(Node n in paths)
        {
            if (n.paths.Contains(this))
            {
                Gizmos.color = Color.green;
            }
            else
                Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, n.transform.position);
        }
    }
    [ContextMenu("Fix Paths")]
    public void FixPaths()
    {
        List<Node> newlist = new List<Node>();
        foreach (Node n in paths)
        {
            if (!newlist.Contains(n))
            {
                newlist.Add(n);
            }
            if (!n.paths.Contains(this))
            {
                n.paths.Add(this);
            }
        }
        paths = newlist;
    }

    /// <summary>
    /// Calculates the amount of steps from this tile to the destination tile.
    /// </summary>
    /// <param name="dest">The destination</param>
    /// <param name="board">All movable locations</param>
    /// <returns>the amount of steps from this tile to another, -1 if not connected</returns>
    public override int StepsTo(Tile dest, List<Tile> board)
    {
        bool connected = Connected((Node)dest, out int steps, 5);
        return connected ? steps : -1;
    }

    /// <summary>
    /// recurses thru the branch structure of the nodes.
    /// </summary>
    /// <param name="other">the other node</param>
    /// <param name="steps">the number of steps to other. -1 if false, 0 if other == this</param>
    /// <returns>whether this node is connected to other</returns>
    public bool Connected(Node other, out int steps, int maxSteps)
    {
        steps = -1;
        if (other == this)
        {
            steps = 0;
            return true;
        }
        if (paths.Contains(other))
        {
            steps = 1;
            return true;
        }
        if(maxSteps > 1)
        {
            foreach (Node n in paths)
            {
                if (n.Connected(other, out steps, maxSteps - 1))
                {
                    steps++;
                    return true;
                }
            }
        }
        // dag https://en.wikipedia.org/wiki/Directed_acyclic_graph
        // Look it up

        /*  Look man, recursion is hard. maybe ill try it again later
        
        steps = -1;
        if (other == this)
        {
            steps = 0;
            return true;
        }
        else
        {
            if (paths.Contains(other))
            {
                steps = 1;
                return true;
            }

            foreach (Node n in paths)
            {
                if (n.Connected(other, out steps))
                {
                    return true;
                }
            }
        }
        // Nope again
        if(other == this)
        {
            steps = 0;
            return true;
        }
        else
        {
            foreach (Node n in paths)
            {
                // 1 more deep
                if (n.paths.Contains(this))
                {
                    steps = 1;
                    return true;
                }
                else if (n.Connected(other, out steps))// must be deeper
                {
                    steps++;
                    return true;
                }
            }
        }
        */
        return false;
    }
}

[System.Serializable]
public class Path
{
    public Node a;
    public Node b;

    public Path(Node a, Node b)
    {
        this.a = a;
        this.b = b;
    }

    public bool Equals(Path obj)
    {
        Path oth = (Path)obj;
        bool aIsA = oth.a == this.a;
        bool bIsB = oth.b == this.b;
        bool aisB = oth.a == this.b;
        bool bisA = oth.b == this.a;
        return (aIsA && bIsB) || (aisB && bisA);
    }
}
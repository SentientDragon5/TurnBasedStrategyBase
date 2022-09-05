/*
Copyright SentientDragon5 2022
No unauthorized Commercial Use.
sentientdragon5gamedev@gmail.com for inquiries
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathCreator : MonoBehaviour
{
    public GameObject pathPrefab;
    public List<Node> nodes;
    public List<Path> paths;
    public Transform pathParent;

    [ContextMenu("Draw Paths")]
    public void DrawPaths()
    {
        paths.Clear();
        foreach(Node a in nodes)
        {
            foreach(Node b in a.paths)
            {
                Path n = new Path(a, b);
                bool contains = false;
                foreach(Path p in paths)
                {
                    if (n.Equals(p))
                    {
                        contains = true;
                    }
                }
                if (!contains)
                {
                    paths.Add(n);
                }
            }
        }

        for (int i = 0; i < nodes.Count; i++)
        {

        }


        foreach (Path p in paths)
        {
            GameObject g = Instantiate(pathPrefab);
            LineRenderer l = g.GetComponent<LineRenderer>();
            Vector3[] positions = new Vector3[2]
            {
                new Vector3(p.a.pos.x, 0, p.a.pos.y),
                new Vector3(p.b.pos.x, 0, p.b.pos.y)
            };
            l.SetPositions(positions);
        }
        //Draws the paths with besier curves or parabolas or lines?
    }
}

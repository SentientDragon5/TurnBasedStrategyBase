// Logan S
// Known Issue of rotations not working.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Level Building Utilities/Plane Builder")]
public class Builder : MonoBehaviour
{

    public enum BuilderMode { Build, Preview};

    public BuilderMode mode = BuilderMode.Build;
    [Header("Prefab"), SerializeField, Tooltip("The Prefab with which to create more of")] GameObject prefab;
    [SerializeField, Tooltip("The Prefab's size and offset to allow for connections")] Bounds prefabBounds = new Bounds(Vector3.zero, new Vector3(5,0,5));


    public Bounds PreviewSize()
    {
        return prefabBounds;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(prefabBounds.center + transform.position, prefabBounds.size);
    }


    public Vector3Int intPosw(Vector3 worldPos)
    {
        Vector3 localpos = worldPos - transform.position;
        return new Vector3Int(Mathf.RoundToInt(localpos.x / prefabBounds.size.x), Mathf.RoundToInt(localpos.y / prefabBounds.size.y), Mathf.RoundToInt(localpos.z / prefabBounds.size.z));
    }
    public Vector3Int intPosl(Vector3 localpos)
    {
        return new Vector3Int(Mathf.RoundToInt(localpos.x / prefabBounds.size.x), Mathf.RoundToInt(localpos.y / prefabBounds.size.y), Mathf.RoundToInt(localpos.z / prefabBounds.size.z));
    }

    public List<Vector3> PossibleLocations(out List<Vector3Int> offsets)
    {

        List<Vector3Int> exist = new List<Vector3Int>();
        List<Vector3Int> possible = new List<Vector3Int>();

        exist.Add(Vector3Int.zero);


        for (int i = 0; i < transform.childCount; i++)
        {
            Transform c = transform.GetChild(i);
            Vector3 localpos = c.localPosition;
            exist.Add(intPosl(localpos));
        }
        foreach (Vector3Int pos in exist)
        {
            if (!(exist.Contains(Vector3Int.forward + pos) || exist.Contains(Vector3Int.forward + pos)))
            {
                possible.Add(Vector3Int.forward + pos);
            }
            if (!(exist.Contains(Vector3Int.back + pos) || exist.Contains(Vector3Int.back + pos)))
            {
                possible.Add(Vector3Int.back + pos);
            }
            if (!(exist.Contains(Vector3Int.right + pos) || exist.Contains(Vector3Int.right + pos)))
            {
                possible.Add(Vector3Int.right + pos);
            }
            if (!(exist.Contains(Vector3Int.left + pos) || exist.Contains(Vector3Int.left + pos)))
            {
                possible.Add(Vector3Int.left + pos);
            }
        }
        foreach(Vector3Int pos in possible)
        {
            if (exist.Contains(Vector3Int.forward + pos))
            {
                possible.Remove(Vector3Int.forward + pos);
            }
            if (exist.Contains(Vector3Int.back + pos))
            {
                possible.Remove(Vector3Int.back + pos);
            }
            if (exist.Contains(Vector3Int.right + pos))
            {
                possible.Remove(Vector3Int.right + pos);
            }
            if (exist.Contains(Vector3Int.left + pos))
            {
                possible.Remove(Vector3Int.left + pos);
            }
        }

        List<Vector3> positionsWorldCanBuild = new List<Vector3>();

        foreach (Vector3Int pos in possible)
        {
            Vector3 posWorld = new Vector3(pos.x * prefabBounds.size.x, pos.y * prefabBounds.size.y, pos.z * prefabBounds.size.z) + transform.position;
            posWorld = transform.rotation * posWorld;
            positionsWorldCanBuild.Add(posWorld);
        }
        offsets = new List<Vector3Int>();
        return positionsWorldCanBuild;
    }

    public void BuildAt(Vector3Int dir, Vector3 from)
    {
        Vector3 targetPos = from + transform.rotation * new Vector3(prefabBounds.size.x * dir.x, prefabBounds.size.y * dir.y, prefabBounds.size.z * dir.z);
        Instantiate(prefab, targetPos, transform.rotation, transform);
    }
    public void BuildAtWorld(Vector3 pos)
    {
        GameObject created = Instantiate(prefab, pos, transform.rotation, transform);
        created.name = gameObject.name + " " + pos.ToString();
    }

}



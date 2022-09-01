using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(Builder)), CanEditMultipleObjects]
public class BuilderEditor : Editor
{
    protected virtual void OnSceneGUI()
    {
        Builder builder = (Builder)target;

        if(builder.mode == Builder.BuilderMode.Preview)
        {
            return;
        }

        List<Vector3Int> offsets = new List<Vector3Int>();
        List<Vector3> positionsCanBuild = builder.PossibleLocations(out offsets);
        float size = builder.PreviewSize().extents.x;
        float pickSize = size;

        foreach(Vector3 pos in positionsCanBuild)
        {
            if (Handles.Button(pos, Quaternion.Euler(90,0,0) * builder.transform.rotation, size, pickSize, Handles.RectangleHandleCap))
            {
                Debug.Log("Built at " + pos);
                builder.BuildAtWorld(pos);
            }
        }
        
    }
}
#else
public class BuilderEditor
{
}
#endif
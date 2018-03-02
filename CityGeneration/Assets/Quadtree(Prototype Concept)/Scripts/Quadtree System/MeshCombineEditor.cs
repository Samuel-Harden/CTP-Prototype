using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof(MeshCombine))]
public class MeshCombineEditor : Editor
{
    void OnSceneGUI()
    {
        MeshCombine mc = target as MeshCombine;

        if (Handles.Button(mc.transform.position + Vector3.up * 5, Quaternion.LookRotation(Vector3.up), 1, 1, Handles.CylinderHandleCap))
        {
            mc.CombineMeshes();
        }
    }
}

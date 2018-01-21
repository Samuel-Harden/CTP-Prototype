using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeData : MonoBehaviour
{
    private int ID;


    public void SetColor(Color _color)
    {
        this.gameObject.GetComponent<MeshRenderer>().material.color = _color;
    }


    public Color GetColor()
    {
        return this.gameObject.GetComponent<MeshRenderer>().material.color;
    }


    public void SetID(int _ID)
    {
        ID = _ID;
    }


    public int GetID()
    {
        return ID;
    }
}

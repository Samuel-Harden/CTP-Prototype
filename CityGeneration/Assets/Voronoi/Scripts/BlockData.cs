using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockData : MonoBehaviour
{

    private int block_id;
    private int node_id;

    // Use this for initialization
    void Start()
    {

    }


    // Update is called once per frame
    void Update()
    {

    }


    public void SetID(int _id)
    {
        block_id = _id;
    }


    public void SetNodeID(int _node_id)
    {
        node_id = _node_id;
    }


    public int GetID()
    {
        return block_id;
    }


    public int GetNodeID()
    {
        return node_id;
    }

}

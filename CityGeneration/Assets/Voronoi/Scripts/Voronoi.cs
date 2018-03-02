using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voronoi : MonoBehaviour
{
    [SerializeField] int grid_width;
    [SerializeField] int grid_height;

    public float block_size;

    public GameObject tile;
    public GameObject node;
    public GameObject marker;

    [SerializeField] LayerMask node_mask;
    [SerializeField] LayerMask block_mask;

    private int[,] grid;
    private GameObject[,] chunks;

    [SerializeField] List<GameObject> Nodes;

    private List<Vector3> positions;

    // Use this for initialization
    void Start()
    {
        grid = new int[grid_width, grid_height];
        chunks = new GameObject[grid_width, grid_height];

        positions = new List<Vector3>();

        SetNodeIDs();

        GenerateBlocks();
        SetNearestNode();

        GenerateRoads();
    }


    void GenerateRoads()
    {
        for (int i = 0; i < Nodes.Count; i++)
        {
            for (int j = 0; j < Nodes.Count; j++)
            {
                if (Nodes[i].transform.position != Nodes[j].transform.position)
                {
                    bool check = false;

                    // The midpoint between the two zones
                    Vector3 pos = (Nodes[i].transform.position + Nodes[j].transform.position) / 2;

                    // if this check has alreaady been done
                    for (int k = 0; k < positions.Count; k++)
                    {
                        if (pos == positions[k])
                        {
                            check = true;
                        }
                    }

                    if (NodesShareBorders(i, j, pos) && check == false)
                    {
                        positions.Add(pos);

                        Vector3 heading = (Nodes[j].transform.position - Nodes[i].transform.position).normalized;

                        Instantiate(marker, pos, Quaternion.LookRotation(heading.normalized, Vector3.up));


                    }
                }
            }
        }
    }


    // pass in the two nodes we wish to check
    bool NodesShareBorders(int _node_one, int _node_two, Vector3 _pos)
    {
        float radius = 2.5f;

        Collider[] blocks = Physics.OverlapSphere(_pos, radius, block_mask);

        int node_one_id = Nodes[_node_one].GetComponent<NodeData>().GetID();
        int node_two_id = Nodes[_node_two].GetComponent<NodeData>().GetID();

        float closest = 50.0f;

        int nearest_block = 0;

        if (blocks.Length > 1)
        {
            for (int i = 0; i < blocks.Length; i++)
            {
                float current_block = Vector3.Distance(blocks[i].transform.position, _pos);

                if (current_block < closest)
                {
                    closest = current_block;
                    nearest_block = i;
                }
            }
        }

        // Now we have the nearest block to the position, check if its owned by either
        // of the comparision nodes
        if (blocks[nearest_block].GetComponent<BlockData>().GetNodeID() == node_one_id ||
                blocks[nearest_block].GetComponent<BlockData>().GetNodeID() == node_two_id)
        {
            return true;
        }

        return false;
    }


    void GenerateBlocks()
    {
        float pos_x = 0;
        float pos_z = 0;

        int ID = 0;

        for (int i = 0; i < grid_height; i++)
        {
            for (int j = 0; j < grid_width; j++)
            {
                // Accross, then up
                //if (grid[j, i] != 2)
                //{
                var chunk = Instantiate(tile, new Vector3(pos_x, 0, pos_z), Quaternion.identity);

                chunk.GetComponent<BlockData>().SetID(ID);
                ID++;

                chunks[j, i] = chunk;
                pos_x += block_size;
                //}
            }

            pos_x = 0;
            pos_z += block_size;
        }
    }


    void SetNearestNode()
    {
        for (int i = 0; i < grid_height; i++)
        {
            for (int j = 0; j < grid_width; j++)
            {
                float closest_node = 500.0f;

                int nearest_node = 0;

                for (int k = 0; k < Nodes.Count; k++)
                {
                    float dist_to_node = Vector3.Distance(chunks[j, i].transform.position, Nodes[k].transform.position);

                    if (dist_to_node < closest_node)
                    {
                        closest_node = dist_to_node;
                        nearest_node = k;
                    }
                }

                // Set its colour based on closest Node
                chunks[j, i].GetComponent<MeshRenderer>().material.color = Nodes[nearest_node].GetComponent<NodeData>().GetColor();

                // Set its node ID
                chunks[j, i].GetComponent<BlockData>().SetNodeID(Nodes[nearest_node].GetComponent<NodeData>().GetID());
            }
        }
    }


    void SetNodeIDs()
    {
        for (int i = 0; i < Nodes.Count; i++)
        {
            Nodes[i].GetComponent<NodeData>().SetID(i);
        }
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;

        /*foreach (Vector3 pos in positions)
        {
            Gizmos.DrawWireSphere(pos, 1);
        }*/
    }
}

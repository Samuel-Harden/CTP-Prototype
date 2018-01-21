using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    [SerializeField] bool gizmos_enabled;
    private Vector3 position;

    private float size_x;
    private float size_z;

    private bool divided;

    private List<GameObject> child_nodes;

    private int no_divisions = 4;
    private int no_build_depth = 3;

    private int divide_count;

    Vector3 bottom_left_pos;
    Vector3 bottom_right_pos;
    Vector3 top_left_pos;
    Vector3 top_right_pos;

    Vector3 new_pos;

    private void Start()
    {
        child_nodes = new List<GameObject>();
    }


    public void Initialise(Vector3 _position, float _size_x, float _size_z,
        List<Vector3> _positions, int _depth, GameObject _node, Transform _parent_node, List<Vector3> _junction_positions, float _road_offset, List<Node> _nodes, ObjectGenerator _object_gen, int _division)
    {
        //Debug.Log("Added Node");

        child_nodes = new List<GameObject>();

        size_x = _size_x;
        size_z = _size_z;

        position = _position;

        // Add all possible juction positions to list
        _junction_positions.Add(position); // Bottom Left
        _junction_positions.Add(new Vector3(position.x + size_x, 0, position.z)); // Bottom Right
        _junction_positions.Add(new Vector3(position.x, 0, position.z + size_z)); // Top Left
        _junction_positions.Add(new Vector3(position.x + size_x, 0, position.z + size_z)); // Top Right

        // Line up positions to size of node
        bottom_left_pos  = position;
        bottom_right_pos = new Vector3(position.x + size_x, 0, position.z);
        top_left_pos     = new Vector3(position.x, 0, position.z + size_z);
        top_right_pos    = new Vector3(position.x + size_x, 0, position.z + size_z);
    

        // Calculate Road Offset
        float offset_x = _road_offset;
        float offset_z = _road_offset;

        CreateOffSet(offset_z, offset_z);

        // Calculate Pavement Offset
        offset_x = Vector3.Distance(bottom_left_pos, bottom_right_pos) / 10;
        offset_z = Vector3.Distance(bottom_left_pos, top_left_pos) / 10;
    
        CreateOffSet(offset_x, offset_z);

        transform.parent = _parent_node.transform;

        if (_depth > 0)
        {
            // Check if this node needs spliting
            if(DivideCheck(_positions))
            {
                divided = true;
                _division++;
                Divide(_positions, _depth, _node, _parent_node, _junction_positions, _road_offset, _nodes, _object_gen, _division);
            }
        }


        // if this node hasn't been divided, and it deeper than the x division
        if (!divided && _division > no_build_depth)
            GenerateBuilding(_object_gen, _road_offset);

        else if (!divided && _division <= no_build_depth)
            GeneratePark(_object_gen, _road_offset);
    }


    private void CreateOffSet(float _offset_x, float _offset_z)
    {
        bottom_left_pos = new Vector3(bottom_left_pos.x + _offset_x, bottom_left_pos.y, bottom_left_pos.z + _offset_z);

        bottom_right_pos = new Vector3(bottom_right_pos.x -_offset_x, bottom_right_pos.y, bottom_right_pos.z + _offset_z);

        top_right_pos = new Vector3(top_right_pos.x - _offset_x, top_right_pos.y, top_right_pos.z - _offset_z);

        top_left_pos = new Vector3(top_left_pos.x + _offset_x, top_left_pos.y, top_left_pos.z - _offset_z);
    }


    bool DivideCheck(List<Vector3> _positions)
    {
        int count = 0;

        foreach(Vector3 pos in _positions)
        {
            // is this position within bounds of node
            if(pos.x >= position.x && pos.x < (position.x + size_x) &&
                pos.z >= position.z && pos.z < (position.z + size_z))
            {
                count++;
            }


            if(count >= divide_count)
            {
                return true;
            }
        }

        return false;
    }


    void Divide(List<Vector3> _positions, int _depth, GameObject _node, Transform _parent_node, List<Vector3> _junction_positions, float _road_offset, List<Node> _nodes, ObjectGenerator _object_gen, int _division)
    {
        // Each recursion this should go down one
        _depth -= 1;

        Vector3 new_position = position;

        float new_size_x = size_x / 2;
        float new_size_z = size_z / 2;

        int count = 0;

        // Order (Bottom left, Bottom right, Top left, Top right)
        for (int i = 0; i < no_divisions; i++)
        {
            var node_obj = Instantiate(_node, new_position, _node.transform.rotation);

            _nodes.Add(node_obj.GetComponent<Node>());

            child_nodes.Add(node_obj);

            node_obj.GetComponent<Node>().SetDivideCount(divide_count);

            node_obj.GetComponent<Node>().Initialise(new_position, new_size_x, new_size_z, _positions, _depth, _node, _parent_node.transform, _junction_positions, _road_offset, _nodes, _object_gen, _division);

            new_position.x += size_x / 2;

            count++;

            if (count > 1)
            {
                new_position.x = position.x;
                new_position.z += size_z / 2;
                count = 0;
            }
        }
    }


    /*
    public void AddFuzz()
    {
        // Take each corner and add 10% fuzz

        float fuzz_x = size_x / 10;
        float fuzz_z = size_z / 10;

        bottom_left_pos.x += Random.Range((bottom_left_pos.x - fuzz_x), (bottom_left_pos.x + fuzz_x));




        if (child_nodes.Count == 0)
            return;

        foreach(GameObject child in child_nodes)
        {
            child.GetComponent<Node>().AddFuzz();
        }
    }*/


    private void GenerateBuilding(ObjectGenerator _object_gen, float _road_offset)
    {
        float x = Vector3.Distance(bottom_left_pos, bottom_right_pos);
        float z = Vector3.Distance(bottom_left_pos, top_left_pos);

        new_pos = new Vector3(bottom_left_pos.x + (x / 2), 0, bottom_left_pos.z + (z / 2));

        var new_building = _object_gen.GenerateBuilding(new_pos, x, z);

        new_building.transform.parent = this.transform;
    }


    private void GeneratePark(ObjectGenerator _object_gen, float _road_offset)
    {
        float x = Vector3.Distance(bottom_left_pos, bottom_right_pos);
        float z = Vector3.Distance(bottom_left_pos, top_left_pos);

        new_pos = new Vector3(bottom_left_pos.x + (x / 2), 0, bottom_left_pos.z + (z / 2));

        var new_park = _object_gen.GeneratePark(new_pos, x, z);

        new_park.transform.parent = this.transform;
    }


    public void SetDivideCount(int _count)
    {
        divide_count = _count;
    }


    private void OnDrawGizmos()
    {
        if (gizmos_enabled)
        {
            /*Gizmos.color = Color.green;

            Gizmos.DrawWireSphere(new_pos, 1);
            // Bottom Left to Bottom Right
            Gizmos.DrawLine(position, new Vector3(position.x + size_x, 0, position.z));

            // Bottom Right to Top Right
            Gizmos.DrawLine(new Vector3(position.x + size_x, 0, position.z), new Vector3(position.x + size_x, 0, position.z + size_z));

            // Top Right to Top Left
            Gizmos.DrawLine(new Vector3(position.x + size_x, 0, position.z + size_z), new Vector3(position.x , 0, position.z + size_z));

            // Top Left to Bottom Left
            Gizmos.DrawLine(new Vector3(position.x, 0, position.z + size_z), position);*/

            // if this node has not been divided we have a building area...
            if (!divided)
            {
                Gizmos.color = Color.red;

                Gizmos.DrawLine(bottom_left_pos, bottom_right_pos);
                Gizmos.DrawLine(bottom_right_pos, top_right_pos);
                Gizmos.DrawLine(top_right_pos, top_left_pos);
                Gizmos.DrawLine(top_left_pos, bottom_left_pos);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    [SerializeField] bool gizmos_enabled;
    private Vector3 position;

    private float lotWidth;
    private float lotLength;

    private Vector3 nodeBoundsPos;

    private bool divided;

    private List<GameObject> child_nodes;

    private int no_divisions = 4;
    private int no_build_depth = 3;

    private int divide_count;

    // Used for building placement
    Vector3 bottom_left_pos;
    Vector3 bottom_right_pos;
    Vector3 top_left_pos;
    Vector3 top_right_pos;

    // Used for path placement
    Vector3 path_bottom_left_pos;
    Vector3 path_bottom_right_pos;
    Vector3 path_top_left_pos;
    Vector3 path_top_right_pos;

    Vector3 new_pos;

    private void Start()
    {
        child_nodes = new List<GameObject>();
    }


    public void Initialise(Vector3 _position, float _size_x, float _size_z,
        List<Vector3> _positions, int _depth, GameObject _node, Transform _parent_node,
        List<Vector3> _junction_positions, float _road_offset, List<Node> _nodes,
        ObjectGenerator _object_gen, int _division)
    {
        //Debug.Log("Added Node");

        child_nodes = new List<GameObject>();

        lotWidth = _size_x;
        lotLength = _size_z;

        position = _position;

        // Add all possible juction positions to list
        _junction_positions.Add(position); // Bottom Left
        _junction_positions.Add(new Vector3(position.x + lotWidth, 0, position.z)); // Bottom Right
        _junction_positions.Add(new Vector3(position.x, 0, position.z + lotLength)); // Top Left
        _junction_positions.Add(new Vector3(position.x + lotWidth, 0, position.z + lotLength)); // Top Right

        // Line up positions to size of node
        bottom_left_pos  = position;
        bottom_right_pos = new Vector3(position.x + lotWidth, 0, position.z);
        top_left_pos     = new Vector3(position.x, 0, position.z + lotLength);
        top_right_pos    = new Vector3(position.x + lotWidth, 0, position.z + lotLength);
    

        // Calculate Road Offset
        float offset_x = _road_offset;
        float offset_z = _road_offset;

        CreateOffSet(offset_z, offset_z); // factors in the road size

        // now mark area for pavement.....
        path_bottom_left_pos  = bottom_left_pos;
        path_bottom_right_pos = bottom_right_pos;
        path_top_left_pos     = top_left_pos;
        path_top_right_pos    = top_right_pos;

        // Calculate Pavement Offset
        //offset_x = Vector3.Distance(bottom_left_pos, bottom_right_pos) / 10;
        //offset_z = Vector3.Distance(bottom_left_pos, top_left_pos) / 10;
    
        //CreateOffSet(offset_x, offset_z); // factors in space for pavement

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
        //if (!divided && _division > no_build_depth)
            //GenerateBuilding(_object_gen, _road_offset);

        //else if (!divided && _division <= no_build_depth)
            //GeneratePark(_object_gen, _road_offset);
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
            if(pos.x >= position.x && pos.x < (position.x + lotWidth) &&
                pos.z >= position.z && pos.z < (position.z + lotLength))
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

        float new_size_x = lotWidth / 2;
        float new_size_z = lotLength / 2;

        int count = 0;

        // Order (Bottom left, Bottom right, Top left, Top right)
        for (int i = 0; i < no_divisions; i++)
        {
            var node_obj = Instantiate(_node, new_position, _node.transform.rotation);

            _nodes.Add(node_obj.GetComponent<Node>());

            child_nodes.Add(node_obj);

            node_obj.GetComponent<Node>().SetDivideCount(divide_count);

            node_obj.GetComponent<Node>().Initialise(new_position, new_size_x, new_size_z, _positions, _depth, _node, _parent_node.transform, _junction_positions, _road_offset, _nodes, _object_gen, _division);

            new_position.x += lotWidth / 2;

            count++;

            if (count > 1)
            {
                new_position.x = position.x;
                new_position.z += lotLength / 2;
                count = 0;
            }
        }
    }


    private void GenerateBuilding(ObjectGenerator _object_gen, float _road_offset)
    {
        float x = Vector3.Distance(bottom_left_pos, bottom_right_pos);
        float z = Vector3.Distance(bottom_left_pos, top_left_pos);

        new_pos = new Vector3(bottom_left_pos.x + (x / 2), 0, bottom_left_pos.z + (z / 2));

        var new_building = _object_gen.GenerateBuilding(new_pos, x, z);

        new_building.transform.parent = this.transform;

        GenerateSideWalk(_object_gen);
    }


    private void GeneratePark(ObjectGenerator _object_gen, float _road_offset)
    {
        float x = Vector3.Distance(bottom_left_pos, bottom_right_pos);
        float z = Vector3.Distance(bottom_left_pos, top_left_pos);

        new_pos = new Vector3(bottom_left_pos.x + (x / 2), 0, bottom_left_pos.z + (z / 2));

        var new_park = _object_gen.GeneratePark(new_pos, x, z);

        new_park.transform.parent = this.transform;

        GenerateSideWalk(_object_gen);
    }


    private void GenerateSideWalk(ObjectGenerator _object_gen)
    {
        float x = Vector3.Distance(path_bottom_left_pos, path_bottom_right_pos);
        float z = Vector3.Distance(path_bottom_left_pos, path_top_left_pos);

        new_pos = new Vector3(path_bottom_left_pos.x + (x / 2), 0, path_bottom_left_pos.z + (z / 2));

        var sidewalk = _object_gen.GenerateSideWalk(new_pos, x, z);

        sidewalk.transform.parent = this.transform;
    }


    public void SetDivideCount(int _count)
    {
        divide_count = _count;
    }


    public bool Divided()
    {
        return divided;
    }


    public Vector3 LotBounds()
    {
        return new Vector3(transform.position.x - lotWidth / 2, 0.0f,
            transform.position.z - lotLength / 2); ;
    }


    public float Width()
    {
        return lotWidth;
    }


    public float Length()
    {
        return lotLength;
    }


    private void OnDrawGizmos()
    {
        if (gizmos_enabled)
        {
            Gizmos.color = Color.green;

            Gizmos.DrawWireSphere(new_pos, 1);
            // Bottom Left to Bottom Right
            Gizmos.DrawLine(position, new Vector3(position.x + lotWidth, 0, position.z));

            // Bottom Right to Top Right
            Gizmos.DrawLine(new Vector3(position.x + lotWidth, 0, position.z), new Vector3(position.x + lotWidth, 0, position.z + lotLength));

            // Top Right to Top Left
            Gizmos.DrawLine(new Vector3(position.x + lotWidth, 0, position.z + lotLength), new Vector3(position.x , 0, position.z + lotLength));

            // Top Left to Bottom Left
            Gizmos.DrawLine(new Vector3(position.x, 0, position.z + lotLength), position);

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

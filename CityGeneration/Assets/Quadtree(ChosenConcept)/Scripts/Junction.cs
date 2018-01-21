using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Junction : MonoBehaviour
{
    [SerializeField] LayerMask check_layers;

    private bool road_forward_valid = false;
    private bool road_right_valid = false;

    private Vector3 road_forward_pos;
    private Vector3 road_right_pos;

    private float grid_size_x;
    private float grid_size_z;

    private float scale_x;
    private float scale_z;

    [SerializeField] GameObject road_section;



    public void Initalise(float _grid_width, float _grid_height)
    {
        grid_size_x = _grid_width;
        grid_size_z = _grid_height;

        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
    }


    public void GenerateRoads()
    {
        // Check roads are ok
        CheckRoadPlacement();

        if (road_forward_valid)
        {
            // Create Road
            var road = Instantiate(road_section, road_forward_pos, Quaternion.identity);

            road.transform.localScale = new Vector3(road.transform.localScale.x, road.transform.localScale.y, scale_z);

            road.transform.parent = transform;
        }

        if (road_right_valid)
        {
            // Create Road
            var road = Instantiate(road_section, road_right_pos, Quaternion.identity);

            road.transform.localScale = new Vector3(road.transform.localScale.x, road.transform.localScale.y, scale_x);

            Vector3 rot = road.transform.rotation.eulerAngles;
            rot.y += 90.0f;

            road.transform.rotation = Quaternion.Euler(rot);

            road.transform.parent = transform;
        }
    }


    private void CheckRoadPlacement()
    {
        // Raycast up
        // set bool
        // Set road_up_pos = pos.x + hald the distance to next junction
        if (transform.position.z < grid_size_z)
        {
            RaycastHit hit_forward;
            if (Physics.Raycast(transform.position, Vector3.forward, out hit_forward, grid_size_z, check_layers))
            {
                if (hit_forward.collider.gameObject.layer == LayerMask.NameToLayer("Junction - Quadtree"))
                {
                    road_forward_valid = true;

                    float distance = Vector3.Distance(transform.position, hit_forward.transform.position);

                    float mid_point = distance / 2;

                    scale_z = distance - gameObject.transform.localScale.z;

                    road_forward_pos = new Vector3(transform.position.x, transform.position.y, transform.position.z + mid_point);

                    Debug.Log("Hit Junction forward");
                }
            }
        }

        // Raycast right
        // set bool
        // Set road_right_pos = pos.x + hald the distance to next junction
        if (transform.position.x < grid_size_x)
        {
            RaycastHit hit_right;
            if (Physics.Raycast(transform.position, Vector3.right, out hit_right, grid_size_x, check_layers))
            {
                if (hit_right.collider.gameObject.layer == LayerMask.NameToLayer("Junction - Quadtree"))
                {
                    road_right_valid = true;

                    float distance = Vector3.Distance(transform.position, hit_right.transform.position);

                    float mid_point = distance / 2;

                    scale_x = distance - gameObject.transform.localScale.x;

                    road_right_pos = new Vector3(transform.position.x + mid_point, transform.position.y, transform.position.z);

                    Debug.Log("Hit Junction Right");
                }
            }
        }
    }
}


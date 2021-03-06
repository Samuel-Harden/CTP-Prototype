﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuadtreeCityGen : MonoBehaviour
{
    [SerializeField] int cityZ;
    [SerializeField] int cityX;
    [SerializeField] int max_depth;

    [SerializeField] int divide_count = 2;
    [SerializeField] int perlin_noise = 10;

    //[SerializeField] int no_positions = 10;

    [SerializeField] bool show_positions;

    [SerializeField] GameObject node;
    [SerializeField] GameObject node_parent;
    [SerializeField] GameObject junction;
    [SerializeField] GameObject junction_parent;

    private PerlinPopGen pop_gen;
    private TileGenQT tileGen;

    private float road_offset;

    private List<Vector3> positions;
    private List<Vector3> new_positions;

    private List<Vector3> junction_positions;
    private List<Vector3> clean_junction_positions;

    private List<Node> nodes;
    private List<Junction> junctions;


    void Start ()
    {
        positions = new List<Vector3>();
        new_positions = new List<Vector3>();
        junction_positions = new List<Vector3>();
        clean_junction_positions = new List<Vector3>();
        nodes = new List<Node>();
        junctions = new List<Junction>();

        SanityCheckInitialSettings();

        // Set the road offset to half a junctions scale
        road_offset = junction.transform.localScale.x / 2;

        pop_gen = GetComponent<PerlinPopGen>();
        tileGen = GetComponent<TileGenQT>();

        pop_gen.SetPerlinNoise(perlin_noise);

        GeneratePositions();

        GenerateInitialNode();
	}


    void GeneratePositions()
    {
        pop_gen.GeneratePopData(cityX, cityZ, positions);

        for (int i = 0; i < positions.Count; i++)
        {
            int pop = pop_gen.GetPop(i);

            if(pop > 90) // Guranteed position
            {
                new_positions.Add(positions[i]);
            }

            else if (pop > 80)
            {
                //75% CHANCE
                if(Random.Range(0, 100) > 25)
                {
                    new_positions.Add(positions[i]);
                }
            }

            else if (pop > 70)
            {
                //50% CHANCE
                if (Random.Range(0, 100) > 50)
                {
                    new_positions.Add(positions[i]);
                }
            }

            else if (pop > 60)
            {
                //25% CHANCE
                if (Random.Range(0, 100) > 75)
                {
                    new_positions.Add(positions[i]);
                }
            }
        }

    }


    void GenerateInitialNode()
    {
        Vector3 pos  = Vector3.zero;
        float size_x = cityX;
        float size_z = cityZ;

        var node_obj = Instantiate(node, pos, node.transform.rotation);

        nodes.Add(node_obj.GetComponent<Node>());

        var build_gen = this.gameObject.GetComponent<ObjectGenerator>();

        node_obj.GetComponent<Node>().SetDivideCount(divide_count);

        node_obj.GetComponent<Node>().Initialise(Vector3.zero,
            size_x, size_z, new_positions, max_depth, node, node_parent.transform,
            junction_positions, road_offset, nodes, build_gen, 0);

        clean_junction_positions = junction_positions.Distinct().ToList();

        /*for(int i = 0; i < clean_junction_positions.Count; i++)
        {
            var crossing = Instantiate(junction, clean_junction_positions[i], Quaternion.identity);

            junctions.Add(crossing.GetComponent<Junction>());

            crossing.GetComponent<Junction>().Initalise(cityX, cityZ);

            crossing.transform.parent = junction_parent.transform;
        }*/

        ClearNodes();

        tileGen.Initialise(nodes, cityX, cityZ);

        //SetRoads();
    }


    void SetRoads()
    {
        foreach(Junction junction in junctions)
        {
            junction.GenerateRoads();
        }
    }


    void ClearNodes()
    {
        Debug.Log(nodes.Count);

        for (int i = nodes.Count - 1; i >= 0; i--)
        {
            if(nodes[i].Divided())
            {
                nodes.RemoveAt(i);
                Debug.Log("Cleared Node");
            }
        }

        Debug.Log(nodes.Count);
    }


    void SanityCheckInitialSettings()
    {
        // Check X + Z are at least half to avoid weird stuff!
        if(cityZ < cityX / 2)
        {
            cityZ = cityX / 2;
        }

        else if (cityX < cityZ / 2)
        {
            cityX = cityZ / 2;
        }

        // May need more Sanity checks here later...
    }


    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;

        if (!show_positions)
            return;

        Gizmos.color = Color.white;

        foreach (Vector3 pos in new_positions)
        {
            Gizmos.DrawWireSphere(pos, 1);
        }
    }
}

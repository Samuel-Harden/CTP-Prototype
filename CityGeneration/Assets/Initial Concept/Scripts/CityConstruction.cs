using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityConstruction : MonoBehaviour
{
    // Building array
    public GameObject[] buildings;

    // Road Peices
    public GameObject roadX;
    public GameObject roadZ;
    public GameObject crossX;

    // City Size
    public int mapWidth = 20;
    public int mapHeight = 20;

    public int maxBlockLength = 8;

    // Spacing between Grid spaces
    private float BuildingSpacing = 1.25f;

    // Perlin Noise Data
    private int[,] mapGrid;

    private void Start()
    {
        // Setup the Mapgrid
        mapGrid = new int[mapWidth, mapHeight];

        GenerateBuildingData();

        GenerateRoadData();

        PopulateRoads();

        PopulateBuildings();
    }



    private void GenerateBuildingData()
    {
        // needs to be Seeded, else same map!
        int seed = Random.Range(0, 100);

        // Generate Map Data
        for (int h = 0; h < mapHeight; h++)
        {
            for (int w = 0; w < mapWidth; w++)
            {
                // Populate mapGrid with Perlin Values, representing
                // which building will be used at this position
                mapGrid[w, h] = (int)(Mathf.PerlinNoise(w / 10.0f + seed, h / 10.0f + seed) * 10);
            }
        }
    }



    private void GenerateRoadData()
    {
        // X Axis (Right)
        int x = 0;
        for (int n = 0; n < mapHeight; n++)
        {
            for (int h = 0; h < mapHeight; h++)
            {
                // Set this position as a Horizontal Road
                mapGrid[x, h] = -1;
            }

            x += 3;

            // If weve gone past the end of the grid
            if (x >= mapWidth)
                break;
        }

        // Z Axis (Up)
        int z = 0;
        for (int n = 0; n < mapWidth; n++)
        {
            for (int w = 0; w < mapWidth; w++)
            {
                // if its already a road from the Z axis,
                // update to a cross section
                if (mapGrid[w, z] == -1)
                {
                    mapGrid[w, z] = -3;
                }

                // otherwise its just a road on the Z axis
                else
                    mapGrid[w, z] = -2;
            }

            // Use a Random Range to increase and Vary block Lengths
            z += Random.Range(3, maxBlockLength);

            // If weve gone past the end of the grid
            if (z >= mapHeight)
                break;
        }
    }



    private void PopulateBuildings()
    {
        for (int h = 0; h < mapHeight; h++)
        {
            for (int w = 0; w < mapWidth; w++)
            {
                int gridID = mapGrid[w, h];

                Vector3 pos = new Vector3(w * BuildingSpacing, 0, h * BuildingSpacing);

                int buildingNo = 0;

                pos.y += 0.5f;

                // 10 because of they Way im using Perlin Noise...
                // This is checking the no. stored in the mapGrid
                // Using this number it decided which building to spawn
                // at this location...
                for (int i = 2; i <= 10; i++, i++)
                {
                    if(buildingNo == 0)
                    {
                        pos.y = 0.5f;
                    }

                    if (gridID < i && gridID >= 0)
                    {
                        // Grass Check
                        if(buildingNo == 4)
                        {
                            pos.y = 0.025f;
                        }

                        CreateBuilding(buildingNo, pos);

                        // Reset Building No and Height
                        buildingNo = 0;
                        pos.y = 0.5f;
                        break;
                    }

                    buildingNo++;
                    pos.y += 0.25f;
                }
            }
        }
    }



    private void PopulateRoads()
    {
        // Cycle through grid and create the roads
        for (int h = 0; h < mapHeight; h++)
        {
            for (int w = 0; w < mapWidth; w++)
            {
                int gridID = mapGrid[w, h];

                Vector3 pos = new Vector3(w * BuildingSpacing,
                    0, h * BuildingSpacing);

                // Road offset so it sits on the floor
                pos.y = 0.025f;

                if (gridID < -2)
                {
                    CreateRoad(crossX, pos);
                }
                else if (gridID < -1)
                {
                    CreateRoad(roadX, pos);
                }
                else if (gridID < 0)
                {
                    CreateRoad(roadZ, pos);
                }
            }
        }
    }



    private void CreateBuilding(int buildingNo, Vector3 pos)
    {
        Instantiate(buildings[buildingNo], pos, Quaternion.identity);
    }



    private void CreateRoad(GameObject roadType, Vector3 pos)
    {
        Instantiate(roadType, pos, roadType.transform.rotation);
    }
}
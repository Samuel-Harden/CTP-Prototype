using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGen : MonoBehaviour
{
    private int map_width = 512;
    private int map_height = 512;
    [SerializeField] int map_depth = 20;

    [SerializeField] float perlin_noise = 10.0f;

    [SerializeField] float water_density = 30;
    [SerializeField] float flatlands_density = 60;
    [SerializeField] float highland_density = 10;

    private float min_map_height = 0.0f;
    [SerializeField] float map_height_step = 0.2f;

    public PaintTerrain paint_terrain;

    private int water;
    private int flatlands;
    private int highlands;

    private float[,] heights;
    private float[,] map_heights;

    private List<int> map_percentages;

    // Use this for initialization
    void Start()
    {
        MapDimentionsCheck();

        map_percentages = new List<int>();

        CheckNoise();

        Terrain terrain = GetComponent<Terrain>();
        terrain.terrainData = GenerateTerrain(terrain.terrainData);

        paint_terrain.ColorTerrain(terrain.terrainData);
    }


    void MapDimentionsCheck()
    {
        if (map_width < 256)
        {
            map_width = 256;
        }

        if (map_height < 256)
        {
            map_height = 256;
        }
    }


    private void CheckNoise()
    {
        if (perlin_noise < 10)
            perlin_noise = 10;

        else if (perlin_noise > 200)
            perlin_noise = 200;
    }


    TerrainData GenerateTerrain(TerrainData terrainData)
    {
        terrainData.heightmapResolution = map_width + 1;

        terrainData.size = new Vector3(map_width, map_depth, map_height);

        heights = new float[map_width, map_height];
        map_heights = new float[map_width, map_height];

        SetPercentages();

        GenerateHeights();

        terrainData.SetHeights(0, 0, heights);

        return terrainData;
    }


    private void SetPercentages()
    {
        float total = water_density + flatlands_density + highland_density;

        water = Mathf.RoundToInt(water_density / total * 88);
        flatlands = Mathf.RoundToInt(flatlands_density / total * 88);
        highlands = Mathf.RoundToInt(highland_density / total * 88);

        // these get added and incremented
        map_percentages.Add(water);
        map_percentages.Add(water + 6);
        map_percentages.Add(water + 6 + flatlands);
        map_percentages.Add(water + 6 + flatlands + 6);
    }


    private void GenerateHeights()
    {
        float seed = Random.Range(0, 100);

        for (int w = 0; w < map_width; w++)
        {
            for (int h = 0; h < map_height; h++)
            {
                map_heights[w, h] =  (float)(Mathf.PerlinNoise(w / perlin_noise + seed, h / perlin_noise + seed) * 100.0f);

                SetHeights(w, h, map_heights[w, h], heights);
            }
        }
    }


    private void SetHeights(int pos_x, int pos_y, float _height, float[,] _heights)
    {

        // SeaBed
        if (_height <= map_percentages[0])
        {
            _heights[pos_x, pos_y] = min_map_height;
            return;
        }

        // Connection from SeaBed - Flatland
        if (_height > map_percentages[0] && _height <= map_percentages[1])
        {
            _heights[pos_x, pos_y] = map_height_step * 1;
            return;
        }

        // Flatlands
        if (_height > map_percentages[1] && _height <= map_percentages[2])
        {
            _heights[pos_x, pos_y] = map_height_step * 2;
            return;
        }

        // Connection from Flatlands - Highlands
        if (_height > map_percentages[2] && _height <= map_percentages[3])
        {
            _heights[pos_x, pos_y] = map_height_step * 3;
            return;
        }


        int remaining_split = 100 - map_percentages[3];

        //Mountains
        if (_height > map_percentages[3] && _height <= map_percentages[3] + remaining_split / 4 )
        {
            _heights[pos_x, pos_y] = map_height_step * 5;
            return;
        }

        if (_height > map_percentages[3] && _height <= map_percentages[3] + remaining_split / 2)
        {
            _heights[pos_x, pos_y] = map_height_step * 7;
            return;
        }

        if (_height > map_percentages[3] && _height <= map_percentages[3] + remaining_split / 4 * 3)
        {
            _heights[pos_x, pos_y] = map_height_step * 9;
            return;
        }

        else
        {
            _heights[pos_x, pos_y] = map_height_step * 11;
            return;
        }
    }
}


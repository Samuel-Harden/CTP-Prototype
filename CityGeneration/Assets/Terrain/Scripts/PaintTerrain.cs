using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintTerrain : MonoBehaviour
{
    [System.Serializable]
    public class SplatHeights
    {
        public int texture_index;
        public float starting_height;
    }

    public SplatHeights[] splat_heights;

    // Use this for initialization
    public void ColorTerrain(TerrainData terrain_data)
    {
        float[,,] splat_map_data = new float[terrain_data.alphamapWidth, terrain_data.alphamapHeight, terrain_data.alphamapLayers];

        for (int y = 0; y < terrain_data.alphamapHeight; y++)
        {
            for (int x = 0; x < terrain_data.alphamapWidth; x++)
            {
                float terrainHeight = terrain_data.GetHeight(y, x);

                float[] splat = new float[splat_heights.Length];

                for (int i = 0; i < splat_heights.Length; i++)
                {
                    if (i == splat_heights.Length - 1 && terrainHeight >= splat_heights[i].starting_height)
                    {
                        splat[i] = 1;
                    }

                    else if (terrainHeight >= splat_heights[i].starting_height && terrainHeight <= splat_heights[i+1].starting_height)
                    {
                        splat[i] = 1;
                    }
                }

                for (int j = 0; j < splat_heights.Length; j++)
                {
                    splat_map_data[x, y, j] = splat[j];
                }
            }
        }

        terrain_data.SetAlphamaps(0, 0, splat_map_data);
    }
}

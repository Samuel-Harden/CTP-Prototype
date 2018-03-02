using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinPopGen : MonoBehaviour
{
    private List<int> population;

    private float perlin_noise = 50;


    public void GeneratePopData(int _width, int _height, List<Vector3> _positions)
    {
        population = new List<int>();

        float seed = Random.Range(0, 100);

        for (int h = 0; h < _height; h++)
        {
            for (int w = 0; w < _width; w++)
            {
                int result = (int)(Mathf.PerlinNoise(w / perlin_noise + seed, h / perlin_noise + seed) * 100);

                population.Add(result);

                Vector3 pos = new Vector3(w, 0, h);
                _positions.Add(pos);
            }
        }

        //return population;
    }


    public void SetPerlinNoise(int _noise)
    {
        perlin_noise = _noise;
    }


    public int GetPop(int _pos)
    {
        return population[_pos];
    }
}

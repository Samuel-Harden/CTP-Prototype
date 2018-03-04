using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGenQT : MonoBehaviour
{
    [SerializeField] GameObject tilePrefab;

    private int cityX;
    private int cityZ;

    private Tile[,] cityMap;

    private int roadSize = 20;

    public void Initialise(List<Node> _nodes, int _cityX, int _cityZ)
    {
        cityX = _cityX;
        cityZ = _cityZ;

        GenerateRoadMap(_nodes);
    }


    private void GenerateRoadMap(List<Node> _nodes)
    {
        cityMap = new Tile[cityZ, cityX];

        //foreach (Node node in _nodes)
        //{
            int posX = (int)_nodes[0].transform.position.x + roadSize;
            int posZ = (int)_nodes[0].transform.position.z + roadSize;

            for (int l = 0; l < _nodes[0].Length() / roadSize; l++)
            {
                for (int w = 0; w < _nodes[0].Width() / roadSize; w++)
                {

                    cityMap[posZ, posX] = CreateTile((float)posX, (float)posZ);

                    posX += roadSize;
                }

                posX = (int)_nodes[0].transform.position.x;
                posZ += roadSize;
            }
        //}
    }


    private Tile CreateTile(float _posX, float _posZ)
    {
        var tile = Instantiate(tilePrefab, new Vector3(_posX, 0, _posZ),
            tilePrefab.transform.rotation);

        //tile.GetComponent<Tile>().SetData(_posX, _posZ);

        return tile.GetComponent<Tile>();
    }
}

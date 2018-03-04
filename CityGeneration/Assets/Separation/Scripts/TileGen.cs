using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGen : MonoBehaviour
{
    [SerializeField] GameObject tilePrefab;

    private int minPosX;
    private int minPosZ;
    private int maxPosX;
    private int maxPosZ;

    private int cityWidth;
    private int cityLength;

    private Tile[,] cityMap;

    private int tileSize = 1;


    public void Initialise(List<BuildingLot> _lots, Vector3 _cityCentre)
    {
        GetCityLimits(_lots);

        GenerateTileMap(_lots);
    }


    private void GenerateTileMap(List<BuildingLot> _lots)
    {
        cityWidth = maxPosX - minPosX / tileSize;
        cityLength = maxPosZ - minPosZ / tileSize;

        cityMap = new Tile[cityLength, cityWidth];

        foreach (BuildingLot lot in _lots)
        {
            int posX = (int)lot.LotBounds().x - minPosX;
            int posZ = (int)lot.LotBounds().z - minPosZ;

            for (int l = 0; l < lot.Length(); l++)
            {
                for (int w = 0; w < lot.Width(); w++)
                {
                    bool isRoad = false;

                    if (l == 0 || l == lot.Length() - 1 || w == 0 || w == lot.Width() - 1)
                        isRoad = true;

                    cityMap[posZ, posX] = CreateTile((float)posX + minPosX + (float)tileSize / 2, (float)posZ + minPosZ + (float)tileSize / 2, isRoad);

                    posX += tileSize;
                }

                posX = (int)lot.LotBounds().x - minPosX;
                posZ += tileSize;
            }
        }
    }


    private Tile CreateTile(float _posX, float _posZ, bool _isRoad)
    {
        var tile = Instantiate(tilePrefab, new Vector3(_posX, 0, _posZ),
            tilePrefab.transform.rotation);

        if (_isRoad)
            tile.GetComponent<Renderer>().material.color = Color.black;

        //tile.GetComponent<Tile>().SetData(_posX, _posZ);

        return tile.GetComponent<Tile>();
    }


    private void GetCityLimits(List<BuildingLot> _lots)
    {
        SetLowestXPos(_lots);
        SetHighestXPos(_lots);
        SetLowestZPos(_lots);
        SetHighestZPos(_lots);
    }


    private void SetLowestXPos(List<BuildingLot> _lots)
    {
        // Find Lowest X position
        foreach (BuildingLot lot in _lots)
        {
            int pos = (int)lot.LotBounds().x;

            if (pos < minPosX)
                minPosX = pos;
        }
    }


    private void SetHighestXPos(List<BuildingLot> _lots)
    {
        // Find highest X position
        foreach (BuildingLot lot in _lots)
        {
            int pos = (int)lot.LotBounds().x + (int)lot.Width();

            if (pos > maxPosX)
                maxPosX = pos;
        }
    }


    private void SetLowestZPos(List<BuildingLot> _lots)
    {
        // Find Lowest Z position
        foreach (BuildingLot lot in _lots)
        {
            int pos = (int)lot.LotBounds().z;

            if (pos < minPosZ)
                minPosZ = pos;
        }
    }


    private void SetHighestZPos(List<BuildingLot> _lots)
    {
        // Find highest Z position
        foreach (BuildingLot lot in _lots)
        {
            int pos = (int)lot.LotBounds().z + (int)lot.Length();

            if (pos > maxPosZ)
                maxPosZ = pos;
        }
    }
}

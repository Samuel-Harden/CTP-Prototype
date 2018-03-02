using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LotGen : MonoBehaviour
{
    [SerializeField] GameObject buildingLot;

    private int lotCount = 0;

    // Lot Amounts
    private int noLargeLots;
    private int currentLarge = 0;
    private int noMediumLots;
    private int currentMedium = 0;
    private int noSmallLots;
    private int currentSmall = 0;

    public void GenerateLots(List<BuildingLot> _lots, int _lotCount, int _cityRadius,
        bool _randNoLots)
    {
        lotCount = _lotCount;

        //SetLotCount(_randNoLots);

        GenerateNoLots(); // For each Size

        Vector3 dungeonCentre = new Vector3((float)_cityRadius / 2, 0.0f,
            (float)_cityRadius / 2);

        int counter = 0;

        Color color = Color.white;

        for (int i = 0; i < lotCount; i++)
        {
            float angle = Random.Range(0, 360);

            Quaternion direction = Quaternion.AngleAxis(angle, Vector3.up);

            Vector3 targetPos = direction * transform.forward *
                Random.Range(0, (_cityRadius / 4));

            Vector3 newPos = dungeonCentre + targetPos;

            newPos = new Vector3(Mathf.RoundToInt(newPos.x),
                0.0f, (Mathf.RoundToInt(newPos.z)));

            Vector2 roomSize = GenerateLotSize(_cityRadius);

            var newRoom = Instantiate(buildingLot, newPos, Quaternion.identity);

            newRoom.transform.localScale = new Vector3(roomSize.x, 1.0f, roomSize.y);

            _lots.Add(newRoom.GetComponent<BuildingLot>());

            // Need to add in height (No of floors for building)
            newRoom.GetComponent<BuildingLot>().Initialise((int)roomSize.x, (int)roomSize.y, 0, newPos, counter, i);

            //newRoom.GetComponent<Renderer>().material.color = color;


            if (i == noSmallLots)
            {
                counter++;
                color = Color.grey;
            }

            else if (i == noSmallLots + noMediumLots)
            {
                counter++;
                color = Color.black;
            }
        }
    }


    private Vector2 GenerateLotSize(int _cityRadius)
    {
        float smallest_size = (float)_cityRadius / 100;
        float largest_size = (float)_cityRadius / 100;

        if (currentSmall <= noSmallLots)
        {
            smallest_size *= 2;
            largest_size *= 4;

            currentSmall++;
            goto setSize;
        }

        if (currentMedium <= noMediumLots)
        {
            smallest_size *= 4;
            largest_size *= 6;

            currentMedium++;
            goto setSize;
        }

        if (currentLarge <= noLargeLots)
        {
            smallest_size *= 6;
            largest_size *= 10;

            currentLarge++;
            goto setSize;
        }

        setSize:

        Vector2 size = new Vector2(Mathf.RoundToInt(Random.Range(smallest_size, largest_size)) * 2,
            Mathf.RoundToInt(Random.Range(smallest_size, largest_size)) * 2);

        return size;
    }


    private void GenerateNoLots()
    {
        float largeRoomDensity = 4;
        float mediumRoomDensity = 20;
        float smallRoomDensity = 76;

        float total = largeRoomDensity + mediumRoomDensity + smallRoomDensity;

        noLargeLots  = Mathf.RoundToInt(largeRoomDensity   / total * lotCount);
        noMediumLots = Mathf.RoundToInt(mediumRoomDensity  / total * lotCount);
        noSmallLots  = Mathf.RoundToInt(smallRoomDensity   / total * lotCount);
    }
}

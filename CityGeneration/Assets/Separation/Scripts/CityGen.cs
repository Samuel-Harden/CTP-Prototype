using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CityGen : MonoBehaviour
{
    References referencer;

    [SerializeField] private List<BuildingLot> lots;

    private bool generateCity;
    [SerializeField] bool drawPositions;

    [SerializeField] int cityRadius = 300;
    [SerializeField] int noLots =  200;

    private Vector3 cityCentre;

    private bool setupComplete;

    private List<Vector3> intersectionPositions;


    private void Start()
    {
        lots = new List<BuildingLot>();
        intersectionPositions = new List<Vector3>();

        referencer = GetComponent<References>();

        referencer.LotGen().GenerateLots(lots, noLots, cityRadius, false);

        generateCity = true;
        setupComplete = false;
    }


    private void Update()
    {
        if(!setupComplete)
        {
            if (generateCity)
            {
                bool overlap = false;

                foreach (BuildingLot lot in lots)
                {
                    lot.Separation().CheckSpacing(lots, cityRadius);
                }

                foreach (BuildingLot lot in lots)
                {
                    if (lot.Separation().IsOverlapping())
                    {
                        overlap = true;
                        break;
                    }
                }


                if (!overlap)
                {
                    // Line up lots onto grid
                    foreach (BuildingLot lot in lots)
                    {
                        lot.Separation().SetPos();
                    }

                    // check that there is still no overlap
                    foreach (BuildingLot lot in lots)
                    {
                        if (lot.Separation().IsOverlapping())
                        {
                            overlap = true;
                            break;
                        }
                    }

                    // if there is no overlap and all lots are lined up 
                    if (!overlap)
                    {
                        // start setting up city properly!
                        cityCentre = SetCityCentre();

                        lots = lots.OrderBy(x => Vector3.Distance(cityCentre, x.transform.position)).ToList();

                        referencer.PathGen().ConnectLots(lots, cityCentre, cityRadius);

                        referencer.TileGen().Initialise(lots, cityCentre);

                        GenerateIntersections();

                        setupComplete = true;
                    }
                }
            }
        }
        
    }


    Vector3 SetCityCentre()
    {
        float minPosX = cityRadius / 2;
        float maxPosX = cityRadius / 2;
        float minPosZ = cityRadius / 2;
        float maxPosZ = cityRadius / 2;

        foreach (BuildingLot lot in lots)
        {
            if (lot.LotBounds().x < minPosX)
                minPosX = lot.LotBounds().x;

            if (lot.LotBounds().x + lot.Width() > maxPosX)
                maxPosX = lot.LotBounds().x + lot.Width();

            if (lot.LotBounds().z < minPosZ)
                minPosZ = lot.LotBounds().z;

            if (lot.LotBounds().z + lot.Length() > maxPosZ)
                maxPosZ = lot.LotBounds().z + lot.Length();
        }

        Vector3 centre = new Vector3(Mathf.Abs(minPosX + maxPosX) / 2, 0.0f, Mathf.Abs(minPosZ + maxPosZ) / 2);

        return centre;
    }


    private void GenerateIntersections()
    {
        foreach (BuildingLot lot in lots)
        {
            intersectionPositions.Add(lot.Separation().GetTopLeftPos());
            intersectionPositions.Add(lot.Separation().GetTopRightPos());
            intersectionPositions.Add(lot.Separation().GetBottomLeftPos());
            intersectionPositions.Add(lot.Separation().GetBottomRightPos());
        }

        intersectionPositions = intersectionPositions.Distinct().ToList();
    }


    private void OnDrawGizmos()
    {
        if(cityCentre != Vector3.zero)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(cityCentre, 1.0f);
        }

        if(drawPositions)
        {
            foreach (Vector3 pos in intersectionPositions)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(pos, 1.0f);
            }
        }
    }
}

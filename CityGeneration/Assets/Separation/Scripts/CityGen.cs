using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CityGen : MonoBehaviour
{
    References references;

    [SerializeField] private List<BuildingLot> lots;

    private bool generateCity;

    private int cityRadius = 50;
    private int noLots =  100;

    private Vector3 cityCentre;

    private bool setupComplete;

    private void Start()
    {
        lots = new List<BuildingLot>();

        references = GetComponent<References>();

        references.LotGen().GenerateLots(lots, noLots, cityRadius, false);

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
                    lot.Separation().CheckSpacing(lots);
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
                        if (!setupComplete)
                        {
                            // start setting up city properly!
                            cityCentre = SetCityCentre();

                            // some lots are not connected to main,
                            // sort list of lots by distance to city centre,
                            // then loop through all to see if they can be moved closer on the x and or z axis (1 at a time)
                            // until they cannot be any closer

                            lots = lots.OrderBy(x => Vector3.Distance(cityCentre, x.transform.position)).ToList();

                            references.PathGen().ConnectLots(lots, cityCentre);

                            setupComplete = true;
                        }
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
            if (lot.Separation().LotBounds().x < minPosX)
                minPosX = lot.Separation().LotBounds().x;

            if (lot.Separation().LotBounds().x + lot.Separation().Width() > maxPosX)
                maxPosX = lot.Separation().LotBounds().x + lot.Separation().Width();

            if (lot.Separation().LotBounds().z < minPosZ)
                minPosZ = lot.Separation().LotBounds().z;

            if (lot.Separation().LotBounds().z + lot.Separation().Length() > maxPosZ)
                maxPosZ = lot.Separation().LotBounds().z + lot.Separation().Length();
        }

        Vector3 centre = new Vector3(Mathf.Abs(minPosX + maxPosX) / 2, 0.0f, Mathf.Abs(minPosZ + maxPosZ) / 2);

        return centre;
    }


    private void OnDrawGizmos()
    {
        if(cityCentre != Vector3.zero)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(cityCentre, 1.0f);
        }
    }
}

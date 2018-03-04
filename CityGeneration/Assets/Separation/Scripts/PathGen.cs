using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathGen : MonoBehaviour
{


    public void ConnectLots(List<BuildingLot> _lots, Vector3 _cityCentre, int _cityRadius)
    {
        // Set closest lot as connected,
        _lots[0].ConnectToMain();

        _lots[0].Separation().BorderCheck(_lots);

        //ConnectUnconnectedLots(_lots, _cityCentre, _cityRadius);
    }


    /*private void ConnectUnconnectedLots(List<BuildingLot> _lots, Vector3 _cityCentre, int _cityRadius)
    {
        foreach (BuildingLot lot in _lots)
        {
            int counter = 0;

            while(!lot.Separation().ConnectedToMain())
            {
                lot.Separation().MoveTowardsCentre(_cityCentre);

                if (lot.Separation().BorderCheck2(_lots))
                    break;

                if (counter == 20)
                {
                    Debug.Log("Couldnt connect");
                    break;
                }

                counter++;
            }
        }
    }*/
}

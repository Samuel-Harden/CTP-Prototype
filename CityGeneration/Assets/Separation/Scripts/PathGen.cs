using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathGen : MonoBehaviour
{


    public void ConnectLots(List<BuildingLot> _lots, Vector3 _cityCentre)
    {
        // Set closest lot as connected,
        _lots[0].Separation().ConnectToMain();

        _lots[0].Separation().BorderCheck(_lots);

        ConnectUnconnectedLots(_lots, _cityCentre);
    }


    private void ConnectUnconnectedLots(List<BuildingLot> _lots, Vector3 _cityCentre)
    {
        foreach (BuildingLot lot in _lots)
        {
            // if lot is not connected
            if (!lot.Separation().ConnectedToMain())
            {
                lot.Separation().UpdatePosition(_cityCentre);
            }
        }
    }
}

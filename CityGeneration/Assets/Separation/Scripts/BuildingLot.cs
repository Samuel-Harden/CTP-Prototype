using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingLot : MonoBehaviour
{
    private int lotWidth;
    private int lotLength;
    private int lotHeight;

    private bool generateLot;

    private int lotID;
    private int lotType;

    private Separation separation;


    public void Initialise(int _lotWidth, int _lotLength, int _lotHeight,
        Vector3 _lotPos, int _roomType, int _roomID)
    {
        separation = GetComponent<Separation>();

        separation.Initialise(_lotWidth, _lotLength, _lotHeight);

        lotWidth = _lotWidth;
        lotLength = _lotLength;
        lotHeight = _lotHeight;
    }


    public Separation Separation()
    {
        return separation;
    }


    private void OnDrawGizmos()
    {
        if (separation.ConnectedToMain())
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, transform.localScale);
            return;
        }

        if (lotType != 0)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(transform.position, transform.localScale);
        }

        if (lotType == 0)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawWireCube(transform.position, transform.localScale);
        }

        /*if (connectedRooms != null)
        {
            foreach (Room room in connectedRooms)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawLine(transform.position, room.transform.position);
            }
        }*/
    }
}

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

    private bool connectedMain;

    private Vector3 lotBoundsPos;


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


    public Vector3 LotBounds()
    {
        return new Vector3(transform.position.x - lotWidth / 2, 0.0f,
            transform.position.z - lotLength / 2); ;
    }


    public int Width()
    {
        return lotWidth;
    }


    public int Length()
    {
        return lotLength;
    }


    public void ConnectToMain()
    {
        connectedMain = true;
    }


    public bool ConnectedToMain()
    {
        return connectedMain;
    }


    private void OnDrawGizmos()
    {
        if (connectedMain)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, transform.localScale);

            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, new Vector3(transform.localScale.x / 2, transform.localScale.y, transform.localScale.z / 2));
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

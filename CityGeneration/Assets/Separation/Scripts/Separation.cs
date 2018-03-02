using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Separation : MonoBehaviour
{
    private Vector3 lotBoundsPoint;

    Vector3 velocity;
    Vector3 acceleration;

    private float maxSpeed = 0.5f;
    private float maxForce = 0.5f;

    private bool overlapping;

    private int lotWidth;
    private int lotLength;
    private int lotHeight;

    private bool connectedMain;

    private List<BuildingLot> connectedLots;


    public void Initialise(int _lotWidth, int _lotLength, int _lotHeight)
    {
        lotWidth = _lotWidth;
        lotLength = _lotLength;
        lotHeight = _lotHeight;
    }


    public void CheckSpacing(List<BuildingLot> _lots)
    {
        lotBoundsPoint = new Vector3(transform.position.x - lotWidth / 2, 0.0f,
            transform.position.z - lotLength / 2);

        List<BuildingLot> overlappingRooms = new List<BuildingLot>();

        overlappingRooms = OverlapCheck(_lots);

        if (overlappingRooms.Count != 0)
        {
            Vector3 sep = Seperate(overlappingRooms);

            acceleration += sep;

            velocity += acceleration;

            velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

            transform.position = (transform.position + velocity);

            // reset acceleration to 0 each cycle
            acceleration = Vector3.zero;

            velocity = Vector3.zero;

            overlapping = true;

            return;
        }

        //if(BorderCheck)

        overlapping = false;
    }


    List<BuildingLot> OverlapCheck(List<BuildingLot> _lots)
    {
        List<BuildingLot> overlappingRooms = new List<BuildingLot>();

        for (int i = 0; i < _lots.Count; i++)
        {
            if (_lots[i].gameObject == this.gameObject)
                continue;

            // ((A.X + A.Width) > (B.X) &&
            if ((lotBoundsPoint.x + lotWidth)
                > (_lots[i].Separation().LotBounds().x) &&
                // (A.X) < (B.X + B.Width) &&
                (lotBoundsPoint.x) < (_lots[i].Separation().LotBounds().x
                    + _lots[i].Separation().Width()) &&

                // (A.Y + A.Height) > (B.Y) &&
                (lotBoundsPoint.z + lotLength)
                > (_lots[i].Separation().LotBounds().z) &&
                // (A.Y) < (B.Y + B.Height))
                (lotBoundsPoint.z) < (_lots[i].Separation().LotBounds().z
                    + _lots[i].Separation().Length()))
            {
                overlappingRooms.Add(_lots[i]);
            }
        }
        return overlappingRooms;
    }


    Vector3 Seperate(List<BuildingLot> _lots)
    {
        Vector3 steer = Vector3.zero;
        int count = 0;

        // check through every other _lot
        for (int i = 0; i < _lots.Count; i++)
        {
            float d = Vector3.Distance(transform.position, _lots[i].transform.position);
            // Calculate vector pointing away from other rooms
            if (d > 0)
            {
                Vector3 diff = (transform.position - _lots[i].transform.position);
                diff.Normalize();
                diff = (diff / d); // Weight by distance
                steer = (steer + diff);
                count++;
            }
        }

        // Average -- divided by how many
        if (count > 0)
        {
            steer = (steer / count);
        }

        // as long as the vector is greater than 0
        if (steer != Vector3.zero)
        {
            steer = Vector3.ClampMagnitude(steer, maxSpeed);

            steer.Normalize();
            steer = (steer * maxSpeed);
            steer = (steer - velocity);
        }

        return steer;
    }


    public void SetPos()
    {
        transform.position = new Vector3(Mathf.RoundToInt(transform.position.x),
        0.0f, Mathf.RoundToInt(transform.position.z));

        lotBoundsPoint = new Vector3(Mathf.RoundToInt(transform.position.x - lotWidth / 2),
        0.0f, Mathf.RoundToInt(transform.position.z - lotLength / 2));
    }


    public void BorderCheck(List<BuildingLot> _lots)
    {
        foreach (BuildingLot lot in _lots)
        {
            if(!lot.Separation().ConnectedToMain())
            {
                // ((A.X + A.Width) >= (B.X) &&
                if ((lotBoundsPoint.x + lotWidth)
                        >= (lot.Separation().LotBounds().x) &&
                    // (A.X) <= (B.X + B.Width) &&
                    (lotBoundsPoint.x) <= (lot.Separation().LotBounds().x
                        + lot.Separation().Width()) &&

                    // (A.Y + A.Height) >= (B.Y) &&
                    (lotBoundsPoint.z + lotLength)
                        >= (lot.Separation().LotBounds().z) &&
                    // (A.Y) <= (B.Y + B.Height))
                    (lotBoundsPoint.z) <= (lot.Separation().LotBounds().z
                        + lot.Separation().Length()))
                {
                    lot.Separation().ConnectToMain();
                    lot.Separation().BorderCheck(_lots);
                }
            }
        }
    }


    public void UpdatePosition(Vector3 _cityCentre)
    {
        //move towards centre, until we are bordering an already connect lot
        // establist which axis is closest
        while(!connectedMain)
        {
            float distanceX = Mathf.Abs(lotBoundsPoint.x + _cityCentre.x);
            float distanceZ = Mathf.Abs(lotBoundsPoint.z + _cityCentre.z);

            if (distanceX < distanceZ)
            {
                if (_cityCentre.x > lotBoundsPoint.x)
                {
                    // move right (positive)
                }

                else
                {
                    // move left (negative)
                }
            }

            else
            {
                // move on the Z axis
                if (_cityCentre.x > lotBoundsPoint.x)
                {
                    // move right (positive)
                }

                else
                {
                    // move left (negative)
                }
            }
        }
    }


    public Vector3 LotBounds()
    {
        return lotBoundsPoint;
    }


    public int Width()
    {
        return lotWidth;
    }


    public int Length()
    {
        return lotLength;
    }


    public bool IsOverlapping()
    {
        return overlapping;
    }


    public void ConnectToMain()
    {
        connectedMain = true;
    }


    public bool ConnectedToMain()
    {
        return connectedMain;
    }
}

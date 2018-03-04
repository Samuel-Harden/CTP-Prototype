using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Separation : MonoBehaviour
{
    private Vector3 lotBoundsPoint;

    Vector3 velocity;
    Vector3 acceleration;

    private float maxSpeed = 0.25f;

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


    public void CheckSpacing(List<BuildingLot> _lots, int _cityRadius)
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

        overlapping = false;
    }


    List<BuildingLot> OverlapCheck(List<BuildingLot> _lots)
    {
        List<BuildingLot> overlappingRooms = new List<BuildingLot>();

        float forceIncrease = 0.5f;

        for (int i = 0; i < _lots.Count; i++)
        {
            if (_lots[i].gameObject == this.gameObject)
                continue;

            // ((A.X + A.Width) > (B.X) &&
            if (((lotBoundsPoint.x - forceIncrease) + (float)lotWidth + forceIncrease)
                > (_lots[i].LotBounds().x) &&
                // (A.X) < (B.X + B.Width) &&
                (lotBoundsPoint.x) < (_lots[i].LotBounds().x
                    + _lots[i].Width()) &&

                // (A.Y + A.Height) > (B.Y) &&
                ((lotBoundsPoint.z - forceIncrease) + (float)lotLength + forceIncrease)
                > (_lots[i].LotBounds().z) &&
                // (A.Y) < (B.Y + B.Height))
                (lotBoundsPoint.z) < (_lots[i].LotBounds().z
                    + _lots[i].Length()))
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
            if(!lot.ConnectedToMain())
            {
                // ((A.X + A.Width) >= (B.X) &&
                if ((lotBoundsPoint.x + lotWidth)
                        >= (lot.LotBounds().x) &&
                    // (A.X) <= (B.X + B.Width) &&
                    (lotBoundsPoint.x) <= (lot.LotBounds().x
                        + lot.Width()) &&

                    // (A.Y + A.Height) >= (B.Y) &&
                    (lotBoundsPoint.z + lotLength)
                        >= (lot.LotBounds().z) &&
                    // (A.Y) <= (B.Y + B.Height))
                    (lotBoundsPoint.z) <= (lot.LotBounds().z
                        + lot.Length()))
                {
                    lot.ConnectToMain();
                    lot.Separation().BorderCheck(_lots);
                }
            }
        }
    }


    public void MoveTowardsCentre(Vector3 _cityCentre)
    {
        float offsetX = 0;
        float offsetZ = 0;

        lotBoundsPoint = new Vector3(transform.position.x - lotWidth / 2, 0.0f,
            transform.position.z - lotLength / 2);

        if (lotBoundsPoint.x > _cityCentre.x)
            offsetX = -1;

        else if (lotBoundsPoint.x < _cityCentre.x)
            offsetX = 1;

        //if (transform.position.z > _cityCentre.z)
            //offsetZ = -1;

        //else if (transform.position.z < _cityCentre.z)
            //offsetZ = 1;

        Debug.Log("moving towards Centre");

        transform.position = new Vector3(transform.position.x + offsetX, transform.position.y, transform.position.z + offsetZ);
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


    public bool IsOverlapping()
    {
        return overlapping;
    }


    public Vector3 GetTopLeftPos()
    {
        return new Vector3(transform.position.x - lotWidth / 2, 0.0f, transform.position.z + lotLength / 2);
    }


    public Vector3 GetTopRightPos()
    {
        return new Vector3(transform.position.x + lotWidth / 2, 0.0f, transform.position.z + lotLength / 2);
    }


    public Vector3 GetBottomLeftPos()
    {
        return new Vector3(transform.position.x - lotWidth / 2, 0.0f, transform.position.z - lotLength / 2);
    }


    public Vector3 GetBottomRightPos()
    {
        return new Vector3(transform.position.x + lotWidth / 2, 0.0f, transform.position.z - lotLength / 2);
    }
}

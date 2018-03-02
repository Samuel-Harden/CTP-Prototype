using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// This class will hold all the references in one place
public class References : MonoBehaviour
{
    CityGen cityGen;
    LotGen  lotGen;
    PathGen pathGen;


    private void Awake()
    {
        cityGen = GetComponent<CityGen>(); 
        lotGen  = GetComponent<LotGen>();
        pathGen = GetComponent<PathGen>();
    }


    public CityGen CityGen()
    {
        return cityGen;
    }


    public LotGen LotGen()
    {
        return lotGen;
    }


    public PathGen PathGen()
    {
        return pathGen;
    }
}

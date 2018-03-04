using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// This class will hold all the referencer in one place
public class References : MonoBehaviour
{
    CityGen cityGen;
    LotGen  lotGen;
    PathGen pathGen;
    TileGen tileGen;


    private void Awake()
    {
        cityGen = GetComponent<CityGen>(); 
        lotGen  = GetComponent<LotGen>();
        pathGen = GetComponent<PathGen>();
        tileGen = GetComponent<TileGen>();
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


    public TileGen TileGen()
    {
        return tileGen;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Town
{
    #region General
    public string Name;
    public int XLoc;
    public int YLoc;
    public int ownerID;
    public bool isCaptial;
    public int totalPop;
    public racePop[] racePops;
    public string[] dominantRaces;
    public int lvl;
    [Range(0, 100)]
    public int stability;
    public List<Shop> shops;
    #endregion

    public Town(Country Nation, int sLvl, WorldMap world, int x, int y, int natId, bool cap)
    {
        ownerID = natId;
        world.mapTiles[x, y].town = true;
        XLoc = x;
        YLoc = y;
        if (cap)
        {
            isCaptial = true;
            lvl = 5;
            stability = Mathf.RoundToInt(75 * Nation.stabilityMod);
        }
        else
        {
            lvl = sLvl;
            stability = Mathf.RoundToInt(50 * Nation.stabilityMod);
        }
    }

    public void getTotalPop()
    {
        foreach (racePop r in racePops)
        {
            totalPop += totalPop;
        }
    }
    
    public void racePopChange(int growth, racePop raceToChange)
    {
        raceToChange.Pop += growth;
        getTotalPop();
    }

}

[System.Serializable]
public struct racePop
{
    public string Race;
    public int Pop;
}
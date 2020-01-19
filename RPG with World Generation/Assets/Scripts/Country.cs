using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Country
{
    public string name;
    public int ID;
    public List<int> townIDs;
    [Range(0.5f, 1.5f)]
    public float stabilityMod;


    public void newTownLandClaim (WorldMap world, int tID, Dictionary<int, Town> tLst)
    {
        for (int y = -tLst[tID].lvl; y <= tLst[tID].lvl; y++)
        {
            for (int x = -tLst[tID].lvl; x <= tLst[tID].lvl; x++)
            {
                if(y < 0 || y > 149 || x < 0 || x > 149)
                {
                    continue;
                }

                if (world.mapTiles[x, y].ownerID == 0)
                {
                    world.mapTiles[x, y].ownerID = ID;
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Buildings
{
    #region general;
    public string name;
    public int stoneCost;
    public int woodCost;
    public int goldCost;
    public int requiredLvl;
    public bool upgrade;
    public string upgradeFrom;
    public bool constrFinished;
    public int monthsToBuild;
    #endregion

    #region Benefits
    public int foodGain;
    public int stoneGain;
    public int woodGain;
    public int goldGain;
    public int tradeValue;
    public int oreGain;
    #endregion

    #region Upkeep
    public int foodUpkeep;
    public int stoneUpkeep;
    public int woodUpkeep;
    public int goldUpkeep;
    public int oreUpkeep;
    #endregion

    #region Specials
    public bool requireToLvl;
    //add bonuses class when made
    #endregion

    public Buildings(Buildings build)
    {
    name = build.name;
     stoneCost = build.stoneCost;
     woodCost = build.woodCost;
     goldCost = build.goldCost;
     requiredLvl = build.requiredLvl;
     upgrade = build.upgrade;
     upgradeFrom = build.upgradeFrom;
     constrFinished = build.constrFinished;
     monthsToBuild = build.monthsToBuild;
     foodGain = build.foodGain;
     stoneGain = build.stoneGain;
     woodGain = build.woodGain;
     goldGain = build.goldGain;
     tradeValue = build.tradeValue;
     oreGain = build.oreGain;
     foodUpkeep = build.foodUpkeep;
     stoneUpkeep = build.stoneUpkeep;
     woodUpkeep = build.woodUpkeep;
     goldUpkeep = build.goldUpkeep;
     oreUpkeep = build.oreUpkeep;
}

    public Buildings()
    {
        stoneCost = 0;
        woodCost = 0;
        goldCost = 0;
        requiredLvl = 0;
        upgrade = false;
        upgradeFrom = "";
        constrFinished = false;
        monthsToBuild = 99;
        foodGain = 0;
        stoneGain = 0;
        woodGain = 0;
        goldGain = 0;
        tradeValue = 0;
        oreGain = 0;
        foodUpkeep = 0;
        stoneUpkeep = 0;
        woodUpkeep = 0;
        goldUpkeep = 0;
        oreUpkeep = 0;
    }

}

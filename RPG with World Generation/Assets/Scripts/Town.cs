using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Town
{
    #region General
    public string Name;
    public bool isCaptial;
    public int totalPop;
    public racePop[] racePops;
    public string[] dominantRaces;
    public List<Buildings> townBuildings = new List<Buildings>();
    public bool buildingInProgress;
    public int buildTimeLeft;
    public int constrIndex;
    public int lvl;
    #endregion

    #region Resources
    public int Gold;
    public int Wood;
    public int Food;
    public int Stone;
    public int Ore;
    #endregion

    #region income
    public int GoldIn;
    public int WoodIn;
    public int FoodIn;
    public int StoneIn;
    public int OreIn;
    #endregion

    #region Upkeep
    public int GoldUp;
    public int WoodUp;
    public int FoodUp;
    public int StoneUp;
    public int OreUp;
    #endregion

    #region DerivedStats
    public int factionTaxes;
    public int tradeValue;
    public float GrowthRate;
    #endregion



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

    public void upkeepStep()
    {
        Gold += GoldIn - GoldUp;
        Stone += StoneIn - StoneUp;
        Wood += WoodIn - WoodUp;
        Food += FoodIn - FoodUp;
        Ore += OreIn - OreUp;
        if (buildTimeLeft > 0)
        {
            buildTimeLeft--;
            if(buildTimeLeft == 0)
            {

            }
        }

    }

    public void buildNewBulding(Buildings toBuild)
    {
        Buildings build = new Buildings(toBuild);
        townBuildings.Add(build);
        buildingInProgress = true;
        buildTimeLeft = toBuild.monthsToBuild;
        constrIndex = townBuildings.Count - 1;
    }

    public void upgradeBuilding(Buildings upgrade)
    {
        int i = 0;
        foreach (Buildings u in townBuildings)
        {
            if (u.name == upgrade.upgradeFrom)
            {
                townBuildings[i] = new Buildings(upgrade);
                constrIndex = i;
                buildTimeLeft = upgrade.monthsToBuild;
                buildingInProgress = true;
                break;
            }
            i++;
        }

    }

    public void constructionComplete()
    {
        townBuildings[constrIndex].constrFinished = true;
        GoldIn += townBuildings[constrIndex].goldGain;
        GoldUp += townBuildings[constrIndex].goldUpkeep;
        StoneIn += townBuildings[constrIndex].stoneGain;
        StoneUp += townBuildings[constrIndex].stoneUpkeep;
        WoodIn += townBuildings[constrIndex].woodGain;
        WoodUp += townBuildings[constrIndex].woodUpkeep;
        FoodIn += townBuildings[constrIndex].foodGain;
        FoodUp += townBuildings[constrIndex].foodUpkeep;
        OreIn += townBuildings[constrIndex].oreGain;
        OreUp += townBuildings[constrIndex].oreUpkeep;
    }

    public void addBuilding(Buildings toAdd)
    {
        Buildings add = new Buildings(toAdd);
        townBuildings.Add(add);
        townBuildings[townBuildings.Count-1].constrFinished = true;
        GoldIn += toAdd.goldGain;
        GoldUp += toAdd.goldUpkeep;
        StoneIn += toAdd.stoneGain;
        StoneUp += toAdd.stoneUpkeep;
        WoodIn += toAdd.woodGain;
        WoodUp += toAdd.woodUpkeep;
        FoodIn += toAdd.foodGain;
        FoodUp += toAdd.foodUpkeep;
        OreIn += toAdd.oreGain;
        OreUp += toAdd.oreUpkeep;
    }
}

[System.Serializable]
public struct racePop
{
    public string Race;
    public int Pop;
}
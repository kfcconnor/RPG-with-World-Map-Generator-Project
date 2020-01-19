using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class WorldMapManager : MonoBehaviour
{
    public WorldMap worldMap;
    public BuildingList buildingTree;
    [SerializeField]
    public Dictionary<int, Country> CountryList;
    [SerializeField]
    public Dictionary<int, Town> TownList;
    public Grid grid; //  You can also use the Tilemap object
    public Text Text;
    public int size;
    public string savePath;
    //public int NumNations;
    public int TownNumber = 0;
    [SerializeField]
    public Town[] Debug;


    public void Update()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int coordinate = grid.WorldToCell(mouseWorldPos);
        MapTile currentTile = worldMap.mapTiles[coordinate.x, coordinate.y];
        Text.text = "Tile X: " + coordinate.x.ToString() + "\nTile Y: " + coordinate.y.ToString() + "\nBiome: " + currentTile.Biome + "\nFood: " + currentTile.Food + "\nWood: " + currentTile.Wood + "\nStone: " + currentTile.Stone + "\nOwner: " + currentTile.ownerID + "\nTown: " + currentTile.town + "\nLife: " + currentTile.lifeScore + "\nOre Types: ";
        //if (currentTile.oreTypes.Count >= 0)
        //{
        //    foreach(ore o in currentTile.oreTypes)
        //    {
        //        Text.text += o.name + " ";
        //    }
        //}
    }

    public void nationSetup (int numOfNations)
    {
        CountryList = new Dictionary<int, Country>();
        TownList = new Dictionary<int, Town>();
        Country currentCountry = new Country();
        for (int i = 0; i < numOfNations; i++)
        {
            currentCountry.name = (i + 1).ToString();
            currentCountry.ID = i + 1;
            currentCountry.stabilityMod = 1;
            CountryList.Add(currentCountry.ID, currentCountry);
        }
    }

    public void addTown(int OwnID, bool captial, int slvl, int x, int y)
    {
        Country owner = CountryList[OwnID];
        TownList.Add(TownNumber, new Town(owner, slvl, worldMap, x, y, OwnID, captial));
        owner.newTownLandClaim(worldMap, TownNumber, TownList);
        TownNumber++;
    }

    public void saveMap(int size)
    {
        string mapJson;
        mapJson = "{\n  \"Map Name\" : [\n";
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                mapJson += JsonUtility.ToJson(worldMap.mapTiles[x, y]) + ",\n";
            }
        }
        mapJson += "] \n }";
        savePath = Directory.GetCurrentDirectory() + "\\map.json";
        File.WriteAllText(savePath, mapJson);

    }

    //public void CreateTown(int townX, int townY, string townName,int lvl, bool Captial)
    //{
    //    List<Buildings> buildingsToAdd = new List<Buildings>();
    //    worldMap.mapTiles[townX, townY].town = true;
    //    worldMap.mapTiles[townX, townY].townDetails = new Town();
    //    worldMap.mapTiles[townX, townY].townDetails.Name = townName;
    //    worldMap.mapTiles[townX, townY].townDetails.Food = Mathf.RoundToInt(worldMap.mapTiles[townX, townY].Food / 10);
    //    worldMap.mapTiles[townX, townY].townDetails.Wood = Mathf.RoundToInt(worldMap.mapTiles[townX, townY].Wood / 10);
    //    worldMap.mapTiles[townX, townY].townDetails.Stone = Mathf.RoundToInt(worldMap.mapTiles[townX, townY].Stone / 10);
    //    worldMap.mapTiles[townX, townY].townDetails.Ore = Mathf.RoundToInt(worldMap.mapTiles[townX, townY].Ore / 10);
    //    worldMap.mapTiles[townX, townY].townDetails.lvl = lvl;
    //    Debug[TownNumber] = new Town();
    //    Debug[TownNumber].Name = townName;
    //    Debug[TownNumber].Food = Mathf.RoundToInt(worldMap.mapTiles[townX, townY].Food / 10);
    //    Debug[TownNumber].Wood = Mathf.RoundToInt(worldMap.mapTiles[townX, townY].Wood / 10);
    //    Debug[TownNumber].Stone = Mathf.RoundToInt(worldMap.mapTiles[townX, townY].Stone / 10);
    //    Debug[TownNumber].Ore = Mathf.RoundToInt(worldMap.mapTiles[townX, townY].Ore / 10);
    //    Debug[TownNumber].lvl = lvl;

    //    if (Captial)
    //    {
    //        worldMap.mapTiles[townX, townY].townDetails.isCaptial = true;
    //        Debug[TownNumber].isCaptial = true;
    //        int startFarms = Mathf.RoundToInt(worldMap.mapTiles[townX, townY].Food / 2);
    //        int startLumberjacks = Mathf.RoundToInt(worldMap.mapTiles[townX, townY].Wood / 2);
    //        int startQuarries = Mathf.RoundToInt(worldMap.mapTiles[townX, townY].Stone / 2);
    //        int startMines = Mathf.RoundToInt(worldMap.mapTiles[townX, townY].Ore / 2);
    //        buildingsToAdd.Add(searchForBuilding("Town Hall"));
    //        buildingsToAdd.Add(searchForBuilding("Market"));
    //        for (int i = 0; i < startFarms; i++)
    //        {
    //            buildingsToAdd.Add(searchForBuilding("Farm"));
    //        }
    //        for (int i = 0; i < startLumberjacks; i++)
    //        {
    //            buildingsToAdd.Add(searchForBuilding("Lumberjack"));
    //        }
    //        for (int i = 0; i < startQuarries; i++)
    //        {
    //            buildingsToAdd.Add(searchForBuilding("Quarry"));
    //        }
    //        for (int i = 0; i < startMines; i++)
    //        {
    //            buildingsToAdd.Add(searchForBuilding("Mine"));
    //        }
    //    }

    //    foreach(Buildings b in buildingsToAdd)
    //    {
    //        worldMap.mapTiles[townX, townY].townDetails.addBuilding(b);
    //        Debug[TownNumber].addBuilding(b);
    //    }

    //    TownNumber++;
    //}

    public Buildings searchForBuilding(string buildingName)
    {
        Buildings buildingFound = new Buildings();
        foreach (Buildings b in buildingTree.allBuldings)
        {
            if (b.name == buildingName)
            {
                buildingFound = b;
            }
        }
        return buildingFound;
    }

}

[System.Serializable]
public struct BuildingList
{
    public Buildings[] allBuldings;
}

[System.Serializable]
public struct WorldMap
{
    public MapTile[,] mapTiles;
    
}

[System.Serializable]
public struct MapTile
{
    public Vector2 coord;
    public string Biome;
    public int BiomeType;
    public int biomeId;
    public float Food;
    public float Trade;
    public float Stone;
    public float Wood;
    public float Ore;
    public bool town;
    public int townID;
    public int ownerID;
    public float lifeScore;
    public bool toClean;
}

[System.Serializable]
public struct NearTile
{
    public MapTile NW;
    public MapTile N;
    public MapTile NE;
    public MapTile W;
    public MapTile E;
    public MapTile SW;
    public MapTile S;
    public MapTile SE;

    public bool isNW;
    public bool isN;
    public bool isNE;
    public bool isW;
    public bool isE;
    public bool isSW;
    public bool isS;
    public bool isSE;
}
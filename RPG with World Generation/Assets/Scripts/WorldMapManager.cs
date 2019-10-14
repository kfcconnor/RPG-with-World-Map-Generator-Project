using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class WorldMapManager : MonoBehaviour
{
    public WorldMap worldMap;
    public BuildingList buildingTree;
    public Grid grid; //  You can also use the Tilemap object
    public Text Text;
    public int size;
    public int NumNations;
    public int TownNumber = 0;
    [SerializeField]
    public Town[] Debug;


    public void Update()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int coordinate = grid.WorldToCell(mouseWorldPos);
        MapTile currentTile = worldMap.mapTiles[coordinate.x, coordinate.y];
        Text.text = "Tile X: " + coordinate.x.ToString() + "\nTile Y: " + coordinate.y.ToString() + "\nBiome: " + currentTile.Biome + "\nFood: " + currentTile.Food + "\nWood: " + currentTile.Wood + "\nStone: " + currentTile.Stone + "\nOre: " + currentTile.Ore + "\nTown: " + currentTile.town + "\nLife: " + currentTile.lifeScore + "\nOre Types: ";
        if (currentTile.oreTypes.Count >= 0)
        {
            foreach(ore o in currentTile.oreTypes)
            {
                Text.text += o.name + " ";
            }
        }
    }

    public void CreateTown(int townX, int townY, string townName,int lvl, bool Captial)
    {
        List<Buildings> buildingsToAdd = new List<Buildings>();
        worldMap.mapTiles[townX, townY].town = true;
        worldMap.mapTiles[townX, townY].townDetails = new Town();
        worldMap.mapTiles[townX, townY].townDetails.Name = townName;
        worldMap.mapTiles[townX, townY].townDetails.Food = Mathf.RoundToInt(worldMap.mapTiles[townX, townY].Food / 10);
        worldMap.mapTiles[townX, townY].townDetails.Wood = Mathf.RoundToInt(worldMap.mapTiles[townX, townY].Wood / 10);
        worldMap.mapTiles[townX, townY].townDetails.Stone = Mathf.RoundToInt(worldMap.mapTiles[townX, townY].Stone / 10);
        worldMap.mapTiles[townX, townY].townDetails.Ore = Mathf.RoundToInt(worldMap.mapTiles[townX, townY].Ore / 10);
        worldMap.mapTiles[townX, townY].townDetails.lvl = lvl;
        Debug[TownNumber] = new Town();
        Debug[TownNumber].Name = townName;
        Debug[TownNumber].Food = Mathf.RoundToInt(worldMap.mapTiles[townX, townY].Food / 10);
        Debug[TownNumber].Wood = Mathf.RoundToInt(worldMap.mapTiles[townX, townY].Wood / 10);
        Debug[TownNumber].Stone = Mathf.RoundToInt(worldMap.mapTiles[townX, townY].Stone / 10);
        Debug[TownNumber].Ore = Mathf.RoundToInt(worldMap.mapTiles[townX, townY].Ore / 10);
        Debug[TownNumber].lvl = lvl;

        if (Captial)
        {
            worldMap.mapTiles[townX, townY].townDetails.isCaptial = true;
            Debug[TownNumber].isCaptial = true;
            int startFarms = Mathf.RoundToInt(worldMap.mapTiles[townX, townY].Food / 10);
            int startLumberjacks = Mathf.RoundToInt(worldMap.mapTiles[townX, townY].Wood / 10);
            int startQuarries = Mathf.RoundToInt(worldMap.mapTiles[townX, townY].Stone / 10);
            int startMines = Mathf.RoundToInt(worldMap.mapTiles[townX, townY].Ore / 10);
            buildingsToAdd.Add(searchForBuilding("Town Hall"));
            for(int i = 0; i < startFarms; i++)
            {
                buildingsToAdd.Add(searchForBuilding("Farm"));
            }
            for (int i = 0; i < startLumberjacks; i++)
            {
                buildingsToAdd.Add(searchForBuilding("Lumberjack"));
            }
            for (int i = 0; i < startQuarries; i++)
            {
                buildingsToAdd.Add(searchForBuilding("Quarry"));
            }
            for (int i = 0; i < startMines; i++)
            {
                buildingsToAdd.Add(searchForBuilding("Mine"));
            }
        }

        foreach(Buildings b in buildingsToAdd)
        {
            worldMap.mapTiles[townX, townY].townDetails.addBuilding(b);
            Debug[TownNumber].addBuilding(b);
        }

        TownNumber++;
    }

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
    public Biome biomeData;
    public Tile tile;
    public float Food;
    public float Trade;
    public float Stone;
    public float Wood;
    public float Ore;
    public List<ore> oreTypes;
    public bool town;
    public Town townDetails;
    public float lifeScore;
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
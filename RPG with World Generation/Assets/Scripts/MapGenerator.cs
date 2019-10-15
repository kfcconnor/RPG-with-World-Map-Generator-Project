using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{

    public enum DrawMode {noiseMap, ColourMap, FalloffMap, HeatMap, RainMap, tileMap}
    public DrawMode drawMode;

    [SerializeField]
    public AnimationCurve heatCurve;
    [SerializeField]
    public AnimationCurve MoistureCurve;

    public Tilemap worldMap;
    public Tilemap TownMap;
    public Tilemap waterMap;
    public Tilemap treeMap;
    public Tilemap mountMap;
    public WorldMapManager worldMapMan;
    public MetalManager metalMan;

    public int size;
    public int centreVertex;
    [Range(0, 1)]
    public float seaLevel;
    public float noiseScale;
    public float rainScale;
    public float heatScale;
    public float maxRain;
    public float minRain = 1000;
    public float maxHeat;
    public float minHeat = 1000;
    public float maxRain2;
    public float minRain2 = 1000;
    public float maxHeat2;
    public float minHeat2 = 1000;

    public int MaxDistance;
    public int offsetZ;

    public int octaves;
    [Range(0,1)]
    public float persistance;
    public float lanunarity;

    public int seed;
    public Vector2 offset;

    public bool autoUpdate;
    public bool useFalloff;

    public Tile coastWater;
    public Tile town;

    public TerrainType[] regions;
    public SeaTerrains[] seaTerrains;
    public MountTerrains[] mountTerrains;
    public HeatTerrains[] heatTerrains;
    public rainTerrains[] rainTerrains;
    public BiomeRow[] biomeRows;

    float[,] falloffMap;

    void Awake()
    {
        falloffMap = FalloffGenerator.GeneratorFalloffMap(size);
        GenerateMap();
        
    }

    public void GenerateMap()
    {
        worldMap.ClearAllTiles();
        TownMap.ClearAllTiles();
        waterMap.ClearAllTiles();
        treeMap.ClearAllTiles();
        mountMap.ClearAllTiles();
        worldMapMan.worldMap.mapTiles = new MapTile[size, size];
        worldMapMan.Debug = new Town[worldMapMan.NumNations];
        captialGen capGen = new captialGen();
        worldMapMan.size = size;
        System.Random resources = new System.Random(seed);
        centreVertex = size / 2;
        Vector2 rainOffset = new Vector2(offset.x + 4000, offset.y + 4000);
        Vector2 heatOffset = new Vector2(offset.x - 2000, offset.y - 2000);
        float[,] noiseMap = Noise.GenerateNoiseMap(size, seed,noiseScale, octaves, persistance, lanunarity, offset);
        float[,] rainMap = Noise.GenerateNoiseMap(size, seed, rainScale, octaves, persistance, lanunarity, rainOffset);
        float[,] uniformHeatMap = Noise.GenerateUniformNoiseMap(size, centreVertex, MaxDistance, offsetZ);
        float[,] randomHeatMap = Noise.GenerateNoiseMap(size, seed, heatScale, octaves, persistance, lanunarity, heatOffset);
        float[,] heatMap = new float[size, size];
        Color[] colourMap = new Color[size * size];

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                if (useFalloff)
                {
                    noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - falloffMap[x, y]);
                }

                heatMap[x, y] = uniformHeatMap[x, y] * randomHeatMap[x, y];
                heatMap[x, y] += heatCurve.Evaluate(noiseMap[x, y] * noiseMap[x, y]);
                rainMap[x, y] += MoistureCurve.Evaluate(noiseMap[x, y] * noiseMap[x, y]);

                float currentRain = rainMap[x, y];
                float currentHeat = heatMap[x, y];

                if (currentRain > maxRain)
                {
                    maxRain = currentRain;
                }
                if (currentRain < minRain)
                {
                    minRain = currentRain;
                }
                if (currentHeat > maxHeat)
                {
                    maxHeat = currentHeat;
                }
                if (currentHeat < minHeat)
                {
                    minHeat = currentHeat;
                }
            }
        }
        int tilNo = 0;
     for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {

                rainMap[x, y] = Mathf.InverseLerp(minRain, maxRain, rainMap[x, y]);

                Vector3Int location = new Vector3Int(x, y, 0);
                float currentHeight = noiseMap[x, y];
                float currentRain = rainMap[x, y];
                float currentHeat = heatMap[x, y];
                int Heat = 0;
                string Rain = "";

                if (currentRain > maxRain)
                {
                    maxRain2 = currentRain;
                }
                if (currentRain < minRain)
                {
                    minRain2 = currentRain;
                }
                if (currentHeat > maxHeat)
                {
                    maxHeat2 = currentHeat;
                }
                if (currentHeat < minHeat)
                {
                    minHeat2 = currentHeat;
                }

                int terrainType = 0;
                worldMapMan.worldMap.mapTiles[x, y].oreTypes = new List<ore>();
                if (currentHeight <= seaLevel)
                {
                    terrainType = 0;
                }
                else if (currentHeight > 0.7)
                {
                    terrainType = 1;
                }
                else
                {
                    terrainType = 2;
                }
                switch (terrainType)
                {
                    case 0:
                        {
                            for (int i = 0; i < seaTerrains.Length; i++)
                            {
                                if(currentHeight <= seaTerrains[i].height)
                                {
                                    colourMap[y * size + x] = seaTerrains[i].colour;
                                    worldMapMan.worldMap.mapTiles[x, y].coord.x = x;
                                    worldMapMan.worldMap.mapTiles[x, y].coord.y = y;
                                    worldMapMan.worldMap.mapTiles[x, y].Biome = seaTerrains[i].name;
                                    worldMapMan.worldMap.mapTiles[x, y].tile = seaTerrains[i].tile;
                                    worldMapMan.worldMap.mapTiles[x, y].Food = resources.Next(0, 10) + seaTerrains[i].FoodMod;
                                    worldMapMan.worldMap.mapTiles[x, y].Stone = resources.Next(0, 10) + seaTerrains[i].StoneMod;
                                    worldMapMan.worldMap.mapTiles[x, y].Wood = resources.Next(0, 10) + seaTerrains[i].WoodMod;
                                    worldMapMan.worldMap.mapTiles[x, y].Ore = 0;
                                    worldMapMan.worldMap.mapTiles[x, y].BiomeType = 0;
                                    //worldMapMan.worldMap.Debug[y * size + x] = worldMapMan.worldMap.mapTiles[x, y];
                                    //worldMap.SetTile(location, seaTerrains[i].tile);
                                    break;
                                }
                            }
                                break;
                        }
                    case 1:
                        {
                            for (int i = 0; i < mountTerrains.Length; i++)
                            {
                                if (currentHeight <= mountTerrains[i].height)
                                {
                                    colourMap[y * size + x] = mountTerrains[i].colour;
                                    worldMapMan.worldMap.mapTiles[x, y].coord.x = x;
                                    worldMapMan.worldMap.mapTiles[x, y].coord.y = y;
                                    worldMapMan.worldMap.mapTiles[x, y].Biome = mountTerrains[i].name;
                                    worldMapMan.worldMap.mapTiles[x, y].tile = mountTerrains[i].tile;
                                    worldMapMan.worldMap.mapTiles[x, y].Food = resources.Next(0, 10) + mountTerrains[i].FoodMod;
                                    worldMapMan.worldMap.mapTiles[x, y].Stone = resources.Next(0, 10) + mountTerrains[i].StoneMod;
                                    worldMapMan.worldMap.mapTiles[x, y].Wood = resources.Next(0, 10) + mountTerrains[i].WoodMod;
                                    worldMapMan.worldMap.mapTiles[x, y].Ore = resources.Next(0, 10) + mountTerrains[i].OreMod;
                                    worldMapMan.worldMap.mapTiles[x, y].BiomeType = 1;
                                    System.Random oreChance = new System.Random();
                                    foreach (ore o in metalMan.OreList)
                                    {
                                        int typeChance = (resources.Next(0, 100));
                                        if (typeChance < o.rarity*100)
                                        {
                                            worldMapMan.worldMap.mapTiles[x, y].oreTypes.Add(o);
                                        }
                                    }
                                    //worldMapMan.worldMap.Debug[y * size + x] = worldMapMan.worldMap.mapTiles[x, y];
                                    //worldMap.SetTile(location, mountTerrains[i].tile);
                                    break;
                                }
                            }
                            break;
                        }
                    case 2:
                        {
                            worldMapMan.worldMap.mapTiles[x, y].BiomeType = 2;
                            for (int i = 0; i < heatTerrains.Length; i++)
                            {
                                if (currentHeat <= heatTerrains[i].height)
                                {
                                    Heat = heatTerrains[i].index;
                                    break;
                                }
                            }
                            for (int i = 0; i < rainTerrains.Length; i++)
                            {
                                if (currentRain <= rainTerrains[i].height)
                                {
                                    Rain = rainTerrains[i].name;
                                    break;
                                }
                            }
                            switch (Rain)
                            {
                                case "Dryest":
                                    {
                                        colourMap[y * size + x] = biomeRows[0].biomes[Heat].color;
                                        if (biomeRows[0].biomes[Heat].forest)
                                        {
                                            worldMapMan.worldMap.mapTiles[x, y].BiomeType = 3;
                                        }
                                        worldMapMan.worldMap.mapTiles[x, y].coord.x = x;
                                        worldMapMan.worldMap.mapTiles[x, y].coord.y = y;
                                        worldMapMan.worldMap.mapTiles[x, y].Biome = biomeRows[0].biomes[Heat].name;
                                        worldMapMan.worldMap.mapTiles[x, y].biomeData = biomeRows[0].biomes[Heat];
                                        worldMapMan.worldMap.mapTiles[x, y].tile = biomeRows[0].biomes[Heat].tileC;
                                        worldMapMan.worldMap.mapTiles[x, y].Food = resources.Next(0, 10) + biomeRows[0].biomes[Heat].FoodMod;
                                        worldMapMan.worldMap.mapTiles[x, y].Stone = resources.Next(0, 10) + biomeRows[0].biomes[Heat].StoneMod;
                                        worldMapMan.worldMap.mapTiles[x, y].Wood = resources.Next(0, 10) + biomeRows[0].biomes[Heat].WoodMod;
                                        worldMapMan.worldMap.mapTiles[x, y].Ore = 0;
                                        //worldMapMan.worldMap.Debug[y * size + x] = worldMapMan.worldMap.mapTiles[x, y];
                                        //worldMap.SetTile(location, biomeRows[0].biomes[Heat].tile);
                                        break;
                                    }
                                case "Dry":
                                    {
                                        colourMap[y * size + x] = biomeRows[1].biomes[Heat].color;
                                        if (biomeRows[1].biomes[Heat].forest)
                                        {
                                            worldMapMan.worldMap.mapTiles[x, y].BiomeType = 3;
                                        }
                                        worldMapMan.worldMap.mapTiles[x, y].coord.x = x;
                                        worldMapMan.worldMap.mapTiles[x, y].coord.y = y;
                                        worldMapMan.worldMap.mapTiles[x, y].Biome = biomeRows[1].biomes[Heat].name;
                                        worldMapMan.worldMap.mapTiles[x, y].biomeData = biomeRows[0].biomes[Heat];
                                        worldMapMan.worldMap.mapTiles[x, y].tile = biomeRows[1].biomes[Heat].tileC;
                                        worldMapMan.worldMap.mapTiles[x, y].Food = resources.Next(0, 10) + biomeRows[1].biomes[Heat].FoodMod;
                                        worldMapMan.worldMap.mapTiles[x, y].Stone = resources.Next(0, 10) + biomeRows[1].biomes[Heat].StoneMod;
                                        worldMapMan.worldMap.mapTiles[x, y].Wood = resources.Next(0, 10) + biomeRows[1].biomes[Heat].WoodMod;
                                        worldMapMan.worldMap.mapTiles[x, y].Ore = 0;
                                        //worldMapMan.worldMap.Debug[y * size + x] = worldMapMan.worldMap.mapTiles[x, y];
                                        //worldMap.SetTile(location, biomeRows[1].biomes[Heat].tile);
                                        break;
                                    }
                                case "Wet":
                                    {
                                        colourMap[y * size + x] = biomeRows[2].biomes[Heat].color;
                                        if (biomeRows[2].biomes[Heat].forest)
                                        {
                                            worldMapMan.worldMap.mapTiles[x, y].BiomeType = 3;
                                        }
                                        worldMapMan.worldMap.mapTiles[x, y].coord.x = x;
                                        worldMapMan.worldMap.mapTiles[x, y].coord.y = y;
                                        worldMapMan.worldMap.mapTiles[x, y].Biome = biomeRows[2].biomes[Heat].name;
                                        worldMapMan.worldMap.mapTiles[x, y].biomeData = biomeRows[0].biomes[Heat];
                                        worldMapMan.worldMap.mapTiles[x, y].tile = biomeRows[2].biomes[Heat].tileC;
                                        worldMapMan.worldMap.mapTiles[x, y].Food = resources.Next(0, 10) + biomeRows[2].biomes[Heat].FoodMod;
                                        worldMapMan.worldMap.mapTiles[x, y].Stone = resources.Next(0, 10) + biomeRows[2].biomes[Heat].StoneMod;
                                        worldMapMan.worldMap.mapTiles[x, y].Wood = resources.Next(0, 10) + biomeRows[2].biomes[Heat].WoodMod;
                                        worldMapMan.worldMap.mapTiles[x, y].Ore = 0;
                                        //worldMapMan.worldMap.Debug[y * size + x] = worldMapMan.worldMap.mapTiles[x, y];
                                        //worldMap.SetTile(location, biomeRows[2].biomes[Heat].tile);
                                        break;
                                    }
                                case "Wettest":
                                    {
                                        colourMap[y * size + x] = biomeRows[3].biomes[Heat].color;
                                        if (biomeRows[3].biomes[Heat].forest)
                                        {
                                            worldMapMan.worldMap.mapTiles[x, y].BiomeType = 3;
                                        }
                                        worldMapMan.worldMap.mapTiles[x, y].coord.x = x;
                                        worldMapMan.worldMap.mapTiles[x, y].coord.y = y;
                                        worldMapMan.worldMap.mapTiles[x, y].Biome = biomeRows[3].biomes[Heat].name;
                                        worldMapMan.worldMap.mapTiles[x, y].biomeData = biomeRows[0].biomes[Heat];
                                        worldMapMan.worldMap.mapTiles[x, y].tile = biomeRows[3].biomes[Heat].tileC;
                                        worldMapMan.worldMap.mapTiles[x, y].Food = resources.Next(0, 10) + biomeRows[3].biomes[Heat].FoodMod;
                                        worldMapMan.worldMap.mapTiles[x, y].Stone = resources.Next(0, 10) + biomeRows[3].biomes[Heat].StoneMod;
                                        worldMapMan.worldMap.mapTiles[x, y].Wood = resources.Next(0, 10) + biomeRows[3].biomes[Heat].WoodMod;
                                        worldMapMan.worldMap.mapTiles[x, y].Ore = 0;
                                        //worldMapMan.worldMap.Debug[y * size + x] = worldMapMan.worldMap.mapTiles[x, y];
                                        //worldMap.SetTile(location, biomeRows[3].biomes[Heat].tile);
                                        break;
                                    }
                            }
                            break;
                        }

                }
                tilNo++;
            }
            
        }
        capGen.captialPos(worldMapMan, size, worldMapMan.NumNations, TownMap, town);
        MapDisplay display = FindObjectOfType<MapDisplay> ();
        if (drawMode == DrawMode.noiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
        }
        else if (drawMode == DrawMode.ColourMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColourMap(colourMap, size));
        }
        else if (drawMode == DrawMode.FalloffMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(FalloffGenerator.GeneratorFalloffMap(size)));
        }
        else if (drawMode == DrawMode.RainMap)
        {
            Color[] colourMap3 = new Color[size * size];
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float currentHeight = rainMap[x, y];
                    for (int i = 0; i < rainTerrains.Length; i++)
                    {
                        if (currentHeight <= rainTerrains[i].height)
                        {
                            colourMap3[y * size + x] = rainTerrains[i].colour;
                            break;
                        }
                    }
                }
                
            }
            display.DrawTexture(TextureGenerator.TextureFromColourMap(colourMap3, size));
        }
        else if (drawMode == DrawMode.HeatMap)
        {
            Color[] colourMap2 = new Color[size * size];
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float currentHeight = heatMap[x, y];
                    for (int i = 0; i < heatTerrains.Length; i++)
                    {
                        if (currentHeight <= heatTerrains[i].height)
                        {
                            colourMap2[y * size + x] = heatTerrains[i].colour;
                            break;
                        }
                    }
                }
            }
            display.DrawTexture(TextureGenerator.TextureFromColourMap(colourMap2, size));
        }
        else if (drawMode == DrawMode.tileMap)
        {
            TextureGenerator.SpriteMapFromTileData(worldMapMan.worldMap, colourMap, size, worldMap, coastWater, waterMap, treeMap, mountMap);
        }
    }

    public class captialGen
    {

        public Vector2Int[] townLocs;

        public void captialPos(WorldMapManager worldMan, int size, int NumNations, Tilemap townMap, Tile town)
        {
            townLocs = new Vector2Int[NumNations];
            worldMan.TownNumber = 0;
            Vector2Int bestTile = new Vector2Int();
            float bestLife = 0;
            for (int i = 0; i < NumNations; i++)
            {
                for (int y = 0; y < size; y++)
                {
                    for (int x = 0; x < size; x++)
                    {
                        bool tooClose = false;
                        if (worldMan.worldMap.mapTiles[x, y].Biome == "Water Shallow" || worldMan.worldMap.mapTiles[x, y].Biome == "Water Deep" || worldMan.worldMap.mapTiles[x, y].Biome == "Snow" || worldMan.worldMap.mapTiles[x, y].town)
                        {
                            continue;
                        }
                        Vector2Int currentTile = new Vector2Int(x, y);
                        for (int j = 0; j < NumNations; j++)
                        {
                            float distance = Vector2Int.Distance(townLocs[j], currentTile);
                            if (distance < 30)
                            {
                                tooClose = true;
                            }
                        }
                        if (tooClose)
                        {
                            continue;
                        }
                        float LocalFood = worldMan.worldMap.mapTiles[x, y].Food + worldMan.worldMap.mapTiles[x + 1, y].Food + worldMan.worldMap.mapTiles[x - 1, y].Food + worldMan.worldMap.mapTiles[x, y + 1].Food + worldMan.worldMap.mapTiles[x, y - 1].Food;
                        float LocalWood = worldMan.worldMap.mapTiles[x, y].Wood + worldMan.worldMap.mapTiles[x + 1, y].Wood + worldMan.worldMap.mapTiles[x - 1, y].Wood + worldMan.worldMap.mapTiles[x, y + 1].Wood + worldMan.worldMap.mapTiles[x, y - 1].Wood;
                        float LocalLife = (LocalFood * 2) + (LocalWood / 2);
                        worldMan.worldMap.mapTiles[x, y].lifeScore = LocalLife;
                        if (LocalLife >= bestLife)
                        {
                            bestTile.x = x;
                            bestTile.y = y;
                            bestLife = LocalLife;
                        }
                    }
                }
                townLocs[i] = bestTile;
                Vector3Int townPos = new Vector3Int(bestTile.x, bestTile.y, 0);
                townMap.SetTile(townPos, town);
                worldMan.CreateTown(bestTile.x, bestTile.y, i.ToString(), 2, true);
                bestLife = 0;
            }
            
        }
    }
    private void OnValidate()
    {
        falloffMap = FalloffGenerator.GeneratorFalloffMap(size);
        if (size < 1)
        {
            size = 1;
        }
        if (lanunarity < 1)
        {
            lanunarity = 1;
        }
        if (octaves < 0)
        {
            octaves = 0;
        }
    }
}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public float rain;
    public Color colour;
}

[System.Serializable]
public struct SeaTerrains
{
    public string name;
    public float height;
    public Color colour;
    public Tile tile;
    [Range(0, 5)]
    public int FoodMod;
    [Range(0, 5)]
    public int TradeMod;
    [Range(0, 5)]
    public int StoneMod;
    [Range(0, 5)]
    public int WoodMod;
    [Range(0, 5)]
    public float OreMod;
}

[System.Serializable]
public struct MountTerrains
{
    public string name;
    public float height;
    public Color colour;
    public Tile tile;
    [Range(0, 5)]
    public int FoodMod;
    [Range(0, 5)]
    public int TradeMod;
    [Range(0, 5)]
    public int StoneMod;
    [Range(0, 5)]
    public int WoodMod;
    [Range(0, 5)]
    public int OreMod;
}

[System.Serializable]
public struct HeatTerrains
{
    public string name;
    public float height;
    public Color colour;
    public int index;
}

[System.Serializable]
public struct rainTerrains
{
    public string name;
public float height;
public Color colour;
}

[System.Serializable]
public struct Biome
{
    public string name;
    public Color color;
    public Tile tileC;
    public Tile tileW;
    public Tile tileE;
    public Tile tileN;
    public Tile tileS;
    public Tile tileNE;
    public Tile tileNEIn;
    public Tile tileNW;
    public Tile tileNWIn;
    public Tile tileSE;
    public Tile tileSEIn;
    public Tile tileSW;
    public Tile tileSWIn;
    public bool forest;
    [Range(0, 5)]
    public int FoodMod;
    [Range(0, 5)]
    public int TradeMod;
    [Range(0, 5)]
    public int StoneMod;
    [Range(0, 5)]
    public int WoodMod;
    [Range(0, 5)]
    public int OreMod;
}

[System.Serializable]
public struct BiomeRow
{
    public Biome[] biomes;
}
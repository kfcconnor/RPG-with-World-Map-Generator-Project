using System.Collections;
using System.Collections.Generic;
using System.IO;
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

    public int numOfNations;

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

    public disallowedTerains[] checkTerrains;
    public TerrainType[] regions;
    public SeaTerrains[] seaTerrains;
    public MountTerrains[] mountTerrains;
    public HeatTerrains[] heatTerrains;
    public rainTerrains[] rainTerrains;
    public tileGroup[] tileGroups;
    public BiomeDic biomes;

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
        populateBiomes();
        worldMapMan.worldMap.mapTiles = new MapTile[size, size];
        //worldMapMan.Debug = new Town[worldMapMan.NumNations];
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
        //bool tilefound = false;

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
                Vector2 tileCond = new Vector2();
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
                //worldMapMan.worldMap.mapTiles[x, y].oreTypes = new List<ore>();
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
                            worldMapMan.worldMap.mapTiles[x,y].biomeId = getBiomeID(biomes.biomeData, terrainType, true, currentHeight, tileCond);
                            worldMapMan.worldMap.mapTiles[x, y] = mapTile(worldMapMan.worldMap.mapTiles[x, y], biomes.biomeData, x, y, resources);
                            colourMap[y * size + x] = biomes.biomeData[worldMapMan.worldMap.mapTiles[x, y].biomeId].color;
                            break;
                        }
                    case 1:
                        {

                            worldMapMan.worldMap.mapTiles[x, y].biomeId = getBiomeID(biomes.biomeData, terrainType, true, currentHeight, tileCond);
                            worldMapMan.worldMap.mapTiles[x, y] = mapTile(worldMapMan.worldMap.mapTiles[x, y], biomes.biomeData, x, y, resources);
                            colourMap[y * size + x] = biomes.biomeData[worldMapMan.worldMap.mapTiles[x, y].biomeId].color;
                            //System.Random oreChance = new System.Random();
                            //foreach (ore o in metalMan.OreList)
                            //{
                            //    int typeChance = (resources.Next(0, 100));
                            //    if (typeChance < o.rarity*100)
                            //    {
                            //        worldMapMan.worldMap.mapTiles[x, y].oreTypes.Add(o);
                            //    }
                            //}
                            //worldMapMan.worldMap.Debug[y * size + x] = worldMapMan.worldMap.mapTiles[x, y];
                            //worldMap.SetTile(location, mountTerrains[i].tile);
                            //break;
                            break;
                        }
                    case 2:
                        {
                            worldMapMan.worldMap.mapTiles[x, y].BiomeType = 2;
                            for (int i = 0; i < heatTerrains.Length; i++)
                            {
                                if (currentHeat <= heatTerrains[i].height)
                                {
                                    tileCond.y = heatTerrains[i].index;
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
                                        tileCond.x = 0;

                                        worldMapMan.worldMap.mapTiles[x, y].biomeId = getBiomeID(biomes.biomeData, terrainType, false, 0, tileCond);

                                        worldMapMan.worldMap.mapTiles[x, y] = mapTile(worldMapMan.worldMap.mapTiles[x, y], biomes.biomeData, x, y, resources);
                                        colourMap[y * size + x] = biomes.biomeData[worldMapMan.worldMap.mapTiles[x,y].biomeId].color;
                                        //worldMapMan.worldMap.Debug[y * size + x] = worldMapMan.worldMap.mapTiles[x, y];
                                        //worldMap.SetTile(location, biomeRows[0].biomes[Heat].tile);
                                        break;
                                    }
                                case "Dry":
                                    {
                                        tileCond.x = 1;
                                        //colourMap[y * size + x] = biomeRows[1].biomes[Heat].color;
                                        worldMapMan.worldMap.mapTiles[x, y].biomeId = getBiomeID(biomes.biomeData, terrainType, false, 0, tileCond);
                                        worldMapMan.worldMap.mapTiles[x, y] = mapTile(worldMapMan.worldMap.mapTiles[x, y], biomes.biomeData, x, y, resources);
                                        colourMap[y * size + x] = biomes.biomeData[worldMapMan.worldMap.mapTiles[x, y].biomeId].color;
                                        //worldMapMan.worldMap.Debug[y * size + x] = worldMapMan.worldMap.mapTiles[x, y];
                                        //worldMap.SetTile(location, biomeRows[1].biomes[Heat].tile);
                                        break;
                                    }
                                case "Wet":
                                    {
                                        tileCond.x = 2;
                                        //colourMap[y * size + x] = biomeRows[2].biomes[Heat].color;
                                        worldMapMan.worldMap.mapTiles[x, y].biomeId = getBiomeID(biomes.biomeData, terrainType, false, 0, tileCond);

                                        worldMapMan.worldMap.mapTiles[x, y] = mapTile(worldMapMan.worldMap.mapTiles[x, y], biomes.biomeData, x, y, resources);
                                        colourMap[y * size + x] = biomes.biomeData[worldMapMan.worldMap.mapTiles[x, y].biomeId].color;
                                        //worldMapMan.worldMap.Debug[y * size + x] = worldMapMan.worldMap.mapTiles[x, y];
                                        //worldMap.SetTile(location, biomeRows[2].biomes[Heat].tile);
                                        break;
                                    }
                                case "Wettest":
                                    {
                                        tileCond.x = 3;
                                        //colourMap[y * size + x] = biomeRows[3].biomes[Heat].color;
                                        worldMapMan.worldMap.mapTiles[x, y].biomeId = getBiomeID(biomes.biomeData, terrainType, false, 0, tileCond);

                                        worldMapMan.worldMap.mapTiles[x, y] = mapTile(worldMapMan.worldMap.mapTiles[x, y], biomes.biomeData, x, y, resources);
                                        colourMap[y * size + x] = biomes.biomeData[worldMapMan.worldMap.mapTiles[x, y].biomeId].color;
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

        //mapClean(worldMapMan, size, colourMap);

        worldMapMan.nationSetup(numOfNations);
        capGen.captialPos(worldMapMan, size, numOfNations, TownMap, town);
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
            TextureGenerator.SpriteMapFromTileData(worldMapMan.worldMap, colourMap, size, worldMap, coastWater, waterMap, treeMap, mountMap, tileGroups, biomes.biomeData);
        }
        //tileCheck(worldMapMan, size, 0, 0);
        //tileCheck(worldMapMan, size, 150, 150);
        //tileCheck(worldMapMan, size, 50, 100);
        //Debug.Log(Directory.GetCurrentDirectory());
        //worldMapMan.saveMap(size);
    }

    public MapTile mapTile(MapTile currentTile, Dictionary<int, Biome> biomes , int x, int y, System.Random resources)
    {
        currentTile.coord.x = x;
        currentTile.coord.y = y;
        currentTile.Biome = biomes[currentTile.biomeId].name;
        currentTile.ownerID = 0;
        currentTile.BiomeType = biomes[currentTile.biomeId].biomeType;
        currentTile.lifeScore = resources.Next(0, 50) * biomes[currentTile.biomeId].lifeMod;
        currentTile.Ore = 0;
        return currentTile;
    }


    public int getBiomeID(Dictionary<int, Biome> biomes, int biomeType, bool Height,float hValue, Vector2 tileCond)
    {
        int biomeID = 0;
        bool tilefound = false;

        if (Height)
        {
            for (int i = 0; i < biomes.Count; i++)
            {
                if(biomeType == 0)
                {
                    if (biomeType == biomes[i].biomeType && biomes[i].tag == "Water")
                    {
                        if (hValue < biomes[i].maxHeight)
                        {
                            biomeID = i;
                            break;
                        }
                    }
                }
                else if (biomeType == 1)
                {
                    if (biomeType == biomes[i].biomeType && biomes[i].tag == "Mount")
                    {
                        if (hValue < biomes[i].maxHeight)
                        {
                            biomeID = i;
                            break;
                        }
                    }
                }
                
            }
        }
        else
        {
            for (int i = 0; i < biomes.Count; i++)
            {
                foreach (Vector2 c in biomes[i].conditions)
                {
                    if (tileCond == c)
                    {
                        biomeID = i;
                        tilefound = true;
                        break;
                    }
                }

                if (tilefound)
                {
                    tilefound = false;
                    break;
                }
            }
        }

        return biomeID;
    }

    //public void mapClean(WorldMapManager worldMan, int size, Color[] colourMap)
    //{

    //    int checkSize = checkTerrains.Length;
    //    int xCount = 0;
    //    int yCount = 2;
    //    List<int[,]> dTerrains = new List<int[,]>();
    //    int[,] currentDTerrain = new int[3, 3];
    //    for (int i = 0; i < checkSize; i++)
    //    {
    //        foreach(int j in checkTerrains[i].terrainMap)
    //        {
    //            if (xCount == 3)
    //            {
    //                yCount--;
    //                xCount = 0;
    //            }

    //            currentDTerrain[xCount, yCount] = j;

    //        }
    //        dTerrains.Add(currentDTerrain);
    //        currentDTerrain = new int[3, 3];
    //    }

    //    for (int y = 0; y < size; y++)
    //    {
    //        for (int x = 0; x < size; x++)
    //        {
    //            if (worldMan.worldMap.mapTiles[x, y].BiomeType == 0)
    //            {
    //                continue;
    //            }
    //            else
    //            {
    //                int[,] currentCheck = tileCheck(worldMan, size, x, y);
    //                foreach(int[,] check in dTerrains)
    //                {
    //                    if(currentCheck == check)
    //                    {
    //                        worldMan.worldMap.mapTiles[x, y].toClean = true;
    //                    }
    //                }
    //            }

    //        }
    //    }

    //    for (int y = 0; y < size; y++)
    //    {
    //        for (int x = 0; x < size; x++)
    //        {
    //            if (worldMan.worldMap.mapTiles[x, y].toClean)
    //            {
    //                colourMap[y * size + x] = seaTerrains[0].colour;
    //                worldMan.worldMap.mapTiles[x, y].BiomeType = 0;
    //                worldMapMan.worldMap.mapTiles[x, y].Biome = seaTerrains[0].name;
    //                worldMapMan.worldMap.mapTiles[x, y].tile = seaTerrains[0].tile;
    //            }

    //        }
    //    }
    //}

    public int[,] tileCheck(WorldMapManager worldMan, int size, int tileX, int tileY)
    {
        int[,] surroundingTileTypes = new int[3,3];
        int xCount = 0;
        int yCount = 2;


        for (int i = tileY + 1; i >= tileY - 1; i--)
        {
            for (int j = tileX - 1; j <=tileX + 1; j++)
            {
                if (i < 0 || i >= size || j < 0 || j >=size)
                {
                    surroundingTileTypes[xCount, yCount] = 3;
                }
                else if(worldMan.worldMap.mapTiles[i,j].BiomeType == 0)
                {
                    surroundingTileTypes[xCount, yCount] = 0;
                }
                else if(worldMan.worldMap.mapTiles[i, j].BiomeType == 1 || worldMan.worldMap.mapTiles[i, j].BiomeType == 2)
                {
                    surroundingTileTypes[xCount, yCount] = 1;
                }

                xCount++;
            }
            xCount = 0;
            yCount--;
        }
        return surroundingTileTypes;
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
                        if (worldMan.worldMap.mapTiles[x, y].BiomeType == 0 || worldMan.worldMap.mapTiles[x, y].BiomeType == 1 || worldMan.worldMap.mapTiles[x, y].town)
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
                        float LocalLife = worldMan.worldMap.mapTiles[x, y].lifeScore;
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
                worldMan.addTown(i + 1, true, 0, bestTile.x, bestTile.y);
                //worldMan.CreateTown(bestTile.x, bestTile.y, i.ToString(), 2, true);
                bestLife = 0;
            }
            
        }
    }

    public void populateBiomes()
    {
        int id = 0;

        biomes.biomeData = new Dictionary<int, Biome>();
        Vector2 cond = new Vector2();
        Biome currentBiome = new Biome();
        currentBiome.conditions = new List<Vector2>();
        currentBiome.name = "Desert";
        currentBiome.color.r = 207f / 255f;
        currentBiome.color.g = 209f / 255f;
        currentBiome.color.b = 125f / 255f;
        currentBiome.biomeType = 2;
        currentBiome.lifeMod = 0.75f;
        currentBiome.tileGroupName = "Desert";
        currentBiome.forest = false;
        cond = new Vector2(0, 0);
        currentBiome.conditions.Add(cond);
        biomes.biomeData.Add(id, currentBiome);
        id++;

        currentBiome = new Biome();
        currentBiome.conditions = new List<Vector2>();
        currentBiome.name = "Grassland";
        currentBiome.color.r = 88f / 255f;
        currentBiome.color.g = 151f / 255f;
        currentBiome.color.b = 22f / 255f;
        currentBiome.biomeType = 2;
        currentBiome.lifeMod = 1f;
        currentBiome.tileGroupName = "Grassland";
        currentBiome.forest = false;
        cond = new Vector2(0, 1);
        currentBiome.conditions.Add(cond);
        biomes.biomeData.Add(id, currentBiome);
        id++;

        currentBiome = new Biome();
        currentBiome.conditions = new List<Vector2>();
        currentBiome.name = "Tundra";
        currentBiome.color.r = 217f / 255f;
        currentBiome.color.g = 189f / 255f;
        currentBiome.color.b = 189f / 255f;
        currentBiome.biomeType = 2;
        currentBiome.lifeMod = 0.75f;
        currentBiome.tileGroupName = "Tundra";
        currentBiome.heightBiome = false;
        cond = new Vector2(0, 2);
        currentBiome.conditions.Add(cond);
        cond = new Vector2(0, 3);
        currentBiome.conditions.Add(cond);
        cond = new Vector2(1, 3);
        currentBiome.conditions.Add(cond);
        cond = new Vector2(2, 3);
        currentBiome.conditions.Add(cond);
        cond = new Vector2(3, 2);
        currentBiome.conditions.Add(cond);
        cond = new Vector2(3, 3);
        currentBiome.conditions.Add(cond);
        biomes.biomeData.Add(id, currentBiome);
        id++;

        currentBiome = new Biome();
        currentBiome.conditions = new List<Vector2>();
        currentBiome.name = "Savanna";
        currentBiome.color.r = 173f / 255f;
        currentBiome.color.g = 217f / 255f;
        currentBiome.color.b = 119f / 255f;
        currentBiome.lifeMod = 1.25f;
        currentBiome.biomeType = 2;
        currentBiome.tileGroupName = "Grassland";
        currentBiome.heightBiome = false;
        cond = new Vector2(1, 0);
        currentBiome.conditions.Add(cond);
        cond = new Vector2(1, 1);
        currentBiome.conditions.Add(cond);
        biomes.biomeData.Add(id, currentBiome);
        id++;

        currentBiome = new Biome();
        currentBiome.conditions = new List<Vector2>();
        currentBiome.name = "Boreal Forest";
        currentBiome.color.r = 57f / 255f;
        currentBiome.color.g = 89f / 255f;
        currentBiome.color.b = 25f / 255f;
        currentBiome.biomeType = 3;
        currentBiome.lifeMod = 1.1f;
        currentBiome.tileGroupName = "Boreal Forest";
        currentBiome.heightBiome = false;
        cond = new Vector2(1, 2);
        currentBiome.conditions.Add(cond);
        cond = new Vector2(2, 1);
        currentBiome.conditions.Add(cond);
        cond = new Vector2(2, 2);
        currentBiome.conditions.Add(cond);
        biomes.biomeData.Add(id, currentBiome);
        id++;

        currentBiome = new Biome();
        currentBiome.conditions = new List<Vector2>();
        currentBiome.name = "Tropical Rainforest";
        currentBiome.color.r = 32f/255f;
        currentBiome.color.g = 173f / 255f;
        currentBiome.color.b = 17f / 255f;
        currentBiome.biomeType = 3;
        currentBiome.lifeMod = 0.75f;
        currentBiome.tileGroupName = "Tropical Rainforest";
        currentBiome.heightBiome = false;
        cond = new Vector2(2, 0);
        currentBiome.conditions.Add(cond);
        cond = new Vector2(3, 0);
        currentBiome.conditions.Add(cond);
        cond = new Vector2(3, 1);
        currentBiome.conditions.Add(cond);
        biomes.biomeData.Add(id, currentBiome);
        id++;

        currentBiome = new Biome();
        currentBiome.conditions = new List<Vector2>();
        currentBiome.name = "Water Deep";
        currentBiome.tag = "Water";
        currentBiome.color.r = 52f / 255f;
        currentBiome.color.g = 98f / 255f;
        currentBiome.color.b = 194f / 255f;
        currentBiome.biomeType = 0;
        currentBiome.lifeMod = 0f;
        currentBiome.tileGroupName = "Water";
        currentBiome.forest = true;
        currentBiome.maxHeight = 0.4f;
        currentBiome.conditions.Add(cond);
        biomes.biomeData.Add(id, currentBiome);
        id++;

        currentBiome = new Biome();
        currentBiome.conditions = new List<Vector2>();
        currentBiome.name = "Water Shallow";
        currentBiome.tag = "Water";
        currentBiome.color.r = 100f / 255f;
        currentBiome.color.g = 100f / 255f;
        currentBiome.color.b = 255f / 255f;
        currentBiome.biomeType = 0;
        currentBiome.lifeMod = 0f;
        currentBiome.tileGroupName = "Water";
        currentBiome.forest = true;
        currentBiome.maxHeight = 0.7f;
        biomes.biomeData.Add(id, currentBiome);
        id++;

        currentBiome = new Biome();
        currentBiome.conditions = new List<Vector2>();
        currentBiome.name = "Rock 1";
        currentBiome.tag = "Mount";
        currentBiome.color.r = 91f / 255f;
        currentBiome.color.g = 68f / 255f;
        currentBiome.color.b = 61f / 255f;
        currentBiome.biomeType = 1;
        currentBiome.lifeMod = 0f;
        currentBiome.tileGroupName = "Hills";
        currentBiome.forest = true;
        currentBiome.maxHeight = 0.8f;
        biomes.biomeData.Add(id, currentBiome);
        id++;

        currentBiome = new Biome();
        currentBiome.conditions = new List<Vector2>();
        currentBiome.name = "Rock 2";
        currentBiome.tag = "Mount";
        currentBiome.color.r = 65f / 255f;
        currentBiome.color.g = 50f / 255f;
        currentBiome.color.b = 50f / 255f;
        currentBiome.biomeType = 1;
        currentBiome.lifeMod = 0f;
        currentBiome.tileGroupName = "Mountain_1";
        currentBiome.forest = true;
        currentBiome.maxHeight = 0.9f;
        biomes.biomeData.Add(id, currentBiome);
        id++;

        currentBiome = new Biome();
        currentBiome.conditions = new List<Vector2>();
        currentBiome.name = "Snow";
        currentBiome.tag = "Mount";
        currentBiome.color.r = 255f / 255f;
        currentBiome.color.g = 255f / 255f;
        currentBiome.color.b = 255f / 255f;
        currentBiome.biomeType = 1;
        currentBiome.lifeMod = 0f;
        currentBiome.tileGroupName = "Mountain_2";
        currentBiome.forest = true;
        currentBiome.maxHeight = 1f;
        biomes.biomeData.Add(id, currentBiome);
        id++;
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
public struct disallowedTerains
{
    public int[] terrainMap;
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
    public string tileGroupName;
    public string tag;
    public int biomeType;
    public bool heightBiome;
    public float maxHeight;
    public bool forest;
    public List<Vector2> conditions;
    [Range(0, 2)]
    public float lifeMod;
}

[System.Serializable]
public struct BiomeDic
{
    public Dictionary<int, Biome> biomeData;
}

[System.Serializable]
public struct tileGroup
{
    public string groupName;
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
}
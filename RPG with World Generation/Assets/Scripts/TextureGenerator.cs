using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class TextureGenerator
{

    public static Texture2D TextureFromColourMap(Color[] colourMap, int size)
    {
        Texture2D texture = new Texture2D(size, size);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colourMap);
        texture.Apply();
        return texture;
    }

    public static Texture2D TextureFromHeightMap(float[,] heightMap)
    {
        int size = heightMap.GetLength(0);



        Color[] colourMap = new Color[size * size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                colourMap[y * size + x] = Color.Lerp(Color.black, Color.white, heightMap[x, y]);
            }
        }

        return TextureFromColourMap(colourMap, size);

    }
    public static void coastGeneration(WorldMap world, int locX, int locY, int size, Tile waterCoast, Tilemap waterMap, tileGroup[] tilegroups, Dictionary<int, Biome> biomeList, Tilemap worldMap)
    {
        NearTile near = new NearTile();
        int i = 0;
        List<string> waterLocs = new List<string>();
        bool[] isWater = new bool[8];
        int tileGroup = getTilegroup(world.mapTiles[locX, locY],biomeList,tilegroups);

        for (int y = 1; y >= -1; y--)
        {
            for (int x = 1; x >= -1; x--)
            {
                if (!(x == 0 && y == 0))
                {
                    int trueX = locX + x;
                    int trueY = locY + y;
                    if ((trueX >= 0 && trueX < size) && (trueY >= 0 && trueY < size))
                    {
                        switch (i)
                        {
                            case 0:
                                {
                                    near.isNW = true;
                                    near.NW = world.mapTiles[trueX, trueY];
                                    break;
                                }
                            case 1:
                                {
                                    near.isN = true;
                                    near.N = world.mapTiles[trueX, trueY];
                                    break;
                                }
                            case 2:
                                {
                                    near.isNE = true;
                                    near.NE = world.mapTiles[trueX, trueY];
                                    break;
                                }
                            case 3:
                                {
                                    near.isW = true;
                                    near.W = world.mapTiles[trueX, trueY];
                                    break;
                                }
                            case 4:
                                {
                                    near.isE = true;
                                    near.E = world.mapTiles[trueX, trueY];
                                    break;
                                }
                            case 5:
                                {
                                    near.isSW = true;
                                    near.SW = world.mapTiles[trueX, trueY];
                                    break;
                                }
                            case 6:
                                {
                                    near.isS = true;
                                    near.S = world.mapTiles[trueX, trueY];
                                    break;
                                }
                            case 7:
                                {
                                    near.isSE = true;
                                    near.SE = world.mapTiles[trueX, trueY];
                                    break;
                                }
                        }
                    }
                    else
                    {
                        switch (i)
                        {
                            case 0:
                                {
                                    near.isNW = false;
                                    break;
                                }
                            case 1:
                                {
                                    near.isN = false;
                                    break;
                                }
                            case 2:
                                {
                                    near.isNE = false;
                                    break;
                                }
                            case 3:
                                {
                                    near.isW = false;
                                    break;
                                }
                            case 4:
                                {
                                    near.isE = false;
                                    break;
                                }
                            case 5:
                                {
                                    near.isSW = false;
                                    break;
                                }
                            case 6:
                                {
                                    near.isS = false;
                                    break;
                                }
                            case 7:
                                {
                                    near.isSE = false;
                                    break;
                                }
                        }
                    }
                    i++;
                }

            }
        }


        for (i = 0; i < 8; i++)
        {
            switch (i)
            {
                case 0:
                    {
                        if (near.isNW)
                        {
                            if (near.NW.BiomeType == 0)
                            {
                                waterLocs.Add("NW");
                                isWater[i] = true;
                            }
                            else
                            {
                                isWater[i] = false;
                            }
                        }
                        else
                        {
                            isWater[i] = false;
                        }
                        break;
                    }
                case 1:
                    {
                        if (near.isN)
                        {
                            if (near.N.BiomeType == 0)
                            {
                                waterLocs.Add("N");
                                isWater[i] = true;
                            }
                            else
                            {
                                isWater[i] = false;
                            }
                        }
                        else
                        {
                            isWater[i] = false;
                        }
                        break;
                    }
                case 2:
                    {
                        if (near.isNE)
                        {
                            if (near.NE.BiomeType == 0)
                            {
                                waterLocs.Add("NE");
                                isWater[i] = true;
                            }
                            else
                            {
                                isWater[i] = false;
                            }
                        }
                        else
                        {
                            isWater[i] = false;
                        }
                        break;
                    }
                case 3:
                    {
                        if (near.isW)
                        {
                            if (near.W.BiomeType == 0)
                            {
                                waterLocs.Add("W");
                                isWater[i] = true;
                            }
                            else
                            {
                                isWater[i] = false;
                            }
                        }
                        else
                        {
                            isWater[i] = false;
                        }
                        break;
                    }
                case 4:
                    {
                        if (near.isE)
                        {
                            if (near.E.BiomeType == 0)
                            {
                                waterLocs.Add("E");
                                isWater[i] = true;
                            }
                            else
                            {
                                isWater[i] = false;
                            }
                        }
                        else
                        {
                            isWater[i] = false;
                        }
                        break;
                    }
                case 5:
                    {
                        if (near.isSW)
                        {
                            if (near.SW.BiomeType == 0)
                            {
                                waterLocs.Add("SW");
                                isWater[i] = true;
                            }
                            else
                            {
                                isWater[i] = false;
                            }
                        }
                        else
                        {
                            isWater[i] = false;
                        }
                        break;
                    }
                case 6:
                    {
                        if (near.isS)
                        {
                            if (near.S.BiomeType == 0)
                            {
                                waterLocs.Add("S");
                                isWater[i] = true;
                            }
                            else
                            {
                                isWater[i] = false;
                            }
                        }
                        else
                        {
                            isWater[i] = false;
                        }
                        break;
                    }
                case 7:
                    {
                        if (near.isSE)
                        {
                            if (near.SE.BiomeType == 0)
                            {
                                waterLocs.Add("SE");
                                isWater[i] = true;
                            }
                            else
                            {
                                isWater[i] = false;
                            }
                        }
                        else
                        {
                            isWater[i] = false;
                        }
                        break;
                    }
            }
        }
        Vector3Int loc = new Vector3Int(locX, locY, 0);
        if (waterLocs.Count == 0 || waterLocs.Count == 8)
        {
            worldMap.SetTile(loc, tilegroups[tileGroup].tileC);
        }
        else
        {
            
            waterMap.SetTile(loc, waterCoast);
            if (isWater[0] && isWater[1] && isWater[2] || isWater[1] || isWater[1] && isWater[2] || isWater[1] && isWater[0])
            {
                worldMap.SetTile(loc, tilegroups[tileGroup].tileN);
            }
            else if (isWater[5] && isWater[6] && isWater[7] || isWater[6] || isWater[6] && isWater[5] || isWater[6] && isWater[7])
            {
                worldMap.SetTile(loc,tilegroups[tileGroup].tileS);
            }
            else if (isWater[0] && isWater[3] && isWater[5] || isWater[3] || isWater[3] && isWater[0] || isWater[3] && isWater[5])
            {
                worldMap.SetTile(loc, tilegroups[tileGroup].tileE);
            }
            else if (isWater[2] && isWater[4] && isWater[7] || isWater[4] || isWater[4] && isWater[2] || isWater[4] && isWater[7])
            {
                worldMap.SetTile(loc, tilegroups[tileGroup].tileW);
            }

            if (isWater[0] && isWater[1] && isWater[3])
            {
                worldMap.SetTile(loc, tilegroups[tileGroup].tileNE);
            }
            else if (isWater[0] && waterLocs.Count == 1)
            {
                worldMap.SetTile(loc, tilegroups[tileGroup].tileNEIn);
            }
            else if (isWater[4] && isWater[1] && isWater[2])
            {
                worldMap.SetTile(loc, tilegroups[tileGroup].tileNW);
            }
            else if (isWater[2] && waterLocs.Count == 1)
            {
                worldMap.SetTile(loc, tilegroups[tileGroup].tileNWIn);
            }
            else if (isWater[3] && isWater[5] && isWater[6])
            {
                worldMap.SetTile(loc, tilegroups[tileGroup].tileSE);
            }
            else if (isWater[5] && waterLocs.Count == 1)
            {
                worldMap.SetTile(loc, tilegroups[tileGroup].tileSEIn);
            }
            else if (isWater[4] && isWater[7] && isWater[6])
            {
                worldMap.SetTile(loc, tilegroups[tileGroup].tileSW);
            }
            else if (isWater[7] && waterLocs.Count == 1)
            {
                worldMap.SetTile(loc, tilegroups[tileGroup].tileSWIn);
            }

        }

    }
    public static void SpriteMapFromTileData(WorldMap world, Color[] colourMap, int size, Tilemap worldMap, Tile coastWater, Tilemap waterMap, Tilemap treeMap, Tilemap mountMap, tileGroup[] tilegroups, Dictionary<int, Biome> biomesList)
    {

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Vector3Int location = new Vector3Int(x, y, 0);
                int tileGroup = getTilegroup(world.mapTiles[x, y], biomesList, tilegroups);
                if (world.mapTiles[x, y].BiomeType == 0)
                {
                    waterMap.SetTile(location, tilegroups[tileGroup].tileC);
                }
                else if (world.mapTiles[x, y].BiomeType == 1)
                {
                    mountMap.SetTile(location, tilegroups[tileGroup].tileC);
                }
                else if (world.mapTiles[x, y].BiomeType == 2)
                {
                    coastGeneration(world, x, y, size, coastWater, waterMap, tilegroups, biomesList, worldMap);
                }
                else if (world.mapTiles[x, y].BiomeType == 3)
                {
                    treeMap.SetTile(location, tilegroups[tileGroup].tileC);
                }
                //worldMap.SetTile(location, world.mapTiles[x, y].tile);
                //worldMap.SetColor(location, colourMap[y * size + x]);
            }
        }
    }

    public static int getTilegroup(MapTile currentTile, Dictionary<int, Biome> biomes, tileGroup[] tileGroups)
    {
        int tileGroup = 0;
        for (int j = 0; j < tileGroups.Length; j++)
        {
            if (biomes[currentTile.biomeId].tileGroupName == tileGroups[j].groupName)
            {
                tileGroup = j;
                break;
            }
        }

        return tileGroup;
    }

}

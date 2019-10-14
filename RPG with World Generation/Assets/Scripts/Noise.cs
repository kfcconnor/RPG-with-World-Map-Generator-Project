using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    public static float[,] GenerateNoiseMap(int size, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset)
    {
        float[,] noiseMap = new float[size, size];

        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffests = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) + offset.y;
            octaveOffests[i] = new Vector2(offsetX, offsetY);
        }

        if (scale <=0)
        {
            scale = 0.0001f;
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = size / 2f;
        float halfHeight = size / 2f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float amplitude = 1;
                float frequency = 0.75f;
                float noiseHeight = 0;
                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x - halfWidth) / scale * frequency + octaveOffests[i].x;
                    float sampleY = (y - halfHeight) / scale * frequency + octaveOffests[i].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }
                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }
                noiseMap[x, y] = noiseHeight;
            }
        }

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }

        return noiseMap;
    }

public static float[,] GenerateUniformNoiseMap(int size, float centerVertexZ, float maxDistanceZ, float offsetZ)
    {
        // create an empty noise map with the mapDepth and mapWidth coordinates
        float[,] noiseMap = new float[size, size];

        for (int zIndex = 0; zIndex < size; zIndex++)
        {
            // calculate the sampleZ by summing the index and the offset
            float sampleZ = zIndex + offsetZ;
            // calculate the noise proportional to the distance of the sample to the center of the level
            float noise = Mathf.Abs(sampleZ - centerVertexZ) / maxDistanceZ;
            // apply the noise for all points with this Z coordinate
            for (int xIndex = 0; xIndex < size; xIndex++)
            {
                noiseMap[size - zIndex - 1, xIndex] = noise;
            }
        }

        return noiseMap;
    }
}

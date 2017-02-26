using System.Collections.Generic;
using UnityEngine;

namespace Sandbox
{
    public class PerlinNoizeGenerator
    {
        Vector2Int sampleOffset;
        int worldHeight;
        float scale = 1.0f;
        int octaves = 1;
        float persistance = 1f;
        float lacunarity = 1f;
        public PerlinNoizeGenerator(Vector2Int sampleOffset, int worldHeight, float scale,int octaves, float persistance, float lacunarity)
        {
            this.sampleOffset = sampleOffset;
            this.worldHeight = worldHeight;
            this.scale = scale;
            this.octaves = octaves;
            this.persistance = persistance;
            this.lacunarity = lacunarity;
        } 

        public int GetHeight(int x)
        {
            float frequency = 1f;
            float amplitude = 1f;
            float noizeHeight = 0;
            for (int i = 0; i < octaves; i++)
            {
                var sampleX = (x + sampleOffset.x) / scale * frequency;
                //Умножение на 2 с последующим вычитанием 1 дает нам диапазон значений [-1:1]
                var perlinValue = Mathf.PerlinNoise(sampleX, 0) * 2 - 1;
                noizeHeight += perlinValue * amplitude;
                amplitude *= persistance;
                frequency *= lacunarity;
            }
            
            return Mathf.FloorToInt(Mathf.InverseLerp(-worldHeight /2, worldHeight / 2, noizeHeight * worldHeight) * worldHeight);
        }
    }
}
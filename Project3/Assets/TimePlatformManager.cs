using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimePlatformManager : MonoBehaviour
{

    public List<TimePlatform> platforms = new List<TimePlatform>();

    static int tilemapsWithoutGreenPlatformCount = 0;
    const int maxTilemapsWithoutGreenPlatformCount = 1;

    struct PlatformDifficultyConfig
    {
        public float startSecond;
        public int minGreen;
        public int maxGreen;
        public int minRed;
        public int maxRed;

    }

    float[] seconds = new float[] {0, 5, 10, 15, 20, 30, 40, 50, 60};

    PlatformDifficultyConfig[] configs = new PlatformDifficultyConfig[]
    {
        new PlatformDifficultyConfig{ startSecond = 0f,  minGreen = 2, maxGreen = 3, minRed = 0, maxRed = 0},
        new PlatformDifficultyConfig{ startSecond = 5f,  minGreen = 2, maxGreen = 2, minRed = 0, maxRed = 1},
        new PlatformDifficultyConfig{ startSecond = 10f, minGreen = 1, maxGreen = 2, minRed = 1, maxRed = 2},
        new PlatformDifficultyConfig{ startSecond = 15f, minGreen = 1, maxGreen = 2, minRed = 1, maxRed = 2},
        new PlatformDifficultyConfig{ startSecond = 20f, minGreen = 0, maxGreen = 1, minRed = 1, maxRed = 2},
        new PlatformDifficultyConfig{ startSecond = 30f, minGreen = 0, maxGreen = 1, minRed = 2, maxRed = 3},
        new PlatformDifficultyConfig{ startSecond = 40f, minGreen = 0, maxGreen = 1, minRed = 2, maxRed = 4},
        new PlatformDifficultyConfig{ startSecond = 50f, minGreen = 0, maxGreen = 1, minRed = 3, maxRed = 5},
        new PlatformDifficultyConfig{ startSecond = 60f, minGreen = 0, maxGreen = 1, minRed = 3, maxRed = 6},

    };


    // Start is called before the first frame update
    void Start()
    {
        if(GameInfo.instance != null)
            GameInfo.instance.StartTimeCounter();


        
        PlatformDifficultyConfig currentDifConfig = configs[0];
        foreach(var config in configs){
            if(config.startSecond <= GameInfo.matchTimeCounter)
            {
                currentDifConfig = config;
            }else
            {
                break;
            }
        }


        int greenToSpawn = Random.Range(currentDifConfig.minGreen, currentDifConfig.maxGreen);

        if(greenToSpawn == 0)
        {
            if(tilemapsWithoutGreenPlatformCount >= maxTilemapsWithoutGreenPlatformCount)
            {
                tilemapsWithoutGreenPlatformCount = 0;
                greenToSpawn = 1;

            }else
            {
                tilemapsWithoutGreenPlatformCount++;
            }
        }

        int redToSpawn = Random.Range(currentDifConfig.minRed, currentDifConfig.maxRed);

        while (greenToSpawn > 0)
        {
            int choosenIndex = Random.Range(0, platforms.Count);
            platforms[choosenIndex].SetGreen();
            platforms.RemoveAt(choosenIndex);
            greenToSpawn--;

        }

        while (redToSpawn > 0)
        {
            int choosenIndex = Random.Range(0, platforms.Count);
            platforms[choosenIndex].SetRed();
            platforms.RemoveAt(choosenIndex);

            redToSpawn--;
        }

        foreach (var platform in platforms)
            Destroy(platform);
    

    }

    
}

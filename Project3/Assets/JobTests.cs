using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

// Job adding two floating point values together
public struct MyJob : IJob
{
    public NativeArray<float> a;
    public NativeArray<float> randomNumbers;

    
    public static float minRange = 0.2f;
    public static float maxRange = 0.2f;
    public int lastRandomGiven;


    float RandomFloat()
    {

        float result = randomNumbers[lastRandomGiven++];
        lastRandomGiven = lastRandomGiven % randomNumbers.Length;

        return result;
    }



    public void Execute()
    {
        a[0] = RandomFloat();
    }

}



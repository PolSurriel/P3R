using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class AIController: MonoBehaviour
{
    bool onStain;

    const int STAIN_TIMES_TO_JUMP = 2;
    int stainJumpsCounter;

    float stainTimeCounter;
    float stainTimeToNextJump;
    const float MAX_TIME_TO_NEXT_JUMP = 0.2f;
    const float MIN_TIME_TO_NEXT_JUMP = 0.7f;

    public Vector2 stainExitDireciton;

    void StainUpdate()
    {
        stainTimeCounter += Time.deltaTime;

        if (stainTimeCounter >= stainTimeToNextJump)
        {
            runner.Jump(stainExitDireciton);
            stainTimeCounter = 0f;
            SetNextTimeToJump();
            stainJumpsCounter++;
            
            if(stainJumpsCounter>= STAIN_TIMES_TO_JUMP)
            {
                onStain = false;
            }
        }

    }

    void SetNextTimeToJump()
    {
        stainTimeToNextJump = Random.Range(MIN_TIME_TO_NEXT_JUMP, MAX_TIME_TO_NEXT_JUMP);

    }

    public void EnterOnStain()
    {
        stainJumpsCounter = 0;
        stainTimeCounter = 0f;
        onStain = true;
        SetNextTimeToJump();

    }
}

using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    public SpriteRenderer baseSR;
    public SpriteRenderer suitSR;
    public SpriteRenderer accessory1SR;
    public SpriteRenderer accessory2SR;

    [HideInInspector]
    public string baseSkin;
    [HideInInspector]
    public string suitSkin;
    [HideInInspector]
    public string accessory1Skin;
    [HideInInspector]
    public string accessory2Skin;

    const string ROOT_PATH = "Animation/";
    const string BASE_PATH = ROOT_PATH + "Base/";
    const string SUIT_PATH = ROOT_PATH + "SUIT/";
    const string AC1_PATH = ROOT_PATH + "A1/";
    const string AC2_PATH = ROOT_PATH + "A2/";

    struct SkinAnimation
    {
        public SpriteAnimationManager.SpriteAnimation floor1;
        public SpriteAnimationManager.SpriteAnimation floor2;
        public SpriteAnimationManager.SpriteAnimation floor3;
        public SpriteAnimationManager.SpriteAnimation floor4;
        public SpriteAnimationManager.SpriteAnimation floor5;
        public SpriteAnimationManager.SpriteAnimation floor1_eye_moved;
        public SpriteAnimationManager.SpriteAnimation floor2_eye_moved;
        public SpriteAnimationManager.SpriteAnimation floor3_eye_moved;
        public SpriteAnimationManager.SpriteAnimation floor4_eye_moved;
        public SpriteAnimationManager.SpriteAnimation floor5_eye_moved;
        public SpriteAnimationManager.SpriteAnimation jump1;
        public SpriteAnimationManager.SpriteAnimation jump2;
        public SpriteAnimationManager.SpriteAnimation jump3;
        public SpriteAnimationManager.SpriteAnimation wall;
        public SpriteAnimationManager.SpriteAnimation wall_edge;
        public SpriteAnimationManager.SpriteAnimation wall_edge_eye_moved;
        public SpriteAnimationManager.SpriteAnimation wall_eye_moved;
        public SpriteAnimationManager.SpriteAnimation wall_transition;

        
    }

    SkinAnimation baseAnimations;
    SkinAnimation suitAnimations;
    SkinAnimation accessory1Animations;
    SkinAnimation accessory2Animations;

    Coroutine previousBaseAnimationPlaying = null;
    Coroutine previousSuiteAnimationPlaying = null;
    Coroutine previousAccessory1AnimationPlaying = null;
    Coroutine previousAccessory2AnimationPlaying = null;

    
    void StopPrevious()
    {
        pendingToExecuteWall = false;
        try
        {
            StopCoroutine(previousBaseAnimationPlaying);
            StopCoroutine(previousSuiteAnimationPlaying);
            StopCoroutine(previousAccessory1AnimationPlaying);
            StopCoroutine(previousAccessory2AnimationPlaying);

        }catch(NullReferenceException e) { }
        

        previousBaseAnimationPlaying = null;
        previousSuiteAnimationPlaying = null;
        previousAccessory1AnimationPlaying = null;
        previousAccessory2AnimationPlaying = null;
    }

    bool pendingToExecuteWall = false;


    const float defaultScale = 0.6f;

    public void Play_floor1              () {StopPrevious(); baseSR.transform.localScale = new Vector3(defaultScale, defaultScale, 1f); previousBaseAnimationPlaying = baseAnimations.floor1.RestartPlay(this); previousSuiteAnimationPlaying = suitAnimations.floor1.RestartPlay(this); previousAccessory1AnimationPlaying = accessory1Animations.floor1.RestartPlay(this); previousAccessory2AnimationPlaying = accessory2Animations.floor1.RestartPlay(this); }
    public void Play_floor2              () {StopPrevious(); baseSR.transform.localScale = new Vector3(defaultScale, defaultScale, 1f);  previousBaseAnimationPlaying = baseAnimations.floor2.RestartPlay(this); previousSuiteAnimationPlaying = suitAnimations.floor2.RestartPlay(this); previousAccessory1AnimationPlaying = accessory1Animations.floor2.RestartPlay(this); previousAccessory2AnimationPlaying = accessory2Animations.floor2.RestartPlay(this);}
    public void Play_floor3              () {StopPrevious(); baseSR.transform.localScale = new Vector3(defaultScale, defaultScale, 1f); previousBaseAnimationPlaying = baseAnimations.floor3.RestartPlay(this); previousSuiteAnimationPlaying = suitAnimations.floor3.RestartPlay(this); previousAccessory1AnimationPlaying = accessory1Animations.floor3.RestartPlay(this); previousAccessory2AnimationPlaying = accessory2Animations.floor3.RestartPlay(this);}
    public void Play_floor4              () {StopPrevious(); baseSR.transform.localScale = new Vector3(defaultScale, defaultScale, 1f); previousBaseAnimationPlaying = baseAnimations.floor4.RestartPlay(this); previousSuiteAnimationPlaying = suitAnimations.floor4.RestartPlay(this); previousAccessory1AnimationPlaying = accessory1Animations.floor4.RestartPlay(this); previousAccessory2AnimationPlaying = accessory2Animations.floor4.RestartPlay(this);}
    public void Play_floor5              () {StopPrevious(); baseSR.transform.localScale = new Vector3(defaultScale, defaultScale, 1f); previousBaseAnimationPlaying = baseAnimations.floor5.RestartPlay(this); previousSuiteAnimationPlaying = suitAnimations.floor5.RestartPlay(this); previousAccessory1AnimationPlaying = accessory1Animations.floor5.RestartPlay(this); previousAccessory2AnimationPlaying = accessory2Animations.floor5.RestartPlay(this);}
    public void Play_floor1_eye_moved    () {StopPrevious(); baseSR.transform.localScale = new Vector3(defaultScale, defaultScale, 1f); previousBaseAnimationPlaying = baseAnimations.floor1_eye_moved.RestartPlay(this); previousSuiteAnimationPlaying = suitAnimations.floor1_eye_moved.RestartPlay(this); previousAccessory1AnimationPlaying = accessory1Animations.floor1_eye_moved.RestartPlay(this); previousAccessory2AnimationPlaying = accessory2Animations.floor1_eye_moved.RestartPlay(this);}
    public void Play_floor2_eye_moved    () {StopPrevious(); baseSR.transform.localScale = new Vector3(defaultScale, defaultScale, 1f); previousBaseAnimationPlaying = baseAnimations.floor2_eye_moved.RestartPlay(this); previousSuiteAnimationPlaying = suitAnimations.floor2_eye_moved.RestartPlay(this); previousAccessory1AnimationPlaying = accessory1Animations.floor2_eye_moved.RestartPlay(this); previousAccessory2AnimationPlaying = accessory2Animations.floor2_eye_moved.RestartPlay(this);}
    public void Play_floor3_eye_moved    () {StopPrevious(); baseSR.transform.localScale = new Vector3(defaultScale, defaultScale, 1f); previousBaseAnimationPlaying = baseAnimations.floor3_eye_moved.RestartPlay(this); previousSuiteAnimationPlaying = suitAnimations.floor3_eye_moved.RestartPlay(this); previousAccessory1AnimationPlaying = accessory1Animations.floor3_eye_moved.RestartPlay(this); previousAccessory2AnimationPlaying = accessory2Animations.floor3_eye_moved.RestartPlay(this);}
    public void Play_floor4_eye_moved    () {StopPrevious(); baseSR.transform.localScale = new Vector3(defaultScale, defaultScale, 1f); previousBaseAnimationPlaying = baseAnimations.floor4_eye_moved.RestartPlay(this); previousSuiteAnimationPlaying = suitAnimations.floor4_eye_moved.RestartPlay(this); previousAccessory1AnimationPlaying = accessory1Animations.floor4_eye_moved.RestartPlay(this); previousAccessory2AnimationPlaying = accessory2Animations.floor4_eye_moved.RestartPlay(this);}
    public void Play_floor5_eye_moved    () {StopPrevious(); baseSR.transform.localScale = new Vector3(defaultScale, defaultScale, 1f); previousBaseAnimationPlaying = baseAnimations.floor5_eye_moved.RestartPlay(this); previousSuiteAnimationPlaying = suitAnimations.floor5_eye_moved.RestartPlay(this); previousAccessory1AnimationPlaying = accessory1Animations.floor5_eye_moved.RestartPlay(this); previousAccessory2AnimationPlaying = accessory2Animations.floor5_eye_moved.RestartPlay(this);}
    public void Play_jump1               () {StopPrevious(); baseSR.transform.localScale = new Vector3(defaultScale, defaultScale, 1f); previousBaseAnimationPlaying = baseAnimations.jump1.RestartPlay(this); previousSuiteAnimationPlaying = suitAnimations.jump1.RestartPlay(this); previousAccessory1AnimationPlaying = accessory1Animations.jump1.RestartPlay(this); previousAccessory2AnimationPlaying = accessory2Animations.jump1.RestartPlay(this);}
    public void Play_jump2               () {StopPrevious(); baseSR.transform.localScale = new Vector3(defaultScale, defaultScale, 1f); previousBaseAnimationPlaying = baseAnimations.jump2.RestartPlay(this); previousSuiteAnimationPlaying = suitAnimations.jump2.RestartPlay(this); previousAccessory1AnimationPlaying = accessory1Animations.jump2.RestartPlay(this); previousAccessory2AnimationPlaying = accessory2Animations.jump2.RestartPlay(this);}
    public void Play_jump3               () {StopPrevious(); baseSR.transform.localScale = new Vector3(defaultScale, defaultScale, 1f); previousBaseAnimationPlaying = baseAnimations.jump3.RestartPlay(this); previousSuiteAnimationPlaying = suitAnimations.jump3.RestartPlay(this); previousAccessory1AnimationPlaying = accessory1Animations.jump3.RestartPlay(this); previousAccessory2AnimationPlaying = accessory2Animations.jump3.RestartPlay(this);}
    public void Play_wall_edge           () {StopPrevious(); baseSR.transform.localScale = new Vector3(defaultScale, defaultScale, 1f); previousBaseAnimationPlaying = baseAnimations.wall_edge.RestartPlay(this); previousSuiteAnimationPlaying = suitAnimations.wall_edge.RestartPlay(this); previousAccessory1AnimationPlaying = accessory1Animations.wall_edge.RestartPlay(this); previousAccessory2AnimationPlaying = accessory2Animations.wall_edge.RestartPlay(this);}
    public void Play_wall_edge_eye_moved () {StopPrevious(); baseSR.transform.localScale = new Vector3(defaultScale, defaultScale, 1f); previousBaseAnimationPlaying = baseAnimations.wall_edge_eye_moved.RestartPlay(this); previousSuiteAnimationPlaying = suitAnimations.wall_edge_eye_moved.RestartPlay(this); previousAccessory1AnimationPlaying = accessory1Animations.wall_edge_eye_moved.RestartPlay(this); previousAccessory2AnimationPlaying = accessory2Animations.wall_edge_eye_moved.RestartPlay(this);}
    public void Play_wall_eye_moved      () {StopPrevious(); baseSR.transform.localScale = new Vector3(defaultScale, defaultScale, 1f); previousBaseAnimationPlaying = baseAnimations.wall_eye_moved.RestartPlay(this); previousSuiteAnimationPlaying = suitAnimations.wall_eye_moved.RestartPlay(this); previousAccessory1AnimationPlaying = accessory1Animations.wall_eye_moved.RestartPlay(this); previousAccessory2AnimationPlaying = accessory2Animations.wall_eye_moved.RestartPlay(this);}
    public void Play_wall () {
        StopPrevious();
        pendingToExecuteWall = true;
        previousBaseAnimationPlaying = baseAnimations.wall_transition.PlayOnce(this, ()=> {
            if (pendingToExecuteWall)
            {
                previousBaseAnimationPlaying = baseAnimations.wall.RestartPlay(this); 
            }
        });
        previousSuiteAnimationPlaying = suitAnimations.wall_transition.PlayOnce(this, () => {
            if (pendingToExecuteWall)
            {
                previousSuiteAnimationPlaying = suitAnimations.wall.RestartPlay(this); 
            }
        });
        previousAccessory1AnimationPlaying = accessory1Animations.wall_transition.PlayOnce(this, () => {
            if (pendingToExecuteWall)
            {
                previousAccessory1AnimationPlaying = accessory1Animations.wall.RestartPlay(this); 
            }
        });
        previousAccessory2AnimationPlaying = accessory2Animations.wall_transition.PlayOnce(this, () => {
            if (pendingToExecuteWall)
            {
                previousAccessory2AnimationPlaying = accessory2Animations.wall.RestartPlay(this);
            }
        });

    }

    public void SetJump1Speed(float speed)
    {
        const float realSpeed = 4f;

        baseAnimations.jump1.speed = speed * realSpeed;
        suitAnimations.jump1.speed = speed * realSpeed;
        accessory1Animations.jump1.speed = speed * realSpeed;
        accessory2Animations.jump1.speed = speed * realSpeed;
    }


    void LoadSlot(ref SkinAnimation slot, SpriteRenderer target, string path)
    {
        slot.floor1 = SpriteAnimationManager.Load(target, path + "ch3rig-floor1");
        slot.floor2 = SpriteAnimationManager.Load(target, path + "ch3rig-floor2");
        slot.floor3 = SpriteAnimationManager.Load(target, path + "ch3rig-floor3");
        slot.floor4 = SpriteAnimationManager.Load(target, path + "ch3rig-floor4");
        slot.floor5 = SpriteAnimationManager.Load(target, path + "ch3rig-floor5");
        slot.floor1_eye_moved = SpriteAnimationManager.Load(target, path + "ch3rig-floor1-eye-moved");
        slot.floor2_eye_moved = SpriteAnimationManager.Load(target, path + "ch3rig-floor2-eye-moved");
        slot.floor3_eye_moved = SpriteAnimationManager.Load(target, path + "ch3rig-floor3-eye-moved");
        slot.floor4_eye_moved = SpriteAnimationManager.Load(target, path + "ch3rig-floor4-eye-moved");
        slot.floor5_eye_moved = SpriteAnimationManager.Load(target, path + "ch3rig-floor5-eye-moved");
        slot.jump1 = SpriteAnimationManager.Load(target, path + "ch3rig-jump1");
        slot.jump2 = SpriteAnimationManager.Load(target, path + "ch3rig-jump2");
        slot.jump3 = SpriteAnimationManager.Load(target, path + "ch3rig-jump3");
        slot.wall = SpriteAnimationManager.Load(target, path + "ch3rig-wall");
        slot.wall_edge = SpriteAnimationManager.Load(target, path + "ch3rig-wall-edge");
        slot.wall_edge_eye_moved = SpriteAnimationManager.Load(target, path + "ch3rig-wall-edge-eye-moved");
        slot.wall_eye_moved = SpriteAnimationManager.Load(target, path + "ch3rig-wall-eye-moved");
        slot.wall_transition = SpriteAnimationManager.Load(target, path + "ch3rig-wall-transition");


        slot.floor1.speed = 1f;
        slot.floor2.speed = 1f;
        slot.floor3.speed = 1f;
        slot.floor4.speed = 1f;
        slot.floor5.speed = 1f;
        slot.floor1_eye_moved.speed = 1f;
        slot.floor2_eye_moved.speed = 1f;
        slot.floor3_eye_moved.speed = 1f;
        slot.floor4_eye_moved.speed = 1f;
        slot.floor5_eye_moved.speed = 1f;
        slot.jump1.speed = 4f;
        slot.jump2.speed = 1f;
        slot.jump3.speed = 1f;
        slot.wall.speed = 1f;
        slot.wall_edge.speed = 1f;
        slot.wall_edge_eye_moved.speed = 1f;
        slot.wall_eye_moved.speed = 1f;
        slot.wall_transition.speed = 2f;
        slot.wall_transition.speed = 2f;


}


    public void LoadAnimations()
    {
        LoadSlot(ref baseAnimations, baseSR, BASE_PATH + baseSkin + "/");
        LoadSlot(ref suitAnimations, suitSR, SUIT_PATH + suitSkin + "/");
        LoadSlot(ref accessory1Animations, accessory1SR, AC1_PATH + accessory1Skin + "/");
        LoadSlot(ref accessory2Animations, accessory2SR, AC2_PATH + accessory2Skin + "/");
    }

}

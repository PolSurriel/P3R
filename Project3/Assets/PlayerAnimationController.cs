using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    public Transform aspectContainer;
    public SpriteRenderer baseSR;
    public SpriteRenderer suitSR;
    public SpriteRenderer accessory1SR;
    public SpriteRenderer accessory2SR;

    [HideInInspector]
    public string baseSkin = "Yellow";
    [HideInInspector]
    public string suitSkin = "Default";
    [HideInInspector]
    public string accessory1Skin = "Default";
    [HideInInspector]
    public string accessory2Skin = "Default";

    const string ROOT_PATH = "Animation/";
    const string BASE_PATH = ROOT_PATH + "Base/";
    const string SUIT_PATH = ROOT_PATH + "SUIT/";
    const string AC1_PATH = ROOT_PATH + "A1/";
    const string AC2_PATH = ROOT_PATH + "A2/";

    class SkinAnimation
    {
        public SpriteAnimationManager.SpriteAnimation floor1_transition;
        public SpriteAnimationManager.SpriteAnimation floor2_transition;
        public SpriteAnimationManager.SpriteAnimation floor3_transition;
        public SpriteAnimationManager.SpriteAnimation floor4_transition;
        public SpriteAnimationManager.SpriteAnimation floor5_transition;
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
        public SpriteAnimationManager.SpriteAnimation wallup;
        public SpriteAnimationManager.SpriteAnimation wall_edge;
        public SpriteAnimationManager.SpriteAnimation wall_edge_eye_moved;
        public SpriteAnimationManager.SpriteAnimation wall_eye_moved;
        public SpriteAnimationManager.SpriteAnimation wall_transition;

        
    }

    SkinAnimation baseAnimations = new SkinAnimation();
    SkinAnimation suitAnimations = new SkinAnimation();
    SkinAnimation accessory1Animations = new SkinAnimation();
    SkinAnimation accessory2Animations = new SkinAnimation();

    Coroutine previousBaseAnimationPlaying = null;

    
    void StopPrevious()
    {
        aspectContainer.transform.localPosition = Vector3.zero; 

        nextAnimationCallbackID = GetCallbackID();
        nextEyeMovedAnimationCallbackID = GetEyeMovedCallbackID();
        try
        {
            StopCoroutine(previousBaseAnimationPlaying);

        }catch(NullReferenceException e) { }
        

        previousBaseAnimationPlaying = null;
    }



    const float defaultScale = 0.6f;
    const float floor1Scale = 0.50f;
    const float floor2Scale = 0.62f;
    const float floor3Scale = 0.57f;
    const float floor4Scale = 0.70f;
    const float floor5Scale = 0.58f;


    const float floor1TScale = 0.57f;
    const float floor2TScale = 0.69f;
    const float floor3TScale = 0.57f;
    const float floor4TScale = 0.88f;
    const float floor5TScale = 0.59f;
    const float floor1_eye_movedScaleScale = floor1Scale;
    const float floor2_eye_movedScaleScale = floor2Scale;
    const float floor3_eye_movedScaleScale = floor3Scale;
    const float floor4_eye_movedScaleScale = floor4Scale;
    const float floor5_eye_movedScaleScale = floor5Scale;
    const float jump1Scale = defaultScale;
    const float jump2Scale = 0.53f;
    const float jump3Scale = 0.515f;
    const float wallScale = 0.56f;
    const float wallTScale = 0.56f;
    const float wall_edgeScale = wallScale;
    const float wall_edge_eye_movedScale = wallScale;
    const float wall_eye_movedScale = wallScale;
    const float wall_upScale = defaultScale;


    Vector3 floor4Offset {
        get
        {
            return (baseSR.flipX? Vector3.right : Vector3.left) * 0.1f;
        }
    }

    // THIS CODE IS AUTO GENERATED, DO NOT TRY TO UNDERSTAND IT. READ SCRIPTER CODE INSTED.
    void WallCallBack() { try { StopCoroutine(previousBaseAnimationPlaying); } catch (NullReferenceException e) { } if (nextEyeMovedAnimationCallbackID != eyeMoved_callBackID) return; var anim = baseAnimations.wall; if (!lastAnimationWasEyeMoved && UnityEngine.Random.Range(0f, 10f) < eachInOnehundredpEyeMovedProbability) { lastAnimationWasEyeMoved = true; anim = baseAnimations.wall_eye_moved; } else { lastAnimationWasEyeMoved = false; } anim.PlayOnce(this, WallCallBack); }
    void Floor1CallBack() { try { StopCoroutine(previousBaseAnimationPlaying); } catch (NullReferenceException e) { } if (nextEyeMovedAnimationCallbackID != eyeMoved_callBackID) return; var anim = baseAnimations.floor1; if (!lastAnimationWasEyeMoved && UnityEngine.Random.Range(0f, 10f) < eachInOnehundredpEyeMovedProbability) { lastAnimationWasEyeMoved = true; anim = baseAnimations.floor1_eye_moved; } else { lastAnimationWasEyeMoved = false; } anim.PlayOnce(this, Floor1CallBack); }
    void Floor2CallBack() { try { StopCoroutine(previousBaseAnimationPlaying); } catch (NullReferenceException e) { } if (nextEyeMovedAnimationCallbackID != eyeMoved_callBackID) return; var anim = baseAnimations.floor2; if (!lastAnimationWasEyeMoved && UnityEngine.Random.Range(0f, 10f) < eachInOnehundredpEyeMovedProbability) { lastAnimationWasEyeMoved = true; anim = baseAnimations.floor2_eye_moved; } else { lastAnimationWasEyeMoved = false; } anim.PlayOnce(this, Floor2CallBack); }
    void Floor3CallBack() { try { StopCoroutine(previousBaseAnimationPlaying); } catch (NullReferenceException e) { } if (nextEyeMovedAnimationCallbackID != eyeMoved_callBackID) return; var anim = baseAnimations.floor3; if (!lastAnimationWasEyeMoved && UnityEngine.Random.Range(0f, 10f) < eachInOnehundredpEyeMovedProbability) { lastAnimationWasEyeMoved = true; anim = baseAnimations.floor3_eye_moved; } else { lastAnimationWasEyeMoved = false; } anim.PlayOnce(this, Floor3CallBack); }
    void Floor4CallBack() { try { StopCoroutine(previousBaseAnimationPlaying); } catch (NullReferenceException e) { } if (nextEyeMovedAnimationCallbackID != eyeMoved_callBackID) return; var anim = baseAnimations.floor4; if (!lastAnimationWasEyeMoved && UnityEngine.Random.Range(0f, 10f) < eachInOnehundredpEyeMovedProbability) { lastAnimationWasEyeMoved = true; anim = baseAnimations.floor4_eye_moved; } else { lastAnimationWasEyeMoved = false; } anim.PlayOnce(this, Floor4CallBack); }
    void Floor5CallBack() { try { StopCoroutine(previousBaseAnimationPlaying); } catch (NullReferenceException e) { } if (nextEyeMovedAnimationCallbackID != eyeMoved_callBackID) return; var anim = baseAnimations.floor5; if (!lastAnimationWasEyeMoved && UnityEngine.Random.Range(0f, 10f) < eachInOnehundredpEyeMovedProbability) { lastAnimationWasEyeMoved = true; anim = baseAnimations.floor5_eye_moved; } else { lastAnimationWasEyeMoved = false; } anim.PlayOnce(this, Floor5CallBack); }
    
    public void Play_wall                () {StopPrevious(); int callbackID = GetCallbackID(); SetScale(new Vector3(wallTScale, wallTScale, 1f));      previousBaseAnimationPlaying = baseAnimations.wall_transition.PlayOnce(this, () => { if (nextAnimationCallbackID == callbackID) { eyeMoved_callBackID = GetEyeMovedCallbackID();   SetScale(new Vector3(wallScale, wallScale, 1f)    ); previousBaseAnimationPlaying = baseAnimations.wall.PlayOnce(this, WallCallBack); } }); }//uitAnimations.wall_transition.PlayOnce(this, () => { if (nextAnimationCallbackID == callbackID) { suitSR.transform.localScale = new Vector3(wallScale, wallScale, 1f); }//uitAnimations.wall.RestartPlay(this); } }); previousAccessory1AnimationPlaying = accessory1Animations.wall_transition.PlayOnce(this, () => { if (nextAnimationCallbackID == callbackID) { accessory1SR.transform.localScale = new Vector3(wallScale, wallScale, 1f); previousAccessory1AnimationPlaying = accessory1Animations.wall.RestartPlay(this); } }); previousAccessory2AnimationPlaying = accessory2Animations.wall_transition.PlayOnce(this, () => { if (nextAnimationCallbackID == callbackID) { accessory2SR.transform.localScale = new Vector3(wallScale, wallScale, 1f); previousAccessory2AnimationPlaying = accessory2Animations.wall.RestartPlay(this); } }); }
    public void Play_floor1              () {StopPrevious(); int callbackID = GetCallbackID(); SetScale(new Vector3(floor1TScale, floor1TScale, 1f));  previousBaseAnimationPlaying = baseAnimations.floor1_transition.PlayOnce(this, () => { if (nextAnimationCallbackID == callbackID) { eyeMoved_callBackID = GetEyeMovedCallbackID(); SetScale(new Vector3(floor1Scale, floor1Scale, 1f)); previousBaseAnimationPlaying = baseAnimations.floor1.PlayOnce(this, Floor1CallBack); } }); }//uitAnimations.wall_transition.PlayOnce(this, () => { if (nextAnimationCallbackID == callbackID) { suitSR.transform.localScale = new Vector3(floor1Scale, floor1Scale, 1f); }//uitAnimations.floor1.RestartPlay(this); } }); previousAccessory1AnimationPlaying = accessory1Animations.floor1_transition.PlayOnce(this, () => { if (nextAnimationCallbackID == callbackID) { accessory1SR.transform.localScale = new Vector3(floor1Scale, floor1Scale, 1f); previousAccessory1AnimationPlaying = accessory1Animations.floor1.RestartPlay(this); } }); previousAccessory2AnimationPlaying = accessory2Animations.floor1_transition.PlayOnce(this, () => { if (nextAnimationCallbackID == callbackID) { accessory2SR.transform.localScale = new Vector3(floor1Scale, floor1Scale, 1f); previousAccessory2AnimationPlaying = accessory2Animations.floor1.RestartPlay(this); } }); }
    public void Play_floor2              () {StopPrevious(); int callbackID = GetCallbackID(); SetScale(new Vector3(floor2TScale, floor2TScale, 1f));  previousBaseAnimationPlaying = baseAnimations.floor2_transition.PlayOnce(this, () => { if (nextAnimationCallbackID == callbackID) { eyeMoved_callBackID = GetEyeMovedCallbackID(); SetScale(new Vector3(floor2Scale, floor2Scale, 1f)); previousBaseAnimationPlaying = baseAnimations.floor2.PlayOnce(this, Floor2CallBack); } }); }//uitAnimations.wall_transition.PlayOnce(this, () => { if (nextAnimationCallbackID == callbackID) { suitSR.transform.localScale = new Vector3(floor2Scale, floor2Scale, 1f); }//uitAnimations.floor2.RestartPlay(this); } }); previousAccessory1AnimationPlaying = accessory1Animations.floor2_transition.PlayOnce(this, () => { if (nextAnimationCallbackID == callbackID) { accessory1SR.transform.localScale = new Vector3(floor2Scale, floor2Scale, 1f); previousAccessory1AnimationPlaying = accessory1Animations.floor2.RestartPlay(this); } }); previousAccessory2AnimationPlaying = accessory2Animations.floor2_transition.PlayOnce(this, () => { if (nextAnimationCallbackID == callbackID) { accessory2SR.transform.localScale = new Vector3(floor2Scale, floor2Scale, 1f); previousAccessory2AnimationPlaying = accessory2Animations.floor2.RestartPlay(this); } }); }
    public void Play_floor3              () {StopPrevious(); int callbackID = GetCallbackID(); SetScale(new Vector3(floor3TScale, floor3TScale, 1f));  previousBaseAnimationPlaying = baseAnimations.floor3_transition.PlayOnce(this, () => { if (nextAnimationCallbackID == callbackID) { eyeMoved_callBackID = GetEyeMovedCallbackID(); SetScale(new Vector3(floor3Scale, floor3Scale, 1f)); previousBaseAnimationPlaying = baseAnimations.floor3.PlayOnce(this, Floor3CallBack); } }); }//uitAnimations.wall_transition.PlayOnce(this, () => { if (nextAnimationCallbackID == callbackID) { suitSR.transform.localScale = new Vector3(floor3Scale, floor3Scale, 1f); }//uitAnimations.floor3.RestartPlay(this); } }); previousAccessory1AnimationPlaying = accessory1Animations.floor3_transition.PlayOnce(this, () => { if (nextAnimationCallbackID == callbackID) { accessory1SR.transform.localScale = new Vector3(floor3Scale, floor3Scale, 1f); previousAccessory1AnimationPlaying = accessory1Animations.floor3.RestartPlay(this); } }); previousAccessory2AnimationPlaying = accessory2Animations.floor3_transition.PlayOnce(this, () => { if (nextAnimationCallbackID == callbackID) { accessory2SR.transform.localScale = new Vector3(floor3Scale, floor3Scale, 1f); previousAccessory2AnimationPlaying = accessory2Animations.floor3.RestartPlay(this); } }); }
    public void Play_floor4              () {StopPrevious(); aspectContainer.transform.localPosition = floor4Offset; int callbackID = GetCallbackID(); SetScale(new Vector3(floor4TScale, floor4TScale, 1f)); previousBaseAnimationPlaying = baseAnimations.floor4_transition.PlayOnce(this, () => { if (nextAnimationCallbackID == callbackID) { aspectContainer.transform.localPosition = Vector3.zero; eyeMoved_callBackID = GetEyeMovedCallbackID(); SetScale(new Vector3(floor4Scale, floor4Scale, 1f)); previousBaseAnimationPlaying = baseAnimations.floor4.PlayOnce(this, Floor4CallBack); } }); }//uitAnimations.wall_transition.PlayOnce(this, () => { if (nextAnimationCallbackID == callbackID) { suitSR.transform.localScale = new Vector3(floor4Scale, floor4Scale, 1f); }//uitAnimations.floor4.RestartPlay(this); } }); previousAccessory1AnimationPlaying = accessory1Animations.floor4_transition.PlayOnce(this, () => { if (nextAnimationCallbackID == callbackID) { accessory1SR.transform.localScale = new Vector3(floor4Scale, floor4Scale, 1f); previousAccessory1AnimationPlaying = accessory1Animations.floor4.RestartPlay(this); } }); previousAccessory2AnimationPlaying = accessory2Animations.floor4_transition.PlayOnce(this, () => { if (nextAnimationCallbackID == callbackID) { accessory2SR.transform.localScale = new Vector3(floor4Scale, floor4Scale, 1f); previousAccessory2AnimationPlaying = accessory2Animations.floor4.RestartPlay(this); } }); }
    public void Play_floor5              () {StopPrevious(); int callbackID = GetCallbackID(); SetScale( new Vector3(floor5TScale, floor5TScale, 1f));  previousBaseAnimationPlaying = baseAnimations.floor5_transition.PlayOnce(this, () => { if (nextAnimationCallbackID == callbackID) { eyeMoved_callBackID = GetEyeMovedCallbackID(); SetScale(new Vector3(floor5Scale, floor5Scale, 1f)); previousBaseAnimationPlaying = baseAnimations.floor5.PlayOnce(this, Floor5CallBack); } }); }//uitAnimations.wall_transition.PlayOnce(this, () => { if (nextAnimationCallbackID == callbackID) { suitSR.transform.localScale = new Vector3(floor5Scale, floor5Scale, 1f); }//uitAnimations.floor5.RestartPlay(this); } }); previousAccessory1AnimationPlaying = accessory1Animations.floor5_transition.PlayOnce(this, () => { if (nextAnimationCallbackID == callbackID) { accessory1SR.transform.localScale = new Vector3(floor5Scale, floor5Scale, 1f); previousAccessory1AnimationPlaying = accessory1Animations.floor5.RestartPlay(this); } }); previousAccessory2AnimationPlaying = accessory2Animations.floor5_transition.PlayOnce(this, () => { if (nextAnimationCallbackID == callbackID) { accessory2SR.transform.localScale = new Vector3(floor5Scale, floor5Scale, 1f); previousAccessory2AnimationPlaying = accessory2Animations.floor5.RestartPlay(this); } }); }
    public void Play_wallup              () {StopPrevious(); SetScale(new Vector3(wall_upScale, wall_upScale, 1f));                              previousBaseAnimationPlaying = baseAnimations.wallup.RestartPlay(this); }//uitAnimations.wallup.RestartPlay(this); previousAccessory1AnimationPlaying = accessory1Animations.wallup.RestartPlay(this); previousAccessory2AnimationPlaying = accessory2Animations.wallup.RestartPlay(this); }
    public void Play_floor1_eye_moved    () {StopPrevious(); SetScale(new Vector3(floor1_eye_movedScaleScale, floor1_eye_movedScaleScale, 1f));  previousBaseAnimationPlaying = baseAnimations.floor1_eye_moved.RestartPlay(this); }//uitAnimations.floor1_eye_moved.RestartPlay(this); previousAccessory1AnimationPlaying = accessory1Animations.floor1_eye_moved.RestartPlay(this); previousAccessory2AnimationPlaying = accessory2Animations.floor1_eye_moved.RestartPlay(this);}
    public void Play_floor2_eye_moved    () {StopPrevious(); SetScale(new Vector3(floor2_eye_movedScaleScale, floor2_eye_movedScaleScale, 1f));  previousBaseAnimationPlaying = baseAnimations.floor2_eye_moved.RestartPlay(this); }//uitAnimations.floor2_eye_moved.RestartPlay(this); previousAccessory1AnimationPlaying = accessory1Animations.floor2_eye_moved.RestartPlay(this); previousAccessory2AnimationPlaying = accessory2Animations.floor2_eye_moved.RestartPlay(this);}
    public void Play_floor3_eye_moved    () {StopPrevious(); SetScale(new Vector3(floor3_eye_movedScaleScale, floor3_eye_movedScaleScale, 1f));  previousBaseAnimationPlaying = baseAnimations.floor3_eye_moved.RestartPlay(this); }//uitAnimations.floor3_eye_moved.RestartPlay(this); previousAccessory1AnimationPlaying = accessory1Animations.floor3_eye_moved.RestartPlay(this); previousAccessory2AnimationPlaying = accessory2Animations.floor3_eye_moved.RestartPlay(this);}
    public void Play_floor4_eye_moved    () {StopPrevious(); SetScale(new Vector3(floor4_eye_movedScaleScale, floor4_eye_movedScaleScale, 1f));  previousBaseAnimationPlaying = baseAnimations.floor4_eye_moved.RestartPlay(this); }//uitAnimations.floor4_eye_moved.RestartPlay(this); previousAccessory1AnimationPlaying = accessory1Animations.floor4_eye_moved.RestartPlay(this); previousAccessory2AnimationPlaying = accessory2Animations.floor4_eye_moved.RestartPlay(this);}
    public void Play_floor5_eye_moved    () {StopPrevious(); SetScale(new Vector3(floor5_eye_movedScaleScale, floor5_eye_movedScaleScale, 1f));  previousBaseAnimationPlaying = baseAnimations.floor5_eye_moved.RestartPlay(this); }//uitAnimations.floor5_eye_moved.RestartPlay(this); previousAccessory1AnimationPlaying = accessory1Animations.floor5_eye_moved.RestartPlay(this); previousAccessory2AnimationPlaying = accessory2Animations.floor5_eye_moved.RestartPlay(this);}
    public void Play_jump1               () {StopPrevious(); SetScale(new Vector3(jump1Scale, jump1Scale, 1f));                                  previousBaseAnimationPlaying = baseAnimations.jump1.RestartPlay(this); }//uitAnimations.jump1.RestartPlay(this); previousAccessory1AnimationPlaying = accessory1Animations.jump1.RestartPlay(this); previousAccessory2AnimationPlaying = accessory2Animations.jump1.RestartPlay(this);}
    public void Play_jump2               () {StopPrevious(); SetScale(new Vector3(jump2Scale, jump2Scale, 1f));                                  previousBaseAnimationPlaying = baseAnimations.jump2.RestartPlay(this); }//uitAnimations.jump2.RestartPlay(this); previousAccessory1AnimationPlaying = accessory1Animations.jump2.RestartPlay(this); previousAccessory2AnimationPlaying = accessory2Animations.jump2.RestartPlay(this);}
    public void Play_jump3               () {StopPrevious(); SetScale(new Vector3(jump3Scale, jump3Scale, 1f));                                  previousBaseAnimationPlaying = baseAnimations.jump3.RestartPlay(this); }//uitAnimations.jump3.RestartPlay(this); previousAccessory1AnimationPlaying = accessory1Animations.jump3.RestartPlay(this); previousAccessory2AnimationPlaying = accessory2Animations.jump3.RestartPlay(this);}
    public void Play_wall_edge           () {StopPrevious(); SetScale(new Vector3(wall_edgeScale, wall_edgeScale, 1f));                          previousBaseAnimationPlaying = baseAnimations.wall_edge.RestartPlay(this); }//uitAnimations.wall_edge.RestartPlay(this); previousAccessory1AnimationPlaying = accessory1Animations.wall_edge.RestartPlay(this); previousAccessory2AnimationPlaying = accessory2Animations.wall_edge.RestartPlay(this);}
    public void Play_wall_edge_eye_moved () {StopPrevious(); SetScale(new Vector3(wall_edge_eye_movedScale, wall_edge_eye_movedScale, 1f));      previousBaseAnimationPlaying = baseAnimations.wall_edge_eye_moved.RestartPlay(this); }//uitAnimations.wall_edge_eye_moved.RestartPlay(this); previousAccessory1AnimationPlaying = accessory1Animations.wall_edge_eye_moved.RestartPlay(this); previousAccessory2AnimationPlaying = accessory2Animations.wall_edge_eye_moved.RestartPlay(this);}
    public void Play_wall_eye_moved      () {StopPrevious(); SetScale(new Vector3(wall_eye_movedScale, wall_eye_movedScale, 1f));                previousBaseAnimationPlaying = baseAnimations.wall_eye_moved.RestartPlay(this); }//uitAnimations.wall_eye_moved.RestartPlay(this); previousAccessory1AnimationPlaying = accessory1Animations.wall_eye_moved.RestartPlay(this); previousAccessory2AnimationPlaying = accessory2Animations.wall_eye_moved.RestartPlay(this);}
    

    void SetScale(Vector3 scale)
    {
        suitSR.transform.localScale = scale;
        baseSR.transform.localScale = scale;
        accessory1SR.transform.localScale = scale;
        accessory2SR.transform.localScale = scale;
    }

    int nextAnimationCallbackID = 0;
    int nextEyeMovedAnimationCallbackID = 0;
    int GetCallbackID()
    {
        nextAnimationCallbackID = (nextAnimationCallbackID + 1) % int.MaxValue;
        return nextAnimationCallbackID;
    }
    int GetEyeMovedCallbackID()
    {
        nextEyeMovedAnimationCallbackID = (nextEyeMovedAnimationCallbackID + 1) % int.MaxValue;
        return nextEyeMovedAnimationCallbackID;
    }

    bool lastAnimationWasEyeMoved = false;
    int eyeMoved_callBackID;

    float eachInOnehundredpEyeMovedProbability = 50f;

 
    

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

        slot.floor1_transition = SpriteAnimationManager.Load(target, path + "ch3rig-floor1-transition");
        slot.floor2_transition = SpriteAnimationManager.Load(target, path + "ch3rig-floor2-transition");
        slot.floor3_transition = SpriteAnimationManager.Load(target, path + "ch3rig-floor3-transition");
        slot.floor4_transition = SpriteAnimationManager.Load(target, path + "ch3rig-floor4-transition");
        slot.floor5_transition = SpriteAnimationManager.Load(target, path + "ch3rig-floor5-transition");
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
        slot.wallup = SpriteAnimationManager.Load(target, path + "ch3rig-wallup");
        slot.wall_edge = SpriteAnimationManager.Load(target, path + "ch3rig-wall-edge");
        slot.wall_edge_eye_moved = SpriteAnimationManager.Load(target, path + "ch3rig-wall-edge-eye-moved");
        slot.wall_eye_moved = SpriteAnimationManager.Load(target, path + "ch3rig-wall-eye-moved");
        slot.wall_transition = SpriteAnimationManager.Load(target, path + "ch3rig-wall-transition");


        slot.floor1_transition.speed = 1f;
        slot.floor2_transition.speed = 1f;
        slot.floor3_transition.speed = 1f;
        slot.floor4_transition.speed = 1f;
        slot.floor5_transition.speed = 1f;
        slot.wallup.speed = 1f;
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
        slot.wall_transition.speed = 1.6f;


}

    void SetSlotIndexReferenceOf(ref SkinAnimation slot,ref SkinAnimation reference)
    {
        slot.floor1_transition.useOtherAnimationIndex = true;
        slot.floor2_transition.useOtherAnimationIndex = true;
        slot.floor3_transition.useOtherAnimationIndex = true;
        slot.floor4_transition.useOtherAnimationIndex = true;
        slot.floor5_transition.useOtherAnimationIndex = true;
        slot.wallup.useOtherAnimationIndex = true;
        slot.floor1.useOtherAnimationIndex = true;
        slot.floor2.useOtherAnimationIndex = true;
        slot.floor3.useOtherAnimationIndex = true;
        slot.floor4.useOtherAnimationIndex = true;
        slot.floor5.useOtherAnimationIndex = true;
        slot.floor1_eye_moved.useOtherAnimationIndex = true;
        slot.floor2_eye_moved.useOtherAnimationIndex = true;
        slot.floor3_eye_moved.useOtherAnimationIndex = true;
        slot.floor4_eye_moved.useOtherAnimationIndex = true;
        slot.floor5_eye_moved.useOtherAnimationIndex = true;
        slot.jump1.useOtherAnimationIndex = true;
        slot.jump2.useOtherAnimationIndex = true;
        slot.jump3.useOtherAnimationIndex = true;
        slot.wall.useOtherAnimationIndex = true;
        slot.wall_edge.useOtherAnimationIndex = true;
        slot.wall_edge_eye_moved.useOtherAnimationIndex = true;
        slot.wall_eye_moved.useOtherAnimationIndex = true;
        slot.wall_transition.useOtherAnimationIndex = true;

        reference.floor1_transition.copyState.Add(slot.floor1_transition);
        reference.floor2_transition.copyState.Add(slot.floor2_transition);
        reference.floor3_transition.copyState.Add(slot.floor3_transition);
        reference.floor4_transition.copyState.Add(slot.floor4_transition);
        reference.floor5_transition.copyState.Add(slot.floor5_transition);
        reference.wallup.copyState.Add(slot.wallup);
        reference.floor1.copyState.Add(slot.floor1);
        reference.floor2.copyState.Add(slot.floor2);
        reference.floor3.copyState.Add(slot.floor3);
        reference.floor4.copyState.Add(slot.floor4);
        reference.floor5.copyState.Add(slot.floor5);
        reference.floor1_eye_moved.copyState.Add(slot.floor1_eye_moved);
        reference.floor2_eye_moved.copyState.Add(slot.floor2_eye_moved);
        reference.floor3_eye_moved.copyState.Add(slot.floor3_eye_moved);
        reference.floor4_eye_moved.copyState.Add(slot.floor4_eye_moved);
        reference.floor5_eye_moved.copyState.Add(slot.floor5_eye_moved);
        reference.jump1.copyState.Add(slot.jump1);
        reference.jump2.copyState.Add(slot.jump2);
        reference.jump3.copyState.Add(slot.jump3);
        reference.wall.copyState.Add(slot.wall);
        reference.wall_edge.copyState.Add(slot.wall_edge);
        reference.wall_edge_eye_moved.copyState.Add(slot.wall_edge_eye_moved);
        reference.wall_eye_moved.copyState.Add(slot.wall_eye_moved);
        reference.wall_transition.copyState.Add(slot.wall_transition);



    }


    public void LoadAnimations()
    {
        LoadSlot(ref baseAnimations, baseSR, BASE_PATH + baseSkin + "/");
        LoadSlot(ref suitAnimations, suitSR, SUIT_PATH + suitSkin + "/");
        LoadSlot(ref accessory1Animations, accessory1SR, AC1_PATH + accessory1Skin + "/");
        LoadSlot(ref accessory2Animations, accessory2SR, AC2_PATH + accessory2Skin + "/");

        SetSlotIndexReferenceOf(ref suitAnimations, ref baseAnimations);
        SetSlotIndexReferenceOf(ref accessory1Animations, ref baseAnimations);
        SetSlotIndexReferenceOf(ref accessory2Animations, ref baseAnimations);
    }

}

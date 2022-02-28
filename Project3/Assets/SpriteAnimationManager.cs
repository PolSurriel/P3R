using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class SpriteAnimationManager
{

    
    public static SpriteAnimation Load(SpriteRenderer spriteRenderer, string path)
    {
        SpriteAnimation result = new SpriteAnimation();
        result.spriteRenderer = spriteRenderer;

        path = path + "_";

        int i = 0;
        while (true)
        {
            
            string indexStrUnformated = ""+i;
            string indexStrTwiSizedFormated = ("0" + i);
            string indexStr = ("00" + i++);
            indexStr = indexStr.Substring(indexStr.Length - 3);

            string src = path + indexStr;

            Sprite sprite = (Resources.Load(src, typeof(Sprite)) as Sprite);
            
            if(sprite == null)
            {
                sprite = (Resources.Load(path + indexStrUnformated, typeof(Sprite)) as Sprite);
                if (sprite == null)
                {
                    sprite = (Resources.Load(path + indexStrTwiSizedFormated, typeof(Sprite)) as Sprite);
                    if (sprite == null)
                        break;
                }
            }

            result.sprites.Add(sprite);

        }

        if(result.sprites.Count == 0)
        {

            if (path.Contains("/Default/"))
            {
                //load empty sprite
                var sprite = (Resources.Load("Animation/SUIT/Default/ch3rig-floor1_000", typeof(Sprite)) as Sprite);
                result.sprites.Add(sprite);
            }
            else
            {
                Debug.LogError("Animation not found for: "+path);

            }

        }


        return result;
    }

    public class SpriteAnimation
    {
        public SpriteRenderer spriteRenderer;
        public float speed = 1f;


        public int currentSpriteIndex;
        public List<Sprite> sprites = new List<Sprite>();

        public Coroutine playRoutine;

        public float framesPerSecond = 60f;

        public Coroutine RestartPlay(MonoBehaviour context)
        {
            currentSpriteIndex = 0;
            return Play(context);
        }

        public Coroutine Play(MonoBehaviour context)
        {
            playRoutine = context.StartCoroutine(PlayRoutine());
            return playRoutine;
        }


        public delegate void OnAnimationComplete();

        public Coroutine PlayOnce(MonoBehaviour context, OnAnimationComplete onCompleteCallback = null)
        {
            currentSpriteIndex = 0;
            playRoutine = context.StartCoroutine(PlayOnceRoutine(onCompleteCallback));
            return playRoutine;
        }

        IEnumerator PlayOnceRoutine(OnAnimationComplete onCompleteCallback)
        {
            float timeCounter = 0f;

            spriteRenderer.sprite = sprites[currentSpriteIndex];

            float changeFrameEach = 1f / framesPerSecond;

            while (true)
            {
                timeCounter += Time.deltaTime * speed;

                if (timeCounter > changeFrameEach)
                {

                    int increment = (int)(timeCounter / changeFrameEach);

                    currentSpriteIndex += increment;

                    if (currentSpriteIndex >= sprites.Count)
                    {
                        if(onCompleteCallback != null)
                        {
                            onCompleteCallback();
                        }

                        break;
                    }

                    spriteRenderer.sprite = sprites[currentSpriteIndex];
                    timeCounter = 0f;

                }

                yield return null;
            }

        }


        IEnumerator PlayRoutine()
        {
            float timeCounter = 0f;

            
            spriteRenderer.sprite = sprites[currentSpriteIndex];

            float changeFrameEach = 1f / framesPerSecond;

            while (true)
            {
                timeCounter += Time.deltaTime * speed;

                if (timeCounter > changeFrameEach)
                {

                    int increment = (int)(timeCounter / changeFrameEach);

                    currentSpriteIndex += increment;
                    currentSpriteIndex = currentSpriteIndex % sprites.Count;
                        
                    spriteRenderer.sprite = sprites[currentSpriteIndex];
                    timeCounter = 0f;
                }

                yield return null;
            }

        }


        public void Stop(MonoBehaviour context)
        {
            context.StopCoroutine(playRoutine);
        }

        

    }


    
}

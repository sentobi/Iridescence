﻿using UnityEngine;
using System.Collections.Generic;

public enum SPRITE_DIRECTION
{
    DIR_DOWN = 0,
    DIR_DOWNLEFT,
    DIR_LEFT,
    DIR_UPLEFT,
    DIR_UP,
    DIR_UPRIGHT,
    DIR_RIGHT,
    DIR_DOWNRIGHT,

    DIR_TOTAL
}

[System.Serializable]
public struct SerialiseSpriteAnimation
{
    public string name;
    public Texture2D texture;   // file name of sprite sheet
    //public Sprite spritesheet;
    public int framesPerStrip;  // number of frames per animation strip
    public bool multiDirectional;   // whether it's 8-directional or not
    public float frameTime; // how long each frame of this sprite lasts
}

public struct SpriteAnimation
{
    public Sprite[] sprites;
    public int framesPerStrip;  // number of frames per animation strip
    public bool multiDirectional;   // whether it's 8-directional or not
    public float frameTime; // how long each frame of this sprite lasts
}

public class SpriteAnimator : MonoBehaviour {

    public SerialiseSpriteAnimation[] initAnimationsList; // FOR INITIALISING ONLY
    public bool playOnce; // whether this animation should be destroyed upon end of animation
    private int animationCount = 0; // which animation it's currently at; used only for playOnce

    private Dictionary<string, SpriteAnimation> animationsList; // list of sprite animations
    private SpriteAnimation currSprAnimation;   // current sprite animation
    private SpriteRenderer sr;  // a handle to this GameObject's SpriteRenderer
    private int currFrame = 0;  // this frame
    private int currDirection = (int)SPRITE_DIRECTION.DIR_DOWN;  // which direction currently facing
    private float timeElapsed = 0f;
    private bool freezeAnimation = false;

    private bool loop;  // whether this current animation will loop or not

    public bool animationComplete = false;

    // repeating or not

    // for idle ONLY
    private bool isIdleAnimation = false;
    private bool isBlinking = false;
    private int direction = 1;

	// Use this for initialization
	void Awake() {
        loop = true;
        currFrame = 0;
        currDirection = (int)SPRITE_DIRECTION.DIR_DOWN;

        Random.seed = 0;

        animationsList = new Dictionary<string, SpriteAnimation>();
        sr = this.GetComponent<SpriteRenderer>();

        for (int i = 0; i < initAnimationsList.Length; ++i) {
            SpriteAnimation temp;
            temp.sprites = Resources.LoadAll<Sprite>("Sprites/" + initAnimationsList[i].texture.name);
            temp.framesPerStrip = initAnimationsList[i].framesPerStrip;
            temp.multiDirectional = initAnimationsList[i].multiDirectional;
            temp.frameTime = initAnimationsList[i].frameTime;

            animationsList.Add(initAnimationsList[i].name, temp);

            if (i == 0)
                currSprAnimation = temp;
        }
        //currSprAnimation = initAnimationsList[0];
        sr.sprite = currSprAnimation.sprites[currFrame];    // frame = 0
    }
	
	// Update for animation
	void Update () {
        if (freezeAnimation)
            return;

        timeElapsed += Time.deltaTime;

        //if (isIdleAnimation)
        //{
        //    if (timeElapsed >= currSprAnimation.frameTime)
        //    {
        //        timeElapsed -= currSprAnimation.frameTime;
        //        if (isBlinking)
        //        {
        //            currFrame += direction;
        //            if (currFrame >= currSprAnimation.framesPerStrip * (1 + currDirection) - 1)
        //            {
        //                direction = -1;
        //            }
        //            else if (currFrame <= currSprAnimation.framesPerStrip * currDirection)
        //            {
        //                direction = 1;
        //                isBlinking = false;
        //            }
        //        }
        //        else
        //        {
        //            ++currFrame;
        //            if (currFrame >= currSprAnimation.framesPerStrip * currDirection + 2)
        //            {
        //                int rand = (int)Random.Range(0f, 2f);
        //                if (rand == 0)
        //                {   // normal idle
        //                    currFrame -= 2;
        //                }
        //                else
        //                {   // blink loop
        //                    isBlinking = true;
        //                }
        //            }
        //        }
        //
        //        sr.sprite = currSprAnimation.sprites[currFrame];
        //    }
        //
        //    return;
        //}   // end of idle animation

        if (timeElapsed >= currSprAnimation.frameTime)
        {
            timeElapsed -= currSprAnimation.frameTime;
            ++currFrame;
            if (currFrame >= currSprAnimation.framesPerStrip * (1 + currDirection))
            {
                if (playOnce)   // for skill sprites / temporary animations (e.g. particles)
                {
                    ++animationCount;
                    if (animationCount >= initAnimationsList.Length)    // end of animation
                    {
                        Destroy(this.gameObject);
                        return;
                    }
                    else
                    {
                        ChangeAnimation(initAnimationsList[animationCount].name, false);
                        return;
                    }
                }
                else    // for player / entities
                {
                    if (loop)
                    {
                        currFrame -= currSprAnimation.framesPerStrip;
                    }
                    else
                    {
                        // for attacking, etc
                        //this.gameObject.SendMessageUpwards("UseSkill");
                        PlayerAttack player = transform.parent.GetComponent<PlayerAttack>();
                        if (player != null) {
                            player.UseSkill();
                            PlayerAction.instance.SetStopAttacking();
                        }

                        // change animation back to idle
                        ChangeAnimation("Idle", true);
                        animationComplete = true;   // only for non-player (i.e. monsters)
                    }
                }
            }

            sr.sprite = currSprAnimation.sprites[currFrame];
        }
    }

    public void ChangeAnimation(string animationName, bool loop)
    {
        currSprAnimation = animationsList[animationName];
        currFrame = currSprAnimation.framesPerStrip * currDirection;    // frame 0 of that strip
        sr.sprite = currSprAnimation.sprites[currFrame];

        isBlinking = false;

        //if (animationName == "Idle")
        //    isIdleAnimation = true;
        //else
        //    isIdleAnimation = false;

        this.loop = loop;
    }

    // Direction
    public int GetCurrDirection()
    {
        return currDirection;
    }
    public void ChangeDirection(int dir)
    {
        if (!currSprAnimation.multiDirectional)
            dir = 0;

        currFrame = dir * currSprAnimation.framesPerStrip + currFrame % currSprAnimation.framesPerStrip;    // keep the same frame number
        currDirection = dir;
        sr.sprite = currSprAnimation.sprites[currFrame];
    }
    public void ChangeDirection(SPRITE_DIRECTION dir)
    {
        ChangeDirection((int)dir);
    }

    /// <summary>
    ///  Function to reset animation to start of this frame
    /// </summary>
    public void ResetAnimation()
    {
        currFrame = currSprAnimation.framesPerStrip * currDirection;    // frame 0 of this strip
        sr.sprite = currSprAnimation.sprites[currFrame];

        timeElapsed = 0f;
    }

    /// <summary>
    ///  Function to freeze or unfreeze animation
    /// </summary>
    public void SetFreezeAnimation(bool freeze)
    {
        freezeAnimation = freeze;
    }

    /// <summary>
    ///  Function to freeze animation for a set period of time
    /// </summary>
    public void SetFreezeAnimation(bool freeze, float time)
    {

    }

}

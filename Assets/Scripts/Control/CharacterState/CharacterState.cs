using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
Author:         Tanner Hunt
Date:           5/12/2024
Version:        0.1.0
Description:    A class that holds values the mover and velocity events reference
ChangeLog:      V 0.1.0 -- 5/12/2024
                    --This class now holds how intense gravity should be
                    --this class now holds the player velocity vector
                    --renamed characterstate to gravitystate
                    --Dev time: 3 Hours
*/
namespace Control{
public class CharacterState : MonoBehaviour
{
    public enum CharacterGravity{
    normalGamePlay,
    grappling,
    jumping
    }
    public CharacterGravity characterGravity {get; private set;}
    public Vector3 playerVelocity;

    public float gravityValue  {get; private set;}
    [SerializeField]public float defaultGravityValue;  //the strength of gravity during normal gameplay
    [SerializeField]public float grapplingGravityValue; //the strength of gravity while reeling on the grapple hook
    [SerializeField]public float jumpingGravityValue; //the momentary lightness a player feels until they press the spacebar

    void Start()
    {
        setGravityState(CharacterGravity.normalGamePlay);
    }
    
/// <summary>
/// allows other scripts to set the current player state for gravity purposes.
/// </summary>
/// <param name="characterState">declared CharacterMover.CharacterState.state,
/// states include normalGamePlay, reeling</param>
    public void setGravityState(CharacterGravity characterGravity){
        this.characterGravity = characterGravity;

        //update gravity value based on characterGravity
        switch(characterGravity)
            {
            case CharacterGravity.normalGamePlay:
                    gravityValue = defaultGravityValue;
                break;
            case CharacterGravity.grappling:
                    gravityValue = grapplingGravityValue;
                break;
            case CharacterGravity.jumping:
                    gravityValue = jumpingGravityValue;
                break;
            default:
                    gravityValue = defaultGravityValue;
                break;
        }
    }
}}
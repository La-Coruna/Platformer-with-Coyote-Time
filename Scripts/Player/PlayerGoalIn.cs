using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

// player에 삽입될 코드임

public class PlayerGoalIn : MonoBehaviour
{
    
    public PlayerMovement playerScript;
    public SpriteRenderer spriteRenderer;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Goal"))
        {
            Debug.Log("clear");
            playerScript.TurnOffPlay();
            playerScript.TurnOffPlay();
            TestManager.showClearMessage();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerKill : MonoBehaviour
{
    
    public Transform respawnPosition;
    public PlayerMovement playerScript;
    public static int death = 0;
    public SpriteRenderer spriteRenderer { get; set; }
    public Color originalColor = new Color(141, 255, 131, 255);
    public Color deathColor = new Color32(243,79,79,255);
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    void ChangeColorDeath()
    {
        spriteRenderer.color = deathColor;
    }
    void ChangeColorAlive()
    {
        spriteRenderer.color = originalColor;
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Obstacle") && !playerScript.isDead)
        {
            ChangeColorDeath();
            playerScript.TurnOffPlay();
            playerScript.TurnOffPlay();
            playerScript.isDead = true;
            playerScript.death++;
            StartCoroutine(RestartAfterDelay());
        }
    }
    
    private IEnumerator RestartAfterDelay()
    {
        yield return new WaitForSeconds(0.5f); 
        ChangeColorAlive();
        transform.position = respawnPosition.position;
        playerScript.isDead = false;
        playerScript.TurnOnPlay();
    }
}

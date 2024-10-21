using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ObstacleTrigger : MonoBehaviour
{
    public GameObject rockPrefab;
    public GameObject warning;
    public GameObject spawn_pos;
    public bool isStartedFalling = false;
    public float waitSecBeforeFalling = 0.5f;
    public float waitSecAfterFalling = 1f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && !isStartedFalling)
        {
            StartCoroutine(SpawnRockAfterDelay());
        }
    }
    
    private IEnumerator SpawnRockAfterDelay()
    {
        isStartedFalling = true;
        TurnOnWarning();
        yield return new WaitForSeconds(waitSecBeforeFalling); // 1초 기다림
        TurnOffWarning();
        SpawnRock();
        yield return new WaitForSeconds(waitSecAfterFalling); // 1초 기다림
        isStartedFalling = false;
    }

    private void SpawnRock()
    {
        Instantiate(rockPrefab, spawn_pos.transform.position, Quaternion.identity);
    }
    private void TurnOnWarning()
    {
        warning.SetActive(true);
    }
    private void TurnOffWarning()
    {
        warning.SetActive(false);
    }

    
}
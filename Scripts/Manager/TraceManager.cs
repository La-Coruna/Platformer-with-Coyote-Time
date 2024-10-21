using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TraceManager : MonoBehaviour
{
    public GameObject current_player;
    private PlayerMovement playerMovementScript;
    
    // trace 남기려고.
    public GameObject normalJumpTracePrefab;
    public GameObject coyoteJumpTracePrefab;
    public GameObject failedJumpTracePrefab;
    private List<GameObject> spawnedJumpTraces = new List<GameObject>(); // 생성된 프리팹들을 저장하기 위한 리스트

    public bool isTraceMode = false;

    private void Start()
    {
        playerMovementScript = current_player.GetComponent<PlayerMovement>();
    }


    void Update()
    {
        // Trace 관련
        if (Input.GetButtonDown("Jump") && isTraceMode)
        {
            SpawnJumpTraces();
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            ClearJumpTraces();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            isTraceMode = !isTraceMode;
        }
    }
    
    private void Respawn()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            current_player.transform.position = new Vector2(-16.17f, 1.45f);
        }
    }
    
    // 점프 트레이스를 생성하는 메서드
    private void SpawnJumpTraces()
    {
        GameObject newPrefab;
        if (playerMovementScript.isGrounded)
        {
            newPrefab = Instantiate(normalJumpTracePrefab, current_player.transform.position, Quaternion.identity);
        }
        else if (playerMovementScript.coyoteTimeCounter > 0f)
        {
            newPrefab = Instantiate(coyoteJumpTracePrefab, current_player.transform.position, Quaternion.identity);
        }
        else
        {
            newPrefab = Instantiate(failedJumpTracePrefab, current_player.transform.position, Quaternion.identity);
        }
            newPrefab.transform.localScale = current_player.transform.localScale;
            spawnedJumpTraces.Add(newPrefab); 
    }

    // 생성된 점프 트레이스를 모두 삭제하는 메서드
    public void ClearJumpTraces()
    {
        foreach (GameObject prefab in spawnedJumpTraces)
        {
            Destroy(prefab); // 생성된 프리팹을 삭제
        }
        spawnedJumpTraces.Clear(); // 리스트를 초기화
    }

    // 점프 트레이스를 활성화 또는 비활성화하는 메서드
    public void ToggleJumpTraces()
    {
        foreach (GameObject prefab in spawnedJumpTraces)
        {
            prefab.SetActive(!prefab.activeSelf); // 프리팹의 활성/비활성 상태를 반전
        }
    }


    void OnDestroy()
    {
        // 스크립트가 삭제될 때 생성된 모든 프리팹을 삭제
        ClearJumpTraces();
    }
}

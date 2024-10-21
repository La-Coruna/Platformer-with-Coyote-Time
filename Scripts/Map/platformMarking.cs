// 밟으면 움직일 수없고 자리에 마킹이 되는 발판
// 점프 거리 측정용.

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class platformMarking : MonoBehaviour
{
    public GameObject tracePrefab;
    public Transform respawnPosition;
    public GameObject player;
    public PlayerMovement playerScript;
    private List<GameObject> spawnedJumpTraces = new List<GameObject>(); // 생성된 프리팹들을 저장하기 위한 리스트

    private void OnCollisionEnter2D(Collision2D Other)
    {
        if (Other.gameObject.CompareTag("Player"))
        {
            GameObject newPrefab;
            newPrefab = Instantiate(tracePrefab, player.transform.position, Quaternion.identity);
            newPrefab.transform.localScale = player.transform.localScale;
            spawnedJumpTraces.Add(newPrefab);
            playerScript.TurnOffPlay();
            playerScript.TurnOffPlay();
            StartCoroutine(RestartAfterDelay());
        }
    }
    private IEnumerator RestartAfterDelay()
    {
        yield return new WaitForSeconds(2f); // 3초 기다림
        player.transform.position = respawnPosition.position;
        playerScript.TurnOnPlay();
    }
        
    public void ClearJumpTraces()
    {
        foreach (GameObject prefab in spawnedJumpTraces)
        {
            Destroy(prefab); // 생성된 프리팹을 삭제합니다.
        }
        spawnedJumpTraces.Clear(); // 리스트를 초기화합니다.
    }
}

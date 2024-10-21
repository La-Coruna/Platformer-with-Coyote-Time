using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine.Serialization;

public class ReplayManager : MonoBehaviour
{
    //실험 폴더 경로
    private static string folderName = "HCI_study1";

    private string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) + "/" +
                          folderName + "/";

    private List<ReplayData> replayDataList; // replay 데이터를 저장할 리스트
    private List<JumpPositionData> jumpPositionDataList; // 충돌한 위치
    private int currentIndex = 0; // 현재 인덱스
    private bool isReplaying = false; // replay 중인지 여부
    private bool hasData = false;

    // 프로퍼티
    public int CurrentIndex => currentIndex;
    public bool HasData => hasData;
    public bool IsReplaying => isReplaying;

    // 실험 정보
    public int participantsNumber = 0;
    public int trialNum = 1;

    public GameObject replayModeCharacter; // replay 할 GameObject

    public GameObject playModeCharacter;

    public GameObject normalJumpTracePrefab;
    public GameObject coyoteJumpTracePrefab;
    public GameObject failedJumpTracePrefab;
    private List<GameObject> spawnedJumpTraces = new List<GameObject>(); // 생성된 프리팹들을 저장하기 위한 리스트
    
    int ReadAllData()
    {
        int result = ReadMovementCSV() + ReadJumpPositionCSV(); // 성공했으면 0 하나라도 실패시 1
        return result;
    }


    public void TryLoadData()
    {
        hasData = ReadAllData() == 0; // 성공했으면 0이므로 not 취해줘서 true
    }

    // CSV 파일 읽기
    private int ReadMovementCSV()
    {
        // 실험 폴더 경로 + csv 파일 경로
        string csvFilePath = path + participantsNumber + "번 피험자/" + trialNum + "/PlayerMovement.csv";

        replayDataList = new List<ReplayData>();

        // 파일 존재 여부 확인
        if (!File.Exists(csvFilePath))
        {
           // Debug.LogError("CSV 파일이 존재하지 않습니다: " + csvFilePath);
            return 1; // 파일이 없으므로 1을 반환
        }

        // CSV 파일 읽기
        string[] lines = File.ReadAllLines(csvFilePath);

        // 첫 번째 라인은 헤더이므로 무시하고 두 번째 라인부터 데이터를 읽음
        for (int i = 1; i < lines.Length - 1; i++)
        {
            string[] values = lines[i].Split(',');
            ReplayData replayData = new ReplayData();
            replayData.CurrentTime = float.Parse(values[0]);
            replayData.Position = new Vector2(float.Parse(values[1]), float.Parse(values[2]));
            replayData.isRight = float.Parse(values[3]);
            replayDataList.Add(replayData);
        }

        return 0; // 파일을 성공적으로 읽었으므로 0을 반환
    }

    private int ReadJumpPositionCSV()
    {
        // 실험 폴더 경로 + csv 파일 경로
        string csvFilePath = path + participantsNumber + "번 피험자/" + trialNum + "/JumpPosition.csv";

        jumpPositionDataList = new List<JumpPositionData>();

        // 파일 존재 여부 확인
        if (!File.Exists(csvFilePath))
        {
            // Debug.LogError("CSV 파일이 존재하지 않습니다: " + csvFilePath);
            return 1; // 파일이 없으므로 1을 반환
        }

        // CSV 파일 읽기
        string[] lines = File.ReadAllLines(csvFilePath);

        // 첫 번째 라인은 헤더이므로 무시하고 두 번째 라인부터 데이터를 읽음
        for (int i = 1; i < lines.Length - 1; i++)
        {
            string[] values = lines[i].Split(',');
            JumpPositionData jumpPositionData = new JumpPositionData();
            jumpPositionData.CurrentTime = float.Parse(values[0]);
            jumpPositionData.Position = new Vector2(float.Parse(values[1]), float.Parse(values[2]));
            jumpPositionData.isRight = float.Parse(values[3]);
            jumpPositionData.timeFromDropToJump = float.Parse(values[4]);
            jumpPositionDataList.Add(jumpPositionData);
        }

        return 0; // 파일을 성공적으로 읽었으므로 0을 반환
    }

    // Replay 하려는 CSV가 달라진 경우 감지
    private void OnValidate()
    {
        isReplaying = false;
        currentIndex = 0;
        TryLoadData();
    }

    // Replay 시 카메라 전환 및 플레이어 오브젝트 변경
    public void PrepareReplay()
    {
        playModeCharacter.SetActive(false);
        replayModeCharacter.SetActive(true);
        //replayModeCamera.SetActive(true);
        return;
    }

    // Replay 종료 시 카메라 전환 및 플레이어 오브젝트 변경
    public void WrapUpReplay()
    {
        playModeCharacter.SetActive(true);
        replayModeCharacter.SetActive(false);
        return;
    }


    // Replay 시작
    public void StartReplay()
    {
        if (playModeCharacter.activeSelf || !replayModeCharacter.activeSelf || !replayModeCharacter)
        {
            PrepareReplay();
        }

        isReplaying = true;
        StartCoroutine(ReplayCoroutine());
        
        ClearJumpTraces();
        SpawnJumpTraces();
        ToggleJumpTraces();
    }
    
    // Resume replay
    public void ResumeReplay()
    {
        if (playModeCharacter.activeSelf || !replayModeCharacter.activeSelf || !replayModeCharacter)
        {
            PrepareReplay();
        }

        isReplaying = true;
        StartCoroutine(ReplayCoroutine());
    }

    // Replay 일시 중지
    public void PauseReplay()
    {
        isReplaying = false;
    }

    // Replay Coroutine
    private IEnumerator ReplayCoroutine()
    {
        float startTime = Time.time; // replay 시작 시간 저장

        // resume할 시, currentIndex의 시간이 될 때까지 안 기다리도록.
        if (currentIndex > 0)
            startTime -= replayDataList[currentIndex].CurrentTime;

        // currentIndex가 replayDataList의 범위 내에 있고 isReplaying이 true인 동안 반복
        while (isReplaying)
        {
            if (currentIndex < replayDataList.Count)
            {
                ReplayData replayData = replayDataList[currentIndex];
                float elapsedTime = Time.time - startTime; // 현재까지의 경과 시간 계산

                // 현재 경과 시간이 replayData의 시간에 도달할 때까지 기다림
                while (elapsedTime < replayData.CurrentTime)
                {
                    elapsedTime = Time.time - startTime;
                    yield return null;
                }

                // replayModeWheelchair의 위치와 회전을 설정
                replayModeCharacter.transform.position = replayData.Position;
                replayModeCharacter.transform.localScale = new Vector3(replayData.isRight, 1, 1);

                currentIndex++;
            }
            else
            {
                currentIndex = 0;
                isReplaying = false;
                break;
            }
        }
    }

    public void OpenFolder()
    {
        System.Diagnostics.Process.Start(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) +
                                         "/" + folderName + "/" + participantsNumber + "번 피험자/" + trialNum + "/");
    }

    // 점프 트레이스를 생성하는 메서드
    private void SpawnJumpTraces()
    {
        // 저장된 점프 위치 데이터를 기반으로 프리팹을 생성합니다.
        foreach (JumpPositionData jumpPositionData in jumpPositionDataList)
        {
            GameObject newPrefab;
            if (jumpPositionData.timeFromDropToJump == 0)
            {
                newPrefab = Instantiate(normalJumpTracePrefab, jumpPositionData.Position, Quaternion.identity);
            }
            else if (jumpPositionData.timeFromDropToJump > 0f)
            {
                newPrefab = Instantiate(coyoteJumpTracePrefab, jumpPositionData.Position, Quaternion.identity);
            }
            else
            {
                newPrefab = Instantiate(failedJumpTracePrefab, jumpPositionData.Position, Quaternion.identity);
            }
            
            newPrefab.transform.localScale = new Vector3(jumpPositionData.isRight,1,1);
            spawnedJumpTraces.Add(newPrefab); 
        }
    }

    // 생성된 점프 트레이스를 모두 삭제하는 메서드
    private void ClearJumpTraces()
    {
        foreach (GameObject prefab in spawnedJumpTraces)
        {
            Destroy(prefab); // 생성된 프리팹을 삭제합니다.
        }
        spawnedJumpTraces.Clear(); // 리스트를 초기화합니다.
    }

    // 점프 트레이스를 활성화 또는 비활성화하는 메서드
    public void ToggleJumpTraces()
    {
        foreach (GameObject prefab in spawnedJumpTraces)
        {
            prefab.SetActive(!prefab.activeSelf); // 프리팹의 활성/비활성 상태를 반전시킵니다.
        }
    }


    void OnDestroy()
    {
        // 스크립트가 삭제될 때 생성된 모든 프리팹을 삭제합니다.
        ClearJumpTraces();
    }
}

public class JumpPositionData
{
    public float CurrentTime;
    public Vector2 Position;
    public float isRight;
    public float timeFromDropToJump;
}

// Replay 데이터를 담는 클래스
public class ReplayData
{
    public float CurrentTime;
    public Vector2 Position;
    public float isRight;
}
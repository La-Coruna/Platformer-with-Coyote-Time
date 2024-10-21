using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.Serialization;

[Serializable()]
public class RecordPathCSV : MonoBehaviour
{
    // 디렉토리 저장 경로
    static string path;
    private static string folderName = "HCI_studyAB";

    public int _participantsNumber;
    public static int participantsNumber;
    public int trialNum = 0;

    public Transform _player;
    public PlayerMovement playerMovementScript;
    public Rigidbody2D rb;

    public float endTime = 9999.0f;

    static CsvFileWriter jumpPositionCSV;
    static List<string> jumpPositionColumns;

    static CsvFileWriter playerMovementCSV;
    static List<string> playerMovementColumns;   
    
    public static bool isEnded = true;

    public static float currentTime = 0f;

    public List<float> collisionTimes = new List<float>();
    public int collisionIdx = 0;

    
    
    public void CreateCSVFile()
    {
        currentTime = 0;
        
        participantsNumber = _participantsNumber;

        path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) +
               "/" + folderName + "/" + participantsNumber + "번 피험자/";

        // 폴더 유무 확인
        DirectoryInfo di = new DirectoryInfo(path);

        while (di.Exists)    // ex) trial 1 폴더가 이미 있으면 trial 2 폴더를 생성하게끔 설정 (1, 2 존재 -> 3 생성)
        {
            ++trialNum;
            path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) +
                   "/" + folderName + "/" + participantsNumber + "번 피험자/" + trialNum + "/";
            di = new DirectoryInfo(path);
        }

        // 폴더가 없으면 폴더 생성
        if (!di.Exists)
            di.Create();

        jumpPositionCSV = new CsvFileWriter(path + "JumpPosition.csv");
        jumpPositionColumns = new List<string>() { "CurrentTime", "JumpPosition_x", "JumpPosition_y","Scale_X", "Time from drop to jump", "isSuccess" };
        playerMovementCSV = new CsvFileWriter(path + "PlayerMovement.csv");
        playerMovementColumns = new List<string>() { "CurrentTime", "Position_x", "Position_y", "Scale_X" };
        
        jumpPositionCSV.WriteRow(jumpPositionColumns);
        playerMovementCSV.WriteRow(playerMovementColumns);
    }

    public void writeDeath()
    {
        playerMovementColumns.Clear();
        playerMovementColumns.Add("Death Count:");
        playerMovementColumns.Add(playerMovementScript.death.ToString());
        playerMovementCSV.WriteRow(playerMovementColumns);
    }
    
    
    public void CloseCSVFile()
    {
        writeDeath();
        
        jumpPositionCSV.Dispose();
        playerMovementCSV.Dispose();
    }

    // isEnded가 false면 recording.
    private void Start()
    {
        currentTime = 0;
        
        participantsNumber = _participantsNumber;

        path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) +
               "/" + folderName + "/" + participantsNumber + "번 피험자/";

        // 폴더 유무 확인
        DirectoryInfo di = new DirectoryInfo(path);

        while (di.Exists)    // ex) trial 1 폴더가 이미 있으면 trial 2 폴더를 생성하게끔 설정 (1, 2 존재 -> 3 생성)
        {
            ++trialNum;
            path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) +
                   "/" + folderName + "/" + participantsNumber + "번 피험자/" + trialNum + "/";
            di = new DirectoryInfo(path);
        }

        // 폴더가 없으면 폴더 생성
        if (!di.Exists)
            di.Create();
    }

    private void Update()
    {
        if (!isEnded)
        {
            Check_Timer();
            WritingPlayerMovementData();

            if (Input.GetButtonDown("Jump"))
            {
                WritingJumpData();
            }

            if (currentTime > endTime)
            {
                End_Timer();
            }
        }
    }

    public void StartTest()
    {
        Start_Timer();
        CreateCSVFile();
    }

    public void EndTest()
    {
        End_Timer();
        CloseCSVFile();
    }

    // Timer 설정
    private void Check_Timer()
    {
        currentTime += Time.deltaTime;
    }

    public void Start_Timer()
    {
        isEnded = false;
        Debug.Log("Start");
    }

    public void End_Timer()
    {
        Debug.Log("End. Time is " + currentTime);
        isEnded = true;
    }

    void WritingPlayerMovementData()
    {
        // Player의 현재 위치 기록
        Vector2 playerPosition = _player.position;
        float playerScaleX = _player.localScale.x;

        playerMovementColumns.Clear();
        playerMovementColumns.Add(currentTime.ToString());
        playerMovementColumns.Add(playerPosition.x.ToString());
        playerMovementColumns.Add(playerPosition.y.ToString());
        playerMovementColumns.Add(playerScaleX.ToString());

        playerMovementCSV.WriteRow(playerMovementColumns);
    }

    void WritingJumpData()
    {
        // Player의 점프 위치 기록
        Vector2 playerPosition = _player.position;
        float playerScaleX = _player.localScale.x;
        float timeFromDropToJump = playerMovementScript.coyoteTime - playerMovementScript.coyoteTimeCounter;
        bool isSuccess = (playerMovementScript.coyoteTime > 0)
            ? (playerMovementScript.coyoteTimeCounter > 0f && !playerMovementScript.isJumping)
            : (Input.GetButtonDown("Jump") && playerMovementScript.isGrounded);

        jumpPositionColumns.Clear();
        jumpPositionColumns.Add(currentTime.ToString());
        jumpPositionColumns.Add(playerPosition.x.ToString());
        jumpPositionColumns.Add(playerPosition.y.ToString());
        jumpPositionColumns.Add(playerScaleX.ToString());
        jumpPositionColumns.Add(timeFromDropToJump.ToString());
        jumpPositionColumns.Add(isSuccess.ToString());
        
        jumpPositionCSV.WriteRow(jumpPositionColumns);
    }

    private void OnApplicationQuit()
    {
        // 어플리케이션이 종료될 때 CSV 파일을 닫음
        jumpPositionCSV?.Dispose();
        playerMovementCSV?.Dispose();
    }

    public void OpenFolder()
    {
        // 결과 폴더를 열기 위해 Explorer를 실행
        System.Diagnostics.Process.Start(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) + "/" + folderName + "/" + participantsNumber + "번 피험자/" + trialNum + "/");
    }
    
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
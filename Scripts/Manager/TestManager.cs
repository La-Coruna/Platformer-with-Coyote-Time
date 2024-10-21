using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class TestManager : MonoBehaviour
{
    public static int gameTrial = 1;
    public static bool isCoyote = false;
    
    public static GameObject clearMessage1Trial;
    public static GameObject clearMessage2Trial;
    public static GameObject startButton;
    
    public static GameObject player;
    public static PlayerMovement playerScript;
    public static Transform respawnPosition;

    public static RecordPathCSV recorder;    
    // static 변수를 할당해주기 위한 변수들
    public bool _isCoyote = false;
    public  GameObject _clearMessage1Trial;
    public  GameObject _clearMessage2Trial;
    public  GameObject _startButton;
    
    public  GameObject _player;
    public  PlayerMovement _playerScript;
    public  Transform _respawnPosition;

    public  RecordPathCSV _recorder;

    public float trainingTime = 30f;
    
    void Start()
    {
        clearMessage1Trial = _clearMessage1Trial;
        clearMessage2Trial = _clearMessage2Trial;
        startButton = _startButton;

        isCoyote = _isCoyote;
    
        player = _player;
        playerScript = _playerScript;
        respawnPosition = _respawnPosition;

        recorder = _recorder;
        
        //연습모드에서 게임 시작 버튼
        StartCoroutine(ShowStartButton());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            Cheat();
        }
        // 코요테 관련
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _playerScript.coyoteTime = 0f;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _playerScript.coyoteTime = 0.05f;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            _playerScript.coyoteTime = 0.1f;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            _playerScript.coyoteTime = 1f;
        }
    }

    public static void showClearMessage()
    {
        // record 종료
        recorder.EndTest();
        if(gameTrial == 1)
            clearMessage1Trial.SetActive(true);
        else
            clearMessage2Trial.SetActive(true);
    }
    public static void CloseClearMessage()
    {
        clearMessage1Trial.SetActive(false);
        clearMessage2Trial.SetActive(false);
    }

    public static void CloseStartButton()
    {
        startButton.SetActive(false);
    }
    public IEnumerator ShowStartButton()
    {
        yield return new WaitForSeconds(trainingTime);
        startButton.SetActive(true);
    }

    public static void LinkToCoyoteSurvey()
    {
        Application.OpenURL("https://forms.gle/f1LGsD9UZuzqqPg36");
    }
    public static void LinkToNormalSurvey()
    {
        Application.OpenURL("https://forms.gle/N6L8VsBjbT3cf3Qm6");
    }

    public static void LinkToSurvey()
    {
        recorder.OpenFolder();
        if (isCoyote)
            LinkToCoyoteSurvey();
        else
            LinkToNormalSurvey();
    }
    
    public static void Replay()
    {
        RestartPlayerAfterClear();
        CloseClearMessage();
    }
    public static void StartGame()
    {
        if(isCoyote)
            playerScript.coyoteTime = 0.05f;
        else
            playerScript.coyoteTime = 0f;

        player.transform.position = respawnPosition.position;
        playerScript.TurnOnPlay();
        
        CloseStartButton();
        recorder.StartTest();
    }
    
    public static void RestartPlayerAfterClear()
    {
        gameTrial++;
        isCoyote = !isCoyote;
        if(isCoyote)
            playerScript.coyoteTime = 0.05f;
        else
            playerScript.coyoteTime = 0f;
            
        // 플레이어 위치 조정
        playerScript.InitDeath();
        player.transform.position = respawnPosition.position;
        playerScript.TurnOnPlay();
        
        // record 시작 2
        recorder.StartTest();
    }
    
    public void Cheat()
    {
        player.transform.position = new Vector2(116, 67);
    }
}

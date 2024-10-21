using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buttonFunction : MonoBehaviour
{
    public void GoToSurvey()
    {
        TestManager.LinkToSurvey();
    }
    public void Replay()
    {
        TestManager.Replay();
    }
    public void StartGame()
    {
        TestManager.StartGame();
    }
}

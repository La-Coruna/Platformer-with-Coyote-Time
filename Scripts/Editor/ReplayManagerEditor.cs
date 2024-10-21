using UnityEngine;
using UnityEditor;
using System.CodeDom.Compiler;

[CustomEditor(typeof(ReplayManager))]
public class ReplayManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        ReplayManager replayManager = (ReplayManager)target;

        // 데이터가 있을 때
        if (replayManager.HasData)
        {
            // replay중이 아닐 때
            if (!replayManager.IsReplaying)
            {
                // 재생을 한번도 안 눌렀을 때, start 
                if (replayManager.CurrentIndex == 0)
                {
                    if (GUILayout.Button("Start Replay"))
                    {
                        replayManager.StartReplay();
                    }
                }
                // 재생 중에 잠시 멈췄던 거였을 때, resume replay
                else
                {
                    if (GUILayout.Button("Resume Replay"))
                    {
                        replayManager.ResumeReplay();
                    }
                }

                // 리플레이모드로 들어가거나 나가거나.
                // switch camera and object
                if (!replayManager.playModeCharacter.activeSelf)
                {
                    if (GUILayout.Button("Switch to Play Mode"))
                    {
                        replayManager.WrapUpReplay();
                    }
                }
                else
                {
                    if (GUILayout.Button("Switch to Replay Mode"))
                    {
                        replayManager.PrepareReplay();
                    }
                }

            }
            // replay 중일 때
            else
            {
                if (GUILayout.Button("Pause Replay"))
                {
                    replayManager.PauseReplay();
                }
            }
            
            if (GUILayout.Button("Toggle Jump Trace"))
            {
                replayManager.ToggleJumpTraces();
            }

            if (GUILayout.Button("Open Folder"))
            {
                replayManager.OpenFolder();
            }
        }
        else // data가 없는 경우
        {
            if (GUILayout.Button("Load Data"))
            {
                replayManager.TryLoadData();
            }
        }
    }
}

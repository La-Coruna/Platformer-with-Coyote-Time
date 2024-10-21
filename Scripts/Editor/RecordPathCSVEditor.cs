using UnityEngine;
using UnityEditor;
using System.CodeDom.Compiler;

[CustomEditor(typeof(RecordPathCSV))]
public class RecordPathCSVEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        RecordPathCSV recordPathCSV = (RecordPathCSV)target;

        if (GUILayout.Button("Start Test"))
        {
            recordPathCSV.StartTest();
        }

        if (GUILayout.Button("Force Stop"))
        {
            recordPathCSV.EndTest();
        }
        
        if (GUILayout.Button("Open Folder"))
        {
            recordPathCSV.OpenFolder();
        }
        
        if (GUILayout.Button("End Game"))
        {
            recordPathCSV.QuitGame();
        }
    }
}

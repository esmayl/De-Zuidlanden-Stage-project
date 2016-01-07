using UnityEngine;
using UnityEditor;
using System.Collections;

public class SaveWindow : EditorWindow {

    [MenuItem("Window/SaveWindow")]
    static void Init()
    {
        SaveWindow window = (SaveWindow)EditorWindow.GetWindow<SaveWindow>();
        window.Show(true);
    }

    void OnGUI()
    {
        if (GUILayout.Button("Save"))
        {
            EditorApplication.SaveScene(Application.dataPath+"/Juiste plaatsing.unity", true);
        }
    }
}

using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "New Dialogue/Dialogue")]
public class DialogueSettings : ScriptableObject
{
    [Header("Settings")]
    public GameObject actor;

    [Header("Dialogue")]
    public Sprite speakerSprite;
    public string sentence;

    public List<Sentences> dialogues = new List<Sentences>();
}

[System.Serializable]
public class Sentences
{
    public string actorName;
    public Sprite profile;
    public Languages sentence;
}

[System.Serializable]
public class Languages
{
    public string portuguese;
    public string english;
    public string spanish;
}

//if inspector open, create usable items in there
#if UNITY_EDITOR
[CustomEditor (typeof(DialogueSettings))]
public class BuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //DrawDefaultInspector();
        base.OnInspectorGUI();

        DialogueSettings dialogSettings = (DialogueSettings)target;
        Languages languages = new Languages();

        //record string on english
        languages.english = dialogSettings.sentence;

        Sentences s = new Sentences();
        s.profile = dialogSettings.speakerSprite;
        s.sentence = languages;

        if (GUILayout.Button("Create Dialogue"))
        {
            if (dialogSettings.sentence != "")
            {
                dialogSettings.dialogues.Add(s);

                dialogSettings.speakerSprite = null;
                dialogSettings.sentence = "";
            }
        }
    }
}

#endif
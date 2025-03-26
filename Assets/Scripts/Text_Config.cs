using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Text_Config", menuName = "Config/TextConfig", order = 1)]
public class Text_Config : ScriptableObject
{
    [Header("InputDialogues")]
    public List<String> inputDialogues_0;
    
    public List<String> inputDialogues_1;
    public List<String> inputDialogues_2;

    [Header("PreDialogues")]
    public List<String> preDialogues;
}


[Serializable]
public class DialogueText
{
    public string text;

    public DialogueText(string inputText)
    {
        text = inputText;
    }
}

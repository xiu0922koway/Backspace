using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Text_Config", menuName = "Config/TextConfig", order = 1)]
public class Text_Config : ScriptableObject
{
    [Header("InputDialogues")]
    public List<String> inputDialogues_0;
    [Tooltip("对应 inputDialogues_0 的每个对话的打字速度曲线。x轴=归一化位置(0-1，0=开头，1=结尾)，y轴=时间间隔(秒)。如果为空或数量不匹配，将使用默认速度。")]
    public List<AnimationCurve> typingCurves_0;
    [Tooltip("对应 inputDialogues_0 的替代文本。当某个位置的字符被删除后重新填充时，会使用替代文本中对应位置的字符。长度应与原文本相同。")]
    public List<String> alternativeDialogues_0;
    
    public List<String> inputDialogues_1;
    [Tooltip("对应 inputDialogues_1 的每个对话的打字速度曲线。x轴=归一化位置(0-1，0=开头，1=结尾)，y轴=时间间隔(秒)。如果为空或数量不匹配，将使用默认速度。")]
    public List<AnimationCurve> typingCurves_1;
    [Tooltip("对应 inputDialogues_1 的替代文本。当某个位置的字符被删除后重新填充时，会使用替代文本中对应位置的字符。长度应与原文本相同。")]
    public List<String> alternativeDialogues_1;
    
    public List<String> inputDialogues_2;
    [Tooltip("对应 inputDialogues_2 的每个对话的打字速度曲线。x轴=归一化位置(0-1，0=开头，1=结尾)，y轴=时间间隔(秒)。如果为空或数量不匹配，将使用默认速度。")]
    public List<AnimationCurve> typingCurves_2;
    [Tooltip("对应 inputDialogues_2 的替代文本。当某个位置的字符被删除后重新填充时，会使用替代文本中对应位置的字符。长度应与原文本相同。")]
    public List<String> alternativeDialogues_2;

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

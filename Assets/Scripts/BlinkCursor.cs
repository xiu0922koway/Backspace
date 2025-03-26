using UnityEngine;
using TMPro;
using System.Collections;

public class BlinkCursor : MonoBehaviour
{
    public TMP_Text mainText;    // 你的实时文本组件
    public  TMP_Text cursorText;  // 用于显示光标的组件
    public  float blinkInterval = 0.5f;  // 光标闪烁间隔

    private bool isCursorVisible = true;

    private void Start()
    {
        // 启动光标闪烁的协程
        StartCoroutine(BlinkingCursor());
    }

    private IEnumerator BlinkingCursor()
    {
        while (true)
        {
            // 切换光标显示状态
            isCursorVisible = !isCursorVisible;
            if(isCursorVisible)
            {
                cursorText.text = "|";
            }
            else
            {
                cursorText.text = "";
            }


            yield return new WaitForSeconds(blinkInterval);
        }
    }

    private void Update()
    {
        // 获取文本信息
        TMP_TextInfo textInfo = mainText.textInfo;
        
        if (textInfo.characterCount > 0)
        {
            // 获取最后一个字符的信息
            int lastIndex = textInfo.characterCount - 1;
            TMP_CharacterInfo lastCharInfo = textInfo.characterInfo[lastIndex];

            // 计算光标的位置 (xAdvance 用于水平位置, baseline 用于垂直居中)
            Vector3 cursorPosition = new Vector3(
                lastCharInfo.xAdvance,
                lastCharInfo.baseLine,
                0
            );

            // 将光标位置转换为世界坐标
            cursorText.rectTransform.position = mainText.transform.TransformPoint(cursorPosition);
        }
        else
        {
            TMP_LineInfo firstLineInfo = textInfo.lineInfo[0];

            // 将光标设置到第一行的基线位置，并贴紧左侧
            Vector3 cursorPosition = new Vector3(
                mainText.rectTransform.rect.xMin, // 最左侧
                firstLineInfo.baseline,          // 基线垂直位置
                0
            );

            // 转换为世界坐标
            cursorText.rectTransform.position = mainText.transform.TransformPoint(cursorPosition);
        }
    }

    public void ChangeMainText(TMP_Text newText)
    {
        mainText = newText;
    }
}

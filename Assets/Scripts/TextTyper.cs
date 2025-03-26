using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class TextTyper : MonoBehaviour
{
    public List<TMP_Text> predialogueTMP = new List<TMP_Text>();
    private TMP_Text inputText;
    public float typingTime = 0.2f;
    public float sendTime = 1f;
    // Start is called before the first frame update
    void Start()
    {
        inputText = this.GetComponent<TMP_Text>();
        inputText.text = "";
    }

    // Update is called once per frame
    public void StartTyping(int index)
    {
        StartCoroutine(TypeCoroutine(index));
    }

    // Type协程，用于逐渐显示文本
    IEnumerator TypeCoroutine(int index)
    {
        inputText.text = ""; // 清空已有文本

        // 等到文字显示完成
        while (inputText.text.Length < predialogueTMP[index].text.Length)
        {
            // 每次增加一位字符
            inputText.text += predialogueTMP[index].text[inputText.text.Length];
            if(inputText.text[inputText.text.Length-1] != ' ') AudioManager.Instance.PlayOneShot(0, Random.Range(0.2f,0.4f));

            // 控制打字速度，可以根据 typingTime 来调整每次打字的延时
            yield return new WaitForSeconds(typingTime);
        }

        yield return new WaitForSeconds(sendTime);

        inputText.text = "";
        AudioManager.Instance.PlayOneShot(3, 0.4f);
    }
}

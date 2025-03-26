using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;


// [ExecuteInEditMode]
public class DialogueBoxController : MonoBehaviour
{
    public bool left;
    private TMP_Text tmpText;
    public string initialText;
    private int index;
    public float distance = 100;
    public float maxWidth = 350;

    public float ellipsisTime = 2;
    private float ellipsisInterval = 0.3f;
    // Start is called before the first frame update
    void Start()
    {

    }

    void Update()
    {
        
    }

    void Initiate()
    {
        tmpText = this.GetComponent<TMP_Text>();
        index = this.transform.GetSiblingIndex();
    }

    void OnEnable()
    {
        Initiate();
        if(left)
        {
            StartCoroutine(TypeEllipsis());
        }
    }

    IEnumerator TypeEllipsis()
    {
        float timer = 0f;
        int ellipsisIndex = 0;
        string[] ellipsisStates = { "•", "• •", "• • •",};

        while (timer < ellipsisTime)
        {
            // 循环切换显示
            tmpText.text = ellipsisStates[ellipsisIndex];
            ellipsisIndex = (ellipsisIndex + 1) % ellipsisStates.Length;

            // 等待0.5秒
            yield return new WaitForSeconds(ellipsisInterval);

            // 增加计时器
            timer += ellipsisInterval;
        }
        // 恢复为原始文本
        AudioManager.Instance.PlayOneShot(2, 0.2f);
        tmpText.text = initialText;
    }

    public void ConfigureDialogueBox()
    {
        tmpText = this.GetComponent<TMP_Text>();

        index = this.transform.GetSiblingIndex();

        
        for(int i = 0; i <3; i++)
        {
            i += 1;
            if(tmpText.preferredWidth < maxWidth)
            {
                tmpText.rectTransform.sizeDelta = new Vector2(tmpText.preferredWidth, tmpText.preferredHeight);
            }
            else
            {
                tmpText.rectTransform.sizeDelta = new Vector2(maxWidth, tmpText.preferredHeight);
            }

            if(index > 0)
            {
                Debug.Log("index > 0");
                float y = this.transform.parent.GetChild(index-1).GetComponent<RectTransform>().anchoredPosition.y;
                y = y - this.transform.parent.GetChild(index-1).GetComponent<TMP_Text>().preferredHeight/2;
                y = y - tmpText.preferredHeight/2;
                y = y - distance;
                
                tmpText.rectTransform.anchoredPosition = new Vector2(tmpText.rectTransform.anchoredPosition.x, y);
            }
        }
    }

    // void OnDrawGizmos()
    // {
    //     ConfigureDialogueBox();   
    // }
}

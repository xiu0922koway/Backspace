using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextRolling : MonoBehaviour
{
    public float width;
    public RectTransform reference;
    
    public List<TMP_Text> tmpTexts;
    // Start is called before the first frame update
    void Start()
    {
        tmpTexts = new List<TMP_Text>();
        tmpTexts.AddRange(this.transform.GetComponentsInChildren<TMP_Text>());
    }

    // Update is called once per frame
    public void ConfigureRolling()
    {
        foreach(var tmpText in tmpTexts)
        {

        }
    }

    public void AdjustDistance(RectTransform tmpText)
    {
        Vector3[] tempTextCorners = new Vector3[4];
        Vector3[] referenceCorners = new Vector3[4];
        tmpText.GetWorldCorners(tempTextCorners);
        reference.GetWorldCorners(referenceCorners);

        // 获取 rectA 和 rectB 的下边缘位置（左下角的 y 坐标）
        float bottomEdgeA = tempTextCorners[0].y;
        float bottomEdgeB = referenceCorners[0].y;

        // 计算 rectA 下边缘与 rectB 下边缘之间的距离
        float distance = Mathf.Abs(bottomEdgeA - bottomEdgeB);

        // 如果距离小于 minimumDistance，则向上移动 rectA
        if (distance < width)
        {
            float offset = width - distance;
            this.GetComponent<RectTransform>().anchoredPosition += new Vector2(0, offset);
            Debug.Log("Adjust");
        }
    }
}

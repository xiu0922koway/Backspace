using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.SceneManagement;
using System.Linq;

// 单词信息结构
[System.Serializable]
public class WordInfo
{
    public int startIndex; // 单词在原文本中的起始位置（包含）
    public int endIndex;   // 单词在原文本中的结束位置（不包含，即下一个字符的位置）
    public int wordIndex;  // 单词在数组中的索引
}

public class Backspace : MonoBehaviour
{
    public bool isChinese;
    public float cnTypeSoundTime = 0.075f;
    private float cnTypeSoundTimer;
    public Text_Config textConfig;
    private List<string> inputDialogues;
    private List<string> alternativeDialogues; // 存储当前使用的替代文本列表
    private List<AnimationCurve> typingCurves; // 存储当前使用的曲线列表
    private List<WordInfo> currentWordInfoList; // 当前对话的单词信息列表
    private HashSet<int> deletedWordIndices; // 记录被完全删除的单词索引
    public TextRolling textRolling;
    private int dialogueIndex;
    private int currentIndex;
    public TMP_Text TMP;

    public List<TMP_Text> preDialogues;


    public float waitTime = 2;
    private float waitTimer = 0;
    public bool isDeleting = false;

    public float typeTime = 0.5f; // 默认打字间隔（当没有曲线时使用）
    private float typeTimer = 0;
    private float currentCharInterval = 0.5f; // 当前字符的间隔时间

    public float buttonTime = 0.2f;
    public float firstButtonTime = 0.2f;
    private float buttonTimer = 0;
    private bool firstHold = false;

    private bool isEnd = false;

    private bool unlockEnter = false;

    private bool realEnd = false;
    
    public bool randomOrder;
    // Start is called before the first frame update
    void Start()
    {
        inputDialogues = new List<string>();
        inputDialogues.AddRange(textConfig.inputDialogues_0);
        alternativeDialogues = new List<string>();
        if(textConfig.alternativeDialogues_0 != null && textConfig.alternativeDialogues_0.Count > 0)
        {
            alternativeDialogues.AddRange(textConfig.alternativeDialogues_0);
        }
        typingCurves = new List<AnimationCurve>();
        if(textConfig.typingCurves_0 != null && textConfig.typingCurves_0.Count > 0)
        {
            typingCurves.AddRange(textConfig.typingCurves_0);
        }
        dialogueIndex = 0;
        deletedWordIndices = new HashSet<int>();
        
        TMP = GameObject.FindGameObjectWithTag("Input").GetComponent<TMP_Text>();

        currentIndex = 0;
        TMP.text = "";
        ParseWordsForCurrentDialogue(); // 解析当前对话的单词
        UpdateCurrentCharInterval(); // 初始化第一个字符的间隔
    }

    // Update is called once per frame
    void Update()
    {       
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadSceneAsync(0);
        }
        
        if(realEnd) return;
        
        if(Input.GetKeyUp(KeyCode.Backspace))
        {
            isDeleting = true;
            firstHold = false;
            if(waitTimer > 0) waitTimer = 0;
            buttonTimer = 0;

            RemoveChar();
        }

        if(Input.GetKeyDown(KeyCode.Backspace))
        {
            firstHold = true;
        }
        
        if(Input.GetKey(KeyCode.Backspace))
        {
            if(firstHold)
            {
                if(buttonTimer < firstButtonTime)
                {
                    buttonTimer += Time.deltaTime;
                }
                else
                {
                    firstHold = false;
                    buttonTimer = 0;
                }
            }
            else
            {
                if(buttonTimer < buttonTime)
                {
                    buttonTimer += Time.deltaTime;
                }
                else
                {
                    buttonTimer = 0;
                    isDeleting = true;
                    firstHold = false;
                    if(waitTimer > 0) waitTimer = 0;

                    RemoveChar();
                }
            }
        }
        

        if(isDeleting)
        {
            if(waitTimer < waitTime)
            {
                waitTimer += Time.deltaTime;
            }
            else
            {
                isDeleting = false;
            }
        }
        else
        {
            if(TMP.text.Length < inputDialogues[currentIndex].Length)
            {
                if(typeTimer < currentCharInterval)
                {
                    typeTimer += Time.deltaTime;
                                  
                }
                else
                {
                    int currentPos = TMP.text.Length;
                    char charToAdd;
                    
                    // 判断当前字符属于哪个单词，如果该单词被完全删除过，使用替代文本
                    int wordIndex = GetWordIndexForPosition(currentPos);
                    bool useAlternative = wordIndex >= 0 && deletedWordIndices.Contains(wordIndex) && 
                                         currentIndex < alternativeDialogues.Count && 
                                         alternativeDialogues[currentIndex] != null &&
                                         currentPos < alternativeDialogues[currentIndex].Length;
                    
                    if(useAlternative)
                    {
                        charToAdd = alternativeDialogues[currentIndex][currentPos];
                        // 调试信息：只在单词的第一个字符时打印
                        var wordInfo = currentWordInfoList.Find(w => w.wordIndex == wordIndex);
                        if(wordInfo != null && currentPos == wordInfo.startIndex)
                        {
                            string originalWord = inputDialogues[currentIndex].Substring(wordInfo.startIndex, wordInfo.endIndex - wordInfo.startIndex);
                            string altWord = alternativeDialogues[currentIndex].Substring(wordInfo.startIndex, wordInfo.endIndex - wordInfo.startIndex);
                            Debug.Log($"[Backspace] 使用替代文本: 位置 {currentPos}, 单词索引 {wordIndex}, \"{originalWord}\" -> \"{altWord}\"");
                        }
                    }
                    else
                    {
                        charToAdd = inputDialogues[currentIndex][currentPos];
                    }
                    
                    TMP.text += charToAdd;
                    if(!isChinese)
                        if(TMP.text[TMP.text.Length-1] != ' ') AudioManager.Instance.PlayOneShot(0, Random.Range(0.1f,0.3f));

                    typeTimer = 0;
                    UpdateCurrentCharInterval(); // 更新下一个字符的间隔
                    
                }
                
                if(isChinese)
                {
                    cnTypeSoundTimer += Time.deltaTime;
                    if(cnTypeSoundTimer > cnTypeSoundTime)
                    {
                        AudioManager.Instance.PlayOneShot(0, Random.Range(0.1f,0.3f));
                        cnTypeSoundTimer = 0;
                    }
                }
            }
            else
            {
                if(isChinese) cnTypeSoundTimer = 0;
            }     
        }

        if(unlockEnter)
        {
            if(Input.GetKeyDown(KeyCode.Return))
            {
                preDialogues[3].gameObject.SetActive(true);
                preDialogues[3].GetComponent<DialogueBoxController>().initialText = TMP.text + "\n \n     (END)";
                preDialogues[3].GetComponent<TMP_Text>().text = TMP.text + "\n \n     (END)";
                preDialogues[3].GetComponent<DialogueBoxController>().ConfigureDialogueBox();

                unlockEnter = false;

                this.transform.parent.GetChild(1).gameObject.SetActive(false);
                this.transform.parent.GetChild(this.transform.GetSiblingIndex()+1).GetComponent<TMP_Text>().text = "";
                this.transform.parent.GetChild(this.transform.GetSiblingIndex()+1).gameObject.SetActive(false);

                AudioManager.Instance.PlayOneShot(3, 0.4f);

                // SteamScript.Instance.CheckACH(0);
                // if(TMP.text == "I love you" || TMP.text == "I love you." || TMP.text == "I love you. " || TMP.text == "I love you.." || TMP.text == "I love you...")
                // {
                //     Debug.Log("I love you");
                //     SteamScript.Instance.CheckACH(1);
                // }

                TMP.text = "";
                realEnd = true;
                //SteamAchievement.UnloadAchievement("ACH_END");
            }
        }
    }

    int ChangeIndex()
    {
        int index = currentIndex;

        inputDialogues.RemoveAt(index);
        if(alternativeDialogues.Count > index)
        {
            alternativeDialogues.RemoveAt(index);
        }
        if(typingCurves.Count > index)
        {
            typingCurves.RemoveAt(index);
        }
        
        // 清空被删除单词的记录，因为切换到新对话了
        deletedWordIndices.Clear();
        ParseWordsForCurrentDialogue(); // 解析新对话的单词
        
        if(inputDialogues.Count == 0)
        {
            if(dialogueIndex == 0)
            {
                dialogueIndex = 1;
                inputDialogues.AddRange(textConfig.inputDialogues_1);
                if(textConfig.alternativeDialogues_1 != null && textConfig.alternativeDialogues_1.Count > 0)
                {
                    alternativeDialogues.AddRange(textConfig.alternativeDialogues_1);
                }
                if(textConfig.typingCurves_1 != null && textConfig.typingCurves_1.Count > 0)
                {
                    typingCurves.AddRange(textConfig.typingCurves_1);
                }
                preDialogues[0].gameObject.SetActive(true);
                waitTimer = -4;

            }
            else if(dialogueIndex == 1)
            {
                dialogueIndex = 2;
                inputDialogues.AddRange(textConfig.inputDialogues_2);
                if(textConfig.alternativeDialogues_2 != null && textConfig.alternativeDialogues_2.Count > 0)
                {
                    alternativeDialogues.AddRange(textConfig.alternativeDialogues_2);
                }
                if(textConfig.typingCurves_2 != null && textConfig.typingCurves_2.Count > 0)
                {
                    typingCurves.AddRange(textConfig.typingCurves_2);
                }
                preDialogues[1].gameObject.SetActive(true);
                waitTimer = -5;
                randomOrder = true;
            }
            else if(dialogueIndex == 2)
            {
                if(!isEnd && !unlockEnter)
                {
                    isEnd = true;
                    if(!isChinese)inputDialogues.Add("Good Night...");
                    else inputDialogues.Add("晚安...");
                    // "Good Night..." 没有对应的曲线和替代文本，将使用默认速度
                    preDialogues[2].gameObject.SetActive(true);
                    waitTimer = -8;
                    unlockEnter = true;

                    //SteamScript.Instance.CheckACH(2);

                }
                else if(isEnd && unlockEnter)
                {
                    inputDialogues.AddRange(textConfig.inputDialogues_0);
                    inputDialogues.AddRange(textConfig.inputDialogues_1);
                    inputDialogues.AddRange(textConfig.inputDialogues_2);
                    alternativeDialogues.Clear();
                    if(textConfig.alternativeDialogues_0 != null && textConfig.alternativeDialogues_0.Count > 0)
                    {
                        alternativeDialogues.AddRange(textConfig.alternativeDialogues_0);
                    }
                    if(textConfig.alternativeDialogues_1 != null && textConfig.alternativeDialogues_1.Count > 0)
                    {
                        alternativeDialogues.AddRange(textConfig.alternativeDialogues_1);
                    }
                    if(textConfig.alternativeDialogues_2 != null && textConfig.alternativeDialogues_2.Count > 0)
                    {
                        alternativeDialogues.AddRange(textConfig.alternativeDialogues_2);
                    }
                    typingCurves.Clear();
                    if(textConfig.typingCurves_0 != null && textConfig.typingCurves_0.Count > 0)
                    {
                        typingCurves.AddRange(textConfig.typingCurves_0);
                    }
                    if(textConfig.typingCurves_1 != null && textConfig.typingCurves_1.Count > 0)
                    {
                        typingCurves.AddRange(textConfig.typingCurves_1);
                    }
                    if(textConfig.typingCurves_2 != null && textConfig.typingCurves_2.Count > 0)
                    {
                        typingCurves.AddRange(textConfig.typingCurves_2);
                    }
                }
                else
                {

                }


            }

        }
        
        if(randomOrder) index = Random.Range(0,inputDialogues.Count);
        else index = 0;

        // 更新当前字符的间隔
        UpdateCurrentCharInterval();
        
        return index;
    }

    // 根据当前字符索引从曲线获取间隔时间（x轴归一化到0-1）
    void UpdateCurrentCharInterval()
    {
        if(currentIndex < typingCurves.Count && typingCurves[currentIndex] != null)
        {
            int charIndex = TMP.text.Length; // 当前要打出的字符索引
            int textLength = inputDialogues[currentIndex].Length; // 当前文本总长度
            
            // 将字符索引归一化到 0-1 范围
            float normalizedX = 0f;
            if(textLength > 1)
            {
                normalizedX = (float)charIndex / (textLength - 1);
            }
            // 如果文本长度为1或0，normalizedX保持为0
            
            currentCharInterval = typingCurves[currentIndex].Evaluate(normalizedX);
            // 确保间隔时间不为负或零
            if(currentCharInterval <= 0)
            {
                currentCharInterval = typeTime; // 使用默认值
            }
        }
        else
        {
            // 没有曲线或曲线为空，使用默认速度
            currentCharInterval = typeTime;
        }
    }


    void RemoveChar()
    {
        if(TMP.text.Length > 1)
        {
            if(TMP.text[TMP.text.Length-1] != ' ') AudioManager.Instance.PlayOneShot(1, Random.Range(0.1f,0.3f));
            TMP.text = TMP.text.Remove(TMP.text.Length-1);
            
            // 检查哪些单词被完全删除了
            CheckDeletedWords();
        }
        else if(TMP.text.Length == 1)
        {
            if(TMP.text[TMP.text.Length-1] != ' ') AudioManager.Instance.PlayOneShot(1, Random.Range(0.1f,0.3f));
            TMP.text = TMP.text.Remove(TMP.text.Length-1);
            CheckDeletedWords();
            currentIndex = ChangeIndex();
            ParseWordsForCurrentDialogue(); // 解析新对话的单词
        }
        else
        {
            Debug.Log("Complete Delete");
        }
    }

    // 解析当前对话的单词信息
    void ParseWordsForCurrentDialogue()
    {
        if(currentIndex >= inputDialogues.Count) return;
        
        string text = inputDialogues[currentIndex];
        currentWordInfoList = new List<WordInfo>();
        
        if(string.IsNullOrEmpty(text)) return;
        
        // 按空格分割单词，但保留空格位置信息
        int currentPos = 0;
        int wordIndex = 0;
        
        while(currentPos < text.Length)
        {
            // 跳过空格
            while(currentPos < text.Length && text[currentPos] == ' ')
            {
                currentPos++;
            }
            
            if(currentPos >= text.Length) break;
            
            // 找到单词的起始位置
            int wordStart = currentPos;
            
            // 找到单词的结束位置（下一个空格或文本结尾）
            while(currentPos < text.Length && text[currentPos] != ' ')
            {
                currentPos++;
            }
            
            int wordEnd = currentPos;
            
            // 创建单词信息
            WordInfo wordInfo = new WordInfo
            {
                startIndex = wordStart,
                endIndex = wordEnd,
                wordIndex = wordIndex
            };
            
            currentWordInfoList.Add(wordInfo);
            wordIndex++;
        }
        
        // 调试信息：打印解析的单词信息
        Debug.Log($"[Backspace] 开始打印对话 {currentIndex}: \"{text}\"");
        Debug.Log($"[Backspace] 解析出 {currentWordInfoList.Count} 个单词:");
        foreach(var wordInfo in currentWordInfoList)
        {
            string word = text.Substring(wordInfo.startIndex, wordInfo.endIndex - wordInfo.startIndex);
            Debug.Log($"[Backspace]   单词 {wordInfo.wordIndex}: \"{word}\" (位置 {wordInfo.startIndex}-{wordInfo.endIndex})");
        }
    }

    // 根据字符位置获取对应的单词索引
    int GetWordIndexForPosition(int position)
    {
        if(currentWordInfoList == null) return -1;
        
        foreach(var wordInfo in currentWordInfoList)
        {
            if(position >= wordInfo.startIndex && position < wordInfo.endIndex)
            {
                return wordInfo.wordIndex;
            }
        }
        
        return -1;
    }

    // 检查哪些单词被完全删除了（当前文本长度 <= 单词的起始位置）
    void CheckDeletedWords()
    {
        if(currentWordInfoList == null) return;
        
        int currentTextLength = TMP.text.Length;
        
        foreach(var wordInfo in currentWordInfoList)
        {
            // 只有当当前文本长度 <= 单词的起始位置时，说明这个单词被完全删除了
            // 因为如果文本长度 > 起始位置，说明至少单词的第一个字符还存在
            bool isFullyDeleted = currentTextLength <= wordInfo.startIndex;
            bool wasAlreadyDeleted = deletedWordIndices.Contains(wordInfo.wordIndex);
            
            if(isFullyDeleted && !wasAlreadyDeleted)
            {
                deletedWordIndices.Add(wordInfo.wordIndex);
                string word = inputDialogues[currentIndex].Substring(wordInfo.startIndex, wordInfo.endIndex - wordInfo.startIndex);
                Debug.Log($"[Backspace] 单词 \"{word}\" (索引 {wordInfo.wordIndex}, 位置 {wordInfo.startIndex}-{wordInfo.endIndex}) 被完全删除！当前文本长度: {currentTextLength}");
            }
            else if(!isFullyDeleted && wasAlreadyDeleted)
            {
                // 如果单词又被重新打出来了，从删除列表中移除（虽然这种情况不应该发生）
                deletedWordIndices.Remove(wordInfo.wordIndex);
                string word = inputDialogues[currentIndex].Substring(wordInfo.startIndex, wordInfo.endIndex - wordInfo.startIndex);
                Debug.Log($"[Backspace] 单词 \"{word}\" (索引 {wordInfo.wordIndex}) 重新出现，从删除列表中移除");
            }
        }
        
        // 打印当前被删除的单词列表
        if(deletedWordIndices.Count > 0)
        {
            string deletedWords = "";
            foreach(int idx in deletedWordIndices)
            {
                var wordInfo = currentWordInfoList.Find(w => w.wordIndex == idx);
                if(wordInfo != null)
                {
                    string word = inputDialogues[currentIndex].Substring(wordInfo.startIndex, wordInfo.endIndex - wordInfo.startIndex);
                    deletedWords += $"\"{word}\"(索引{idx}) ";
                }
            }
            Debug.Log($"[Backspace] 当前被完全删除的单词: {deletedWords}");
        }
    }
}

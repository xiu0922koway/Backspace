using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.SceneManagement;

public class Backspace : MonoBehaviour
{
    public bool isChinese;
    public float cnTypeSoundTime = 0.075f;
    private float cnTypeSoundTimer;
    public Text_Config textConfig;
    private List<string> inputDialogues;
    public TextRolling textRolling;
    private int dialogueIndex;
    private int currentIndex;
    public TMP_Text TMP;

    public List<TMP_Text> preDialogues;


    public float waitTime = 2;
    private float waitTimer = 0;
    public bool isDeleting = false;

    public float typeTime = 0.5f;
    private float typeTimer = 0;

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
        dialogueIndex = 0;
        
        TMP = GameObject.FindGameObjectWithTag("Input").GetComponent<TMP_Text>();

        currentIndex = 0;
        TMP.text = "";
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
                if(typeTimer < typeTime)
                {
                    typeTimer += Time.deltaTime;
                                  
                }
                else
                {
                    TMP.text += inputDialogues[currentIndex][TMP.text.Length];
                    if(!isChinese)
                        if(TMP.text[TMP.text.Length-1] != ' ') AudioManager.Instance.PlayOneShot(0, Random.Range(0.1f,0.3f));

                    typeTimer = 0;
                    
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
        if(inputDialogues.Count == 0)
        {
            if(dialogueIndex == 0)
            {
                dialogueIndex = 1;
                inputDialogues.AddRange(textConfig.inputDialogues_1);
                preDialogues[0].gameObject.SetActive(true);
                waitTimer = -4;

            }
            else if(dialogueIndex == 1)
            {
                dialogueIndex = 2;
                inputDialogues.AddRange(textConfig.inputDialogues_2);
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
                    preDialogues[2].gameObject.SetActive(true);
                    waitTimer = -8;
                    unlockEnter = true;
                }
                else if(isEnd && unlockEnter)
                {
                    inputDialogues.AddRange(textConfig.inputDialogues_0);
                    inputDialogues.AddRange(textConfig.inputDialogues_1);
                    inputDialogues.AddRange(textConfig.inputDialogues_2);
                }
                else
                {

                }


            }

        }
        
        if(randomOrder) index = Random.Range(0,inputDialogues.Count);
        else index = 0;

        return index;
    }


    void RemoveChar()
    {
        if(TMP.text.Length > 1)
        {
            if(TMP.text[TMP.text.Length-1] != ' ') AudioManager.Instance.PlayOneShot(1, Random.Range(0.1f,0.3f));
            TMP.text = TMP.text.Remove(TMP.text.Length-1);
        }
        else if(TMP.text.Length == 1)
        {
            if(TMP.text[TMP.text.Length-1] != ' ') AudioManager.Instance.PlayOneShot(1, Random.Range(0.1f,0.3f));
            TMP.text = TMP.text.Remove(TMP.text.Length-1);
            currentIndex = ChangeIndex();
        }
        else
        {
            Debug.Log("Complete Delete");
        }
    }
}

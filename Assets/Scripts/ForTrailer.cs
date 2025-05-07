using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ForTrailer : MonoBehaviour
{
    public List<string> texts = new List<string>();
    private int index;
    public TMP_Text TMP;

    public float waitTime = 2;
    private float waitTimer = 0;
    public bool isDeleting = false;

    public float typeTime = 0.5f;
    private float typeTimer = 0;

    public float buttonTime = 0.2f;
    public float firstButtonTime = 0.2f;
    private float buttonTimer = 0;
    private bool firstHold = false;

    public bool isChinese;
    public float cnTypeSoundTime = 0.075f;
    private float cnTypeSoundTimer;

    // Start is called before the first frame update
    void Start()
    {
        TMP = this.GetComponent<TMP_Text>();
        isDeleting = true;
        TMP.text = "";
        index = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // if(Input.GetKeyDown(KeyCode.Space))
        // {
        //     //SteamAchievement.UnloadAchievement("ACH_END");
        // }
        
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        
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
            if(TMP.text.Length < texts[index].Length)
            {
                if(typeTimer < typeTime)
                {
                    typeTimer += Time.deltaTime;
                }
                else
                {
                    TMP.text += texts[index][TMP.text.Length];
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
        }
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
            if(index < 10)
            {
                index += 1;
            }
            else
            {
                //StartCoroutine(LoadMainScene());
            }

        }
    }

    
}


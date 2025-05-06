using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetTimer : MonoBehaviour
{
    public float resetTime = 600;
    public float resetTimer = 0;
    
    public bool isLoading;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isLoading) return;
        
        if(Input.anyKey)
        {
            resetTimer = 0;
            return;
        }

        resetTimer += Time.deltaTime;

        if(resetTimer > resetTime)
        {
            isLoading = true;
            SceneManager.LoadSceneAsync(0);
        }
    }
}

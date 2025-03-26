using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public AudioClip[] audioClips;

    private void Awake()
    {
        // 确保只有一个实例存在
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 在场景切换时不销毁
        }
        else
        {
            Destroy(gameObject); // 如果已经存在，则销毁这个新实例
        }

        Application.targetFrameRate = 60;
    }

    public void PlayOneShot(int i, float volume)
    {
        this.GetComponent<AudioSource>().PlayOneShot(audioClips[i], volume);
    }
}

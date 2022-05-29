using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    public AudioClip[] audioes;
    public AudioSource backgroundAudio;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {        

    }

    // Update is called once per frame
    void Update()
    {
        if (backgroundAudio.isPlaying == false)
        {
            backgroundAudio.clip = audioes[UnityEngine.Random.RandomRange(0, 4)];
            backgroundAudio.Play();
        }

        if (GameObject.FindGameObjectsWithTag("Audio").Length > 1)
        {
            for (int i = 1; i < GameObject.FindGameObjectsWithTag("Audio").Length; i++)
            {
                GameObject.FindGameObjectsWithTag("Audio")[i].SetActive(false);
            }
        }
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using QAds;

public class LevelsManager : MonoBehaviour
{
    [Header("Put your URLS")]
    public string facebookURL;
    public string instagramURL;
    public string reviewURL;
    public string moreURL;
    [Header("Objects")]
    public Text crystals;
    public GameObject pannelSettings;
    public GameObject backSettings;
    public Image soundBtn;
    [Header("Looked levels")]
    public GameObject[] lookedLevels;
    [Header("Sprites")]
    public Sprite soundON;
    public Sprite soundOFF;
    [Header("Sounds")]
    public AudioClip click;

    private int unlookedLevels;
    private bool isSettings;
    private int soundStatus;
    private AudioSource music;

    void Start()
    {
        unlookedLevels = PlayerPrefs.GetInt("UnlookedLevels"); //Get number of levels which was unloocked till now
        soundStatus = PlayerPrefs.GetInt("sound"); //Get the sound status


    //Add 100 crystals when player open the game for the first time
        if(PlayerPrefs.GetInt("add100Crystals") == 0)
        {
            PlayerPrefs.SetInt("crystalsNum", 100);
            PlayerPrefs.SetInt("add100Crystals",1);
            PlayerPrefs.Save();
        }
    //-----------------------------------------------------

        crystals.text = "" + PlayerPrefs.GetInt("crystalsNum"); //Update number of crystals every time when this is changed

        music = gameObject.AddComponent<AudioSource>(); //Add Audio Source for playing audio clips

        if (soundStatus == 0)
        {
            soundBtn.sprite = soundON;
        }
        else
        {
            soundBtn.sprite = soundOFF;
        }

    //Unlooked the levels which was passed till now
        for (int i = 0; i < unlookedLevels; i++)
        {
            lookedLevels[i].SetActive(false);
        }

        //Show banner in the bottom
        if (SceneManager.GetActiveScene().name == "menu")
            QuickAds.instance.ShowBanner();
        else
            QuickAds.instance.RemoveBanner();
    }
    private void Update()
    {
        if (soundStatus == 0)
            music.mute = false;
        else
            music.mute = true;
    }
    public void LoadLevel(string name)
    {
        SceneManager.LoadScene(name);
    }
    public void OpenFacebook()
    {
        Application.OpenURL(facebookURL);
    }
    public void MoreBtn()
    {
        Application.OpenURL(moreURL);
    }
    public void OpenInstagram()
    {
        Application.OpenURL(instagramURL);
    }
    public void ReviewBtn()
    {
        Application.OpenURL(reviewURL);
    }
    public void Settings()
    {
        if (isSettings == false)
        {
            backSettings.SetActive(true);
            pannelSettings.SetActive(true);
            isSettings = true;
        }
        else
        {
            backSettings.SetActive(false);
            pannelSettings.SetActive(false);
            isSettings = false;
        }
    }
    public void SoundBtn()
    {
        if(soundStatus == 0)
        {
            soundBtn.sprite = soundOFF;
            PlayerPrefs.SetInt("sound",1);
            soundStatus = 1;
        }
        else
        {
            soundBtn.sprite = soundON;
            PlayerPrefs.SetInt("sound",0);
            soundStatus = 0;
        }

        PlayerPrefs.Save();
    }
    public void Click()
    {
        music.clip = click;
        music.Play();
    }
}



using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using QAds;
using GoogleMobileAds.Api;
using System.Collections;

public class LevelManager : MonoBehaviour {

    private const float disDownLevelConstruction = 0.45f;

    public static LevelManager instance { get; set; }

    [Header("Set up level propreties")]
    public int currentLevel;
    public int numberOfBrickInLevel;
    public int maxLifeBrick;
    [Header("Sprites")]
    public Sprite darkMode;
    public Sprite lightMode;
    [Header("Sounds")]
    public AudioClip click;
    public AudioClip ball;
    public AudioClip levelComplete;
    public AudioClip levelFail;
    [Header("Objects of level")]
    public GameObject levelConstruction;
    public GameObject gatherButton;
    public GameObject twoButtons;
    public GameObject pauseMenu;
    public GameObject levelFailed;
    public GameObject levelCompleted;
    public Image soundBtn;
    public Image modeButton;
    public Text scoreText;
    public Image contentProgressBar;
    [Header("Sprites")]
    public Sprite soundON;
    public Sprite soundOFF;
    private BannerView bannerView;

    [HideInInspector] public bool gatherBalls;
    [HideInInspector] public int nGatherBalls;
    [HideInInspector] public bool isDecreaseButtonPresed;
    [HideInInspector] public int score;
    [HideInInspector] public int numberOfPointsInLevel;
    [HideInInspector] public int destroyedBricks;

    private bool isNowDarkMode; //Check if dark mode is activated or not
    private bool isLevelSaved;  // check if level was saved or not
    private AudioSource music;  //This is main Audio Source which will play all sounds
    private int soundStatus;

    private void Awake()
    {
        instance = this;

        music = gameObject.AddComponent<AudioSource>();
        soundStatus = PlayerPrefs.GetInt("sound");

        Time.timeScale = 1.0f;

        if (soundStatus == 0)
        {
            soundBtn.sprite = soundON;
        }
        else
        {
            soundBtn.sprite = soundOFF;
        }
    }
    private void Start()
    {
        isNowDarkMode = true;

        GameObject.Find("top_pannel").GetComponent<SpriteRenderer>().color = new Color(42f / 255, 182f / 255, 229f / 255, 1f);
        GameObject.Find("bottom_pannel").GetComponent<SpriteRenderer>().color = new Color(42f / 255, 182f / 255, 229f / 255, 1f);
        //GameObject.Find("middle_pannel").GetComponent<SpriteRenderer>().color = new Color(0f / 255, 0f / 255, 0f / 255, 1f);

        Balls.instance.ball.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
        Balls.instance.tnArrivedBallsBack.color = new Color(1f, 1f, 1f, 1f);

        if(QuickAds.instance != null)
          QuickAds.instance.RemoveBanner();

        MobileAds.Initialize(initStatus => { });
    }
    private void Update()
    {
        scoreText.text = "" + score;
        contentProgressBar.fillAmount = (float)score / (float)numberOfPointsInLevel;

        if (soundStatus == 0)
        {
            music.mute = false;
            GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioSource>().enabled = true;
        }
        else
        {
            music.mute = true;
            GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioSource>().enabled = false;
        }            

        //When level is completed
        if (numberOfBrickInLevel <= destroyedBricks) //Check if number of destroyed bricks is not equal with number of bricks in level
        {
            levelCompleted.SetActive(true);

            if (isLevelSaved == false)
            {
                PlayerPrefs.SetInt("crystalsNum", PlayerPrefs.GetInt("crystalsNum")+50);
                PlayerPrefs.Save();

                if (QuickAds.instance != null)
                {
                    QuickAds.instance.ShowInterstitialAd();
                    QuickAds.instance.ShowRewardedAd();
                }

                this.RequestBanner();


                if (currentLevel > PlayerPrefs.GetInt("UnlookedLevels")) //Chekc if the level is not already saved
                {
                    PlayerPrefs.SetInt("UnlookedLevels", PlayerPrefs.GetInt("UnlookedLevels") + 1);
                    PlayerPrefs.Save();
                }

                LevelCompleteSound();

                isLevelSaved = true;
            }

            GameObject.Find("complete_text").GetComponent<Text>().text = "STAGE   " + currentLevel;

            Time.timeScale = 0.0f;
        }
        //-------------------------------------------------------------------

        //When we pressed Gather Button,to gather all the balls
        if (nGatherBalls >= Balls.instance.ballNumberThrown && gatherBalls)
        {
            gatherBalls = false; //It means that all balls was already gathered
            nGatherBalls = 0;  //Initialize the number of gathered balls

            gatherButton.SetActive(false);
            twoButtons.SetActive(true); //Set actve buttons which can increase number of balls and another which can decrease life of all balls

            Balls.instance.tnArrivedBallsBack.text = "x" + Balls.instance.numberBalls;
            Balls.instance.startThrowBalls = false;
            Balls.instance.nArrivedBallsBack = Balls.instance.numberBalls;

            DownLevelConstruction();
            Balls.instance.levelConstructionDown = true;
        }
    //------------------------------------------------------------------------

    //Decrease all balls life with -5 when derease button was pressed
        if (isDecreaseButtonPresed)
        {
            GameObject[] bricks = GameObject.FindGameObjectsWithTag("brick");

            foreach (GameObject obj in bricks)
            {
                obj.GetComponent<Brick>().brickCollission.sortingOrder = 5;
                obj.GetComponent<Brick>().brickLife -= 5;

                LevelManager.instance.score += 5;
            }

            isDecreaseButtonPresed = false;
        }
    //-----------------------------------------------------------------------------

    //Check if you have enough crystals to duplicate number of balls or decrease life of all bricks
        if(PlayerPrefs.GetInt("crystalsNum") >= 100)
        {
            if(GameObject.Find("duplicate_number_of_balls_button") != null)
               GameObject.Find("duplicate_number_of_balls_button").GetComponent<Button>().interactable = true;
            if(GameObject.Find("decrease_bircks_life_brick") != null)
               GameObject.Find("decrease_bircks_life_brick").GetComponent<Button>().interactable = true;
        }
        else
        {
            if (GameObject.Find("duplicate_number_of_balls_button") != null)
            {
                GameObject.Find("duplicate_number_of_balls_button").GetComponent<Button>().interactable = false;
                GameObject.Find("decrease_bircks_life_brick").GetComponent<Button>().interactable = false;
            }
        }
    //----------------------------------------------------------------------------------------------------
    }

    private void RequestBanner()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-4043205548312565/8506519715";
#elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-4043205548312565/3800987946";
#else
            string adUnitId = "unexpected_platform";
#endif

        // Create a 320x50 banner at the top of the screen.
        this.bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Top);

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the banner with the request.
        this.bannerView.LoadAd(request);
    }

    public void DownLevelConstruction()
    {
        levelConstruction.transform.position = new Vector3(levelConstruction.transform.position.x, levelConstruction.transform.position.y - disDownLevelConstruction);
    }
    public void GatherBallsButton()
    {
        gatherBalls = true;
        nGatherBalls += Balls.instance.nArrivedBallsBack;
    }
    public void DecreaseButtonPressed()
    {
        Balls.instance.nArrivedBallsBack = Balls.instance.numberBalls;
        isDecreaseButtonPresed = true;


        PlayerPrefs.SetInt("crystalsNum", PlayerPrefs.GetInt("crystalsNum") - 100);
        PlayerPrefs.Save();
    }
    public void IncreaseNumberOfBalls()
    {
        Balls.instance.numberBalls += 5;
        Balls.instance.tnArrivedBallsBack.text = "x" + Balls.instance.numberBalls;
        Balls.instance.nArrivedBallsBack = Balls.instance.numberBalls;

        PlayerPrefs.SetInt("crystalsNum", PlayerPrefs.GetInt("crystalsNum") - 100);
        PlayerPrefs.Save();
    }
    public void ChangeLevelMode()
    {
        GameObject[] balls = GameObject.FindGameObjectsWithTag("ball");

        if (isNowDarkMode)
        {
            GameObject.Find("top_pannel").GetComponent<SpriteRenderer>().color = new Color(104f / 255f, 156f / 255f, 135f / 255f, 1f);
            GameObject.Find("bottom_pannel").GetComponent<SpriteRenderer>().color = new Color(104f / 255f, 156f / 255f, 135f / 255f, 1f);
            GameObject.Find("middle_pannel").GetComponent<SpriteRenderer>().color = new Color(236f / 255f, 231f / 255f, 231f / 255f, 1f);

            Balls.instance.ball.GetComponent<SpriteRenderer>().color = new Color(48f / 255f, 40f / 255f, 40f / 255f, 1f);
            Balls.instance.tnArrivedBallsBack.GetComponent<Text>().color = new Color(0, 0f, 0f, 1f);

            foreach (GameObject obj in balls)
                obj.GetComponent<SpriteRenderer>().color = new Color(48f / 255f, 40f / 255f, 40f / 255f, 1f);

            modeButton.sprite = lightMode;
            isNowDarkMode = false;
        }
        else
        {
            GameObject.Find("top_pannel").GetComponent<SpriteRenderer>().color = new Color(42f / 255, 182f / 255, 229f / 255, 1f);
            GameObject.Find("bottom_pannel").GetComponent<SpriteRenderer>().color = new Color(42f / 255, 182f / 255, 229f / 255, 1f);
            GameObject.Find("middle_pannel").GetComponent<SpriteRenderer>().color = new Color(0f / 255, 0f / 255, 0f / 255, 1f);

            Balls.instance.ball.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
            Balls.instance.tnArrivedBallsBack.color = new Color(1f, 1f, 1f, 1f);

            foreach (GameObject obj in balls)
                obj.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);

            modeButton.sprite = darkMode;
            isNowDarkMode = true;
        }
    }
    public void Pause()
    {
        pauseMenu.SetActive(true);
        levelConstruction.SetActive(false);

        Time.timeScale = 0f;
    }
    public void UnPause()
    {
        pauseMenu.SetActive(false);
        levelConstruction.SetActive(true);

        Time.timeScale = 1f;
    }
    public void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }
    public void NextLevel()
    {
        int nextLevel = currentLevel + 1;
        string name = string.Concat("level_",nextLevel.ToString());

        SceneManager.LoadScene(name);
    }
    public void Click()
    {        
        music.clip = click;
        music.Play();
    }
    public void BallTouchedBrickSound()
    {
        music.clip = ball;
        music.Play();
    }
    public void LevelCompleteSound()
    {
        music.clip = levelComplete;
        music.Play();
    }
    public void LevelFailSound()
    {
        music.clip = levelFail;
        music.Play();
    }
    public void SoundBtn()
    {
        if (soundStatus == 0)
        {
            soundBtn.sprite = soundOFF;
            PlayerPrefs.SetInt("sound", 1);
            soundStatus = 1;
        }
        else
        {
            soundBtn.sprite = soundON;
            PlayerPrefs.SetInt("sound", 0);
            soundStatus = 0;
        }

        PlayerPrefs.Save();
    }
}

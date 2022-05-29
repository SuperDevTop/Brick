using UnityEngine;
using UnityEngine.UI;
using QAds;

public class Brick : MonoBehaviour
{
    public int brickLife;
    public Text text;
    public SpriteRenderer brickCollission;

    private float timer;
    private bool check;
    private SpriteRenderer sp;
    public int initialColorG = 70;
    public int stepColor;
    private int maxLifeBrick;

    private Vector2 posText;
    private bool isLevelFail;

    private void Start()
    {
        maxLifeBrick = LevelManager.instance.maxLifeBrick;
        brickLife = (int)Random.Range(maxLifeBrick/2, maxLifeBrick);

        LevelManager.instance.numberOfPointsInLevel += brickLife;

        posText = Camera.main.WorldToScreenPoint(this.gameObject.transform.position);
        text.gameObject.GetComponent<RectTransform>().position = new Vector3(posText.x, posText.y);

        brickCollission = brickCollission.gameObject.GetComponent<SpriteRenderer>();
        sp = GetComponent<SpriteRenderer>();

        //Set up color of brick in dependence of brik life,if it will have more life than brik will be darker else more lighter will be
        stepColor = (int)(150 / maxLifeBrick);
        initialColorG = 70 + stepColor*(maxLifeBrick - brickLife);
        sp.color = ColorLevel();
    }
    private void FixedUpdate()
    {
        text.text = "" + brickLife;

    //When a brick will have position less -2.0f than the player will lose
        if (transform.position.y < -3.0f)
        {
            LevelManager.instance.levelFailed.SetActive(true);
            LevelManager.instance.levelConstruction.SetActive(false);

            if(isLevelFail == false)
            {
                LevelManager.instance.LevelFailSound();
                QuickAds.instance.ShowInterstitialAd();
                isLevelFail = true;
            }

            GameObject.Find("fail_text").GetComponent<Text>().text = "STAGE   " + LevelManager.instance.currentLevel;

            Time.timeScale = 0f;
        }
        //-------------------------------------------------------------------------------
        if (brickLife <= 0)
        {
            LevelManager.instance.destroyedBricks++;
            Destroy(this.gameObject);
        }
        if(brickCollission.sortingOrder == 5 && !check)
        {
            timer = Time.time + 0.1f;
            check = true;
        }
        if (timer < Time.time && check)
        {
            brickCollission.sortingOrder = 0;
            sp.color = ColorLevel();
            check = false;
        }
        if(Balls.instance.levelConstructionDown)
        {
            posText = Camera.main.WorldToScreenPoint(this.gameObject.transform.position);
            text.gameObject.GetComponent<RectTransform>().position = new Vector3(posText.x, posText.y);
        }
    }
    private Color ColorLevel()
    {
        Color colorLevel = new Color(sp.color.r,(float)initialColorG / 255f,sp.color.b,sp.color.a);

        return colorLevel;
    }
}

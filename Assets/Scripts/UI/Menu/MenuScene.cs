using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoogleMobileAds.Api;
using System;

public class MenuScene : MonoBehaviour
{
    public GameObject[] levels;
    public Image forwardButton;
    public Image backwardButton;
    public Sprite[] buttonSprites;
    private BannerView bannerView;

    int levelCount = 0;
    // Start is called before the first frame update

   

    void Start()
    {
        MobileAds.Initialize(initStatus => { });
        this.RequestBanner();

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

    public void ForwardClick()
    {
        levelCount++;

        if (Mathf.Abs( levelCount) < levels.Length)
        {            
            for (int i = 0; i < levels.Length; i++)
            {
                levels[i].SetActive(false);
            }

            levels[levelCount].SetActive(true);
            backwardButton.sprite = buttonSprites[1];
            forwardButton.sprite = buttonSprites[1];
        }

        if (levelCount == levels.Length - 1)
        {
            forwardButton.sprite = buttonSprites[0];
            backwardButton.sprite = buttonSprites[1];
        }       
    }

    public void Backwardclick()
    {
        if (levelCount == 0)
        {
            backwardButton.sprite = buttonSprites[0];
            forwardButton.sprite = buttonSprites[1];
        }
        else if (Mathf.Abs(levelCount) < levels.Length - 1)
        {
            levelCount--;

            for (int i = 0; i < levels.Length; i++)
            {
                levels[i].SetActive(false);
            }
            
            backwardButton.sprite = buttonSprites[1];

            if (levelCount < 0)
            {
                forwardButton.sprite = buttonSprites[0];
                levels[levels.Length + levelCount].SetActive(true);
            }
            else
            {
                forwardButton.sprite = buttonSprites[1];
                levels[levelCount].SetActive(true);
            }
        }
    }
}

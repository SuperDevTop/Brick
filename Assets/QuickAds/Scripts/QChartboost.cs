using UnityEngine;
using System;
#if QUICK_ADS_CHARTBOOST
using ChartboostSDK;
#endif

namespace QAds
{
    public class QChartboost : MonoBehaviour
#if QUICK_ADS_CHARTBOOST
    , IQInterstitial, IQRewarded, IQAdService
#endif
    {

#if QUICK_ADS_CHARTBOOST

    bool interstitialCallbacksLoaded = false;
    bool rewardedCallbacksLoaded = false;

    public void InitializeAdService()
    {
#if UNITY_EDITOR
        Debug.Log("Chartboost ads can not be created in the edtior");
#endif
        gameObject.AddComponent<Chartboost>();

        CBSettings chartboostSetttings = ScriptableObject.CreateInstance<CBSettings>();

        chartboostSetttings.androidAppId = QuickAds.instance.ChartboostAndroidId;
        chartboostSetttings.SetAndroidAppId(QuickAds.instance.ChartboostAndroidId);
        chartboostSetttings.androidAppSecret = QuickAds.instance.ChartboostAndroidSign;
        chartboostSetttings.SetAndroidAppSecret(QuickAds.instance.ChartboostAndroidSign);

        chartboostSetttings.iOSAppId = QuickAds.instance.ChartboostIOSId;
        chartboostSetttings.SetIOSAppId(QuickAds.instance.ChartboostIOSId);
        chartboostSetttings.iOSAppSecret = QuickAds.instance.ChartboostIOSSign;
        chartboostSetttings.SetIOSAppSecret(QuickAds.instance.ChartboostIOSSign);

        chartboostSetttings.amazonAppId = QuickAds.instance.ChartboostAmazonId;
        chartboostSetttings.SetAmazonAppId(QuickAds.instance.ChartboostAmazonId);
        chartboostSetttings.amazonAppSecret = QuickAds.instance.ChartboostAmazonSign;
        chartboostSetttings.SetAmazonAppSecret(QuickAds.instance.ChartboostAmazonSign);

        QuickAds.instance.ReportOnScreen("Chartboost initializing");
    }
       
    public void RequestInterstitial()
    {
        QuickAds.instance.ReportOnScreen("Request for interstitial ad from Chartboost");
        LoadInterstitialCallbacks();
        Chartboost.cacheInterstitial(CBLocation.Default);        
    }

    public void LoadInterstitialCallbacks()         /////////////// LOAD INTERSTITIAL CALLBACKS ///////////////
    {
        if (!interstitialCallbacksLoaded)
        {
            Chartboost.didCacheInterstitial += didCacheInterstitial;
            Chartboost.didFailToLoadInterstitial += didFailToLoadInterstitial;
            Chartboost.didDisplayInterstitial += didDisplayInterstitial;
            Chartboost.didCloseInterstitial += didCloseInterstitial;

            interstitialCallbacksLoaded = true;
        }
    }
    void didCacheInterstitial(CBLocation location)
    {
        QuickAds.instance.ReportOnScreen("Chartboost interstitial ad has been loaded");
    }
    void didFailToLoadInterstitial(CBLocation location, CBImpressionError error)
    {
        QuickAds.instance.ReportOnScreen("Chartboost interstitial ad fail to load");
    }
    void didDisplayInterstitial(CBLocation location)
    {

    }
    void didCloseInterstitial(CBLocation location)
    {
        RequestInterstitial();
    }

    public void LoadRewardedCallbacks()         /////////////// LOAD REWARDED CALLBACKS ///////////////
    {
        if (!rewardedCallbacksLoaded)
        {
            Chartboost.didCacheRewardedVideo += didCacheRewardedVideo;
            Chartboost.didFailToLoadRewardedVideo += didFailToLoadRewardedVideo;
            Chartboost.didCloseRewardedVideo += didCloseRewardedVideo;
            Chartboost.didCompleteRewardedVideo += didCompleteRewardedVideo;

            rewardedCallbacksLoaded = true;
        }
    }
    void didCacheRewardedVideo(CBLocation location)
    {
        QuickAds.instance.ReportOnScreen("Chartboost rewarded ad has been loaded");
    }
    void didFailToLoadRewardedVideo(CBLocation location, CBImpressionError error)
    {
        QuickAds.instance.ReportOnScreen("Chartboost rewarded ad fail to load");
    }
    // Called after a rewarded video has been closed.
    void didCloseRewardedVideo(CBLocation location)
    {
        RequestRewardedAd();
    }
    // Called after a rewarded video has been viewed completely and user is eligible for reward.
    void didCompleteRewardedVideo(CBLocation location, int reward)
    {
        QuickAds.instance.ReportOnScreen("The player has fully watched the Chartboost rewarded video");
        QuickAds.instance.rewardedVideoIsShown.Invoke();
        QuickAds.instance.ReportOnScreen("Rewarding event is activating");
    }
    
    public void ShowInterstitial(Action<bool> success)
    {
        if (Chartboost.hasInterstitial(CBLocation.Default))
        {
            Chartboost.showInterstitial(CBLocation.Default);
            success(true);
        }
        else
        {
            RequestInterstitial();
            success(false);
        }
    }

    public void RequestRewardedAd()
    {
        QuickAds.instance.ReportOnScreen("Request for rewarded ad from Chartbooost");
        Chartboost.cacheRewardedVideo(CBLocation.Default);
        LoadRewardedCallbacks();
    }

    public void ShowRewardedAd(Action<bool> success)
    {
        if (Chartboost.hasRewardedVideo(CBLocation.Default))
        {
            Chartboost.showRewardedVideo(CBLocation.Default);
            success(true);
        }
        else
        {
            RequestRewardedAd();
            success(false);
        }
    }

    void DisableAllCallbacks()
    {
        DisableInterstitialCallbacks();
        DisableRewardedCallbacks();
    }

    public void DisableInterstitialCallbacks()
    {
        if (interstitialCallbacksLoaded)
        {
            Chartboost.didCacheInterstitial -= didCacheInterstitial;
            Chartboost.didFailToLoadInterstitial -= didFailToLoadInterstitial;
            Chartboost.didDisplayInterstitial -= didDisplayInterstitial;
            Chartboost.didCloseInterstitial -= didCloseInterstitial;
        }
    }

    public void DisableRewardedCallbacks()
    {
        if (rewardedCallbacksLoaded)
        {
            Chartboost.didCacheRewardedVideo -= didCacheRewardedVideo;
            Chartboost.didFailToLoadRewardedVideo -= didFailToLoadRewardedVideo;
            Chartboost.didCloseRewardedVideo -= didCloseRewardedVideo;
            Chartboost.didCompleteRewardedVideo -= didCompleteRewardedVideo;
        }
    }

    private void OnDisable()
    {
        DisableAllCallbacks();
    }

#endif
    }
}

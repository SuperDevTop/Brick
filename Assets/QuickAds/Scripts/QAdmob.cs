using System;
using UnityEngine;
#if QUICK_ADS_ADMOB
using GoogleMobileAds.Api;
#endif

namespace QAds
{
    public enum AdmobBannerPositions
    {
        Top = 0,
        Bottom = 1,
        TopLeft = 2,
        TopRight = 3,
        BottomLeft = 4,
        BottomRight = 5,
        Center = 6
    }
    public enum AdmobBannerSize
    {
        Smart_Banner,
        Standart_Banner,
        Tablet_FullSize,
        Tablet_Leader_Board,
        Rectangle
    }

    public class QAdmob : MonoBehaviour
#if QUICK_ADS_ADMOB
    , IQBanners, IQInterstitial, IQRewarded, IQAdService
#endif
    {

#if QUICK_ADS_ADMOB

        BannerView bannerView;
        InterstitialAd interstitial;
        RewardBasedVideoAd rewardBasedVideo;

        bool bannerCallbacksLoaded = false;
        bool interstitialCallbacksLoaded = false;
        bool rewardedCallbacksLoaded = false;

        public void InitializeAdService()
        {
#if UNITY_EDITOR
            Debug.Log("Admob ads can not be created in the edtior");
#endif

#if UNITY_ANDROID
            string appId = QuickAds.instance.AdmobAndroidAppId;
#elif UNITY_IPHONE
        string appId = QuickAds.instance.AdmobIOSAppId;
#else
        Debug.LogError("Quick Ads. Unexpected platform. Switch to Android or IOS platform");
#endif
#if QUICK_ADS_ADMOB
            // Initialize the Google Mobile Ads SDK.
            MobileAds.Initialize(appId);
            if (appId == "" || appId == null)
                //Debug.LogError("Quick ads. Attention! An Admob application ID must be filled.");
            QuickAds.instance.ReportOnScreen("Admob initialization");
#endif

            // Get singleton reward based video ad reference.
            rewardBasedVideo = RewardBasedVideoAd.Instance;
        }

        /////////////////// BANNERS ///////////////////
        public void ShowBanner(Action<bool> success)
        {
#if UNITY_ANDROID
            string adUnitId = QuickAds.instance.testMode ? QuickAds.instance.AdmobAndroidBannerIdTEST : QuickAds.instance.AdmobAndroidBannerId;
#elif UNITY_IPHONE
        string adUnitId = QuickAds.instance.testMode ? QuickAds.instance.AdmobIOSBannerIdTEST : QuickAds.instance.AdmobIOSBannerId;
#else
        Debug.LogError("Quick Ads. Unexpected platform. Switch to Android or IOS platform");
#endif
            RemoveBanner();
            bannerView = new BannerView(adUnitId, SetBannerSize(), SetBannerPosition());
            LoadBannerCallbacks();
            // Create an empty ad request.
            AdRequest request = new AdRequest.Builder().Build();

            // Load the banner with the request.
            bannerView.LoadAd(request);
            success(bannerView != null);
        }

        public void RemoveBanner()
        {
            if (bannerView != null)
            {
                bannerView.Destroy();
                QuickAds.instance.ReportOnScreen("Admob banner has been removed");
            }
        }

        public void LoadBannerCallbacks()
        {
            if (!bannerCallbacksLoaded)
            {

                // Called when an ad request has successfully loaded.
                bannerView.OnAdLoaded += HandleOnAdLoadedBanner;

                // Called when an ad request failed to load.
                bannerView.OnAdFailedToLoad += HandleOnAdFailedToLoadBanner;

                // Called when an ad is clicked.
                //bannerView.OnAdOpening += HandleOnAdOpenedBanner;

                // Called when the user returned from the app after an ad click.
                //bannerView.OnAdClosed += HandleOnAdClosedBanner;

                // Called when the ad click caused the user to leave the application.
                //bannerView.OnAdLeavingApplication += HandleOnAdLeavingApplicationBanner;

                bannerCallbacksLoaded = true;
            }

        }
        void HandleOnAdLoadedBanner(object sender, EventArgs args)
        {
            QuickAds.instance.ReportOnScreen("Attempt to show Admob banner was successful");
        }
        void HandleOnAdFailedToLoadBanner(object sender, AdFailedToLoadEventArgs args)
        {
            QuickAds.instance.ReportOnScreen("Attempt to show Admob banner failed. " + args.Message);
            InitializeAdService();
        }

        //void HandleOnAdOpenedBanner(object sender, EventArgs args)
        //{
        //    MonoBehaviour.print("HandleAdOpened event received");
        //}

        //void HandleOnAdClosedBanner(object sender, EventArgs args)
        //{
        //    MonoBehaviour.print("HandleAdClosed event received");
        //}

        //void HandleOnAdLeavingApplicationBanner(object sender, EventArgs args)
        //{
        //    MonoBehaviour.print("HandleAdLeftApplication event received");
        //}

        AdPosition SetBannerPosition()
        {
            switch (QuickAds.instance.admobBannerPosition)
            {
                case AdmobBannerPositions.Bottom:
                    return AdPosition.Bottom;
                case AdmobBannerPositions.BottomLeft:
                    return AdPosition.BottomLeft;
                case AdmobBannerPositions.BottomRight:
                    return AdPosition.BottomRight;
                case AdmobBannerPositions.Center:
                    return AdPosition.Center;
                case AdmobBannerPositions.Top:
                    return AdPosition.Top;
                case AdmobBannerPositions.TopLeft:
                    return AdPosition.TopLeft;
                case AdmobBannerPositions.TopRight:
                    return AdPosition.TopRight;
                default:
                    return AdPosition.Bottom;
            }
        }
        AdSize SetBannerSize()
        {
            switch (QuickAds.instance.admobBannerSize)
            {
                case AdmobBannerSize.Smart_Banner:
                    return AdSize.SmartBanner;
                case AdmobBannerSize.Standart_Banner:
                    return AdSize.Banner;
                case AdmobBannerSize.Rectangle:
                    return AdSize.MediumRectangle;
                case AdmobBannerSize.Tablet_FullSize:
                    return AdSize.IABBanner;
                case AdmobBannerSize.Tablet_Leader_Board:
                    return AdSize.Leaderboard;
                default:
                    return AdSize.SmartBanner;
            }
        }

        /////////////////// INTERSTITIAL //////////////////
        public void RequestInterstitial()
        {
#if UNITY_ANDROID
            string adUnitId = QuickAds.instance.testMode ? QuickAds.instance.AdmobAndroidInterstitialIdTEST : QuickAds.instance.AdmobAndroidInterstitialId;
#elif UNITY_IPHONE
        string adUnitId = QuickAds.instance.testMode ? QuickAds.instance.AdmobIOSInterstitialIdTEST : QuickAds.instance.AdmobIOSInterstitialId;
#else
        Debug.LogError("Quick Ads. Unexpected platform. Switch to Android or IOS platform");
#endif

            QuickAds.instance.ReportOnScreen("Request for interstitial ad from Admob");
            // Initialize an InterstitialAd.
            interstitial = new InterstitialAd(adUnitId);
            LoadInterstitialCallbacks();
            // Create an empty ad request.
            AdRequest request = new AdRequest.Builder().Build();
            // Load the interstitial with the request.
            interstitial.LoadAd(request);
            if (adUnitId == "" || adUnitId == null)
            {

            }
        }
        public void ShowInterstitial(Action<bool> success)
        {
            if (interstitial.IsLoaded())
            {
                interstitial.Show();
                success(true);
                RequestInterstitial();
            }
            else
            {
                success(false);
                InitializeAdService();
                RequestInterstitial();
            }
        }

        public void LoadInterstitialCallbacks() 
        {
            if (!interstitialCallbacksLoaded)
            {

                // Called when an ad request has successfully loaded.
                interstitial.OnAdLoaded += HandleOnAdLoadedInterstitial;
                // Called when an ad request failed to load.
                interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoadInterstitial;
                // Called when an ad is shown.
                //interstitial.OnAdOpening += HandleOnAdOpenedInterstitial;
                // Called when the ad is closed.
                //interstitial.OnAdClosed += HandleOnAdClosedInterstitial;
                // Called when the ad click caused the user to leave the application.
                //interstitial.OnAdLeavingApplication += HandleOnAdLeavingApplicationInterstitial;
                interstitialCallbacksLoaded = true;
            }
        }
        void HandleOnAdLoadedInterstitial(object sender, EventArgs args)
        {
            QuickAds.instance.ReportOnScreen("Admob interstitial ad has been loaded");
        }
        void HandleOnAdFailedToLoadInterstitial(object sender, AdFailedToLoadEventArgs args)
        {
            QuickAds.instance.ReportOnScreen("Admob interstitial ad failed to load. " + args.Message);
        }
        //void HandleOnAdOpenedInterstitial(object sender, EventArgs args)
        //{
        //    QuickAds.instance.ReportOnScreen("Admob interstitial ad request has loaded");
        //}
        //void HandleOnAdClosedInterstitial(object sender, EventArgs args)
        //{
        //    MonoBehaviour.print("HandleAdClosed event received");
        //}
        //void HandleOnAdLeavingApplicationInterstitial(object sender, EventArgs args)
        //{
        //    MonoBehaviour.print("HandleAdLeftApplication event received");
        //}

        /////////////////// REWARDED //////////////////

        public void RequestRewardedAd()
        {
            QuickAds.instance.ReportOnScreen("Request for rewarded ad from Admob");

            LoadRewardedCallbacks();

#if UNITY_ANDROID
            string adUnitId = QuickAds.instance.testMode ? QuickAds.instance.AdmobAndroidRewardedIdTEST : QuickAds.instance.AdmobAndroidRewardedId;
#elif UNITY_IPHONE
        string adUnitId = QuickAds.instance.testMode ? QuickAds.instance.AdmobIOSRewardedIdTEST : QuickAds.instance.AdmobIOSRewardedId;
#else
        Debug.LogError("Quick Ads. Unexpected platform. Switch to Android or IOS platform");
#endif

            // Create an empty ad request.
            AdRequest request = new AdRequest.Builder().Build();
            // Load the rewarded video ad with the request.
            rewardBasedVideo.LoadAd(request, adUnitId);

        }
        public void ShowRewardedAd(Action<bool> success)
        {
            if (rewardBasedVideo.IsLoaded())
            {
                rewardBasedVideo.Show();
                success(true);
                //RequestRewardedAd();
            }
            else
            {
                success(false);
                InitializeAdService();
                RequestRewardedAd();
            }

        }

        public void LoadRewardedCallbacks()       
        {
            if (!rewardedCallbacksLoaded)
            {

                // Called when an ad request has successfully loaded.
                rewardBasedVideo.OnAdLoaded += HandleRewardBasedVideoLoadedRewarded;

                // Called when an ad request failed to load.
                rewardBasedVideo.OnAdFailedToLoad += HandleRewardBasedVideoFailedToLoadRewarded;

                // Called when an ad is shown.
                //rewardBasedVideo.OnAdOpening += HandleRewardBasedVideoOpenedRewarded;

                // Called when the ad starts to play.
                //rewardBasedVideo.OnAdStarted += HandleRewardBasedVideoStartedRewarded;

                // Called when the user should be rewarded for watching a video.
                rewardBasedVideo.OnAdRewarded += HandleRewardBasedVideoRewardedRewarded;

                // Called when the ad is closed.
                rewardBasedVideo.OnAdClosed += HandleRewardBasedVideoClosedRewarded;

                // Called when the ad click caused the user to leave the application.
                //rewardBasedVideo.OnAdLeavingApplication += HandleRewardBasedVideoLeftApplicationRewarded;

                rewardedCallbacksLoaded = true;
            }
        }
        void HandleRewardBasedVideoLoadedRewarded(object sender, EventArgs args)
        {
            QuickAds.instance.ReportOnScreen("Admob rewarded ad has been loaded");
        }
        void HandleRewardBasedVideoFailedToLoadRewarded(object sender, AdFailedToLoadEventArgs args)
        {
            QuickAds.instance.ReportOnScreen("Admob rewarded ad request failed to load. " + args.Message);
        }
        //void HandleRewardBasedVideoOpenedRewarded(object sender, EventArgs args)
        //{
        //    MonoBehaviour.print("HandleRewardBasedVideoOpened event received");
        //}
        //void HandleRewardBasedVideoStartedRewarded(object sender, EventArgs args)
        //{
        //   MonoBehaviour.print("HandleRewardBasedVideoStarted event received");
        //}
        void HandleRewardBasedVideoClosedRewarded(object sender, EventArgs args)
        {
            RequestRewardedAd();
        }
        void HandleRewardBasedVideoRewardedRewarded(object sender, Reward args)
        {
            string type = args.Type;
            double amount = args.Amount;
            QuickAds.instance.ReportOnScreen("The player has fully watched the Admob rewarded video");
            QuickAds.instance.rewardedVideoIsShown.Invoke();
            QuickAds.instance.ReportOnScreen("Rewarding event is activating");
        }
        //void HandleRewardBasedVideoLeftApplicationRewarded(object sender, EventArgs args)
        //{
        //   MonoBehaviour.print("HandleRewardBasedVideoLeftApplication event received");
        //}


        void DisableAllCallbacks()
        {
            DisableBannerCallbacks();
            DisableInterstitialCallbacks();
            DisableRewardedCallbacks();
        }
        public void DisableBannerCallbacks()
        {
            if (bannerCallbacksLoaded)
            {
                bannerView.OnAdLoaded -= HandleOnAdLoadedBanner;
                bannerView.OnAdFailedToLoad -= HandleOnAdFailedToLoadBanner;
            }
        }
        public void DisableInterstitialCallbacks()
        {
            if (interstitialCallbacksLoaded)
            {
                interstitial.OnAdLoaded -= HandleOnAdLoadedInterstitial;
                interstitial.OnAdFailedToLoad -= HandleOnAdFailedToLoadInterstitial;
            }
        }
        public void DisableRewardedCallbacks()
        {
            if (rewardedCallbacksLoaded)
            {
                rewardBasedVideo.OnAdLoaded -= HandleRewardBasedVideoLoadedRewarded;
                rewardBasedVideo.OnAdFailedToLoad -= HandleRewardBasedVideoFailedToLoadRewarded;
                rewardBasedVideo.OnAdRewarded -= HandleRewardBasedVideoRewardedRewarded;
            }
        }

        private void OnDisable()
        {
            DisableAllCallbacks();
        }
#endif
    }
}

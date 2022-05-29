using UnityEngine;
using System;
#if QUICK_ADS_UNITY
using UnityEngine.Advertisements;
#endif

namespace QAds
{
    public class QUnity : MonoBehaviour
#if QUICK_ADS_UNITY
    , IQAdService, IQInterstitial, IQRewarded
#endif

    {
#if QUICK_ADS_UNITY
        public void InitializeAdService()
        {
            QuickAds.instance.ReportOnScreen("Unity initialization");
#if UNITY_ANDROID
            Advertisement.Initialize(QuickAds.instance.unityAndroidId);
            if (string.IsNullOrEmpty(QuickAds.instance.unityAndroidId))
                Debug.LogError("Attention. Unity App Android ID is not fiiled.");
#elif UNITY_IOS
        Advertisement.Initialize(QuickAds.instance.unityIOSId);
            if (string.IsNullOrEmpty(QuickAds.instance.unityIOSId))
                Debug.LogError("Attention. Unity App IOS ID is not fiiled.");
#endif
        }
        
        public void LoadInterstitialCallbacks() { }
        public void DisableInterstitialCallbacks() { }
        public void LoadRewardedCallbacks() { }
        public void DisableRewardedCallbacks() { }

        public void RequestInterstitial()
        {
            //THE METHOD IS EMPTY
        }

        public void ShowInterstitial(Action<bool> success)
        {
            if (Advertisement.IsReady(QuickAds.instance.unityInterstitialVideoId))
            {
                var options = new ShowOptions { resultCallback = HandleShowResultInterstitial };
                Advertisement.Show(QuickAds.instance.unityInterstitialVideoId, options);
                success(true);
            }
            else
            {
                success(false);
            }
        }

        public void RequestRewardedAd()
        {
            //THE METHOD IS EMPTY
        }

        public void ShowRewardedAd(Action<bool> success)
        {
            if (Advertisement.IsReady(QuickAds.instance.unityRewardedVideoId))
            {
                var options = new ShowOptions { resultCallback = HandleShowResultRewarded };
                Advertisement.Show(QuickAds.instance.unityRewardedVideoId, options);
                success(true);
            }
            else
            {
                success(false);
            }
        }

        void HandleShowResultInterstitial(ShowResult result)
        {
            switch (result)
            {
                case ShowResult.Finished:
                    break;
                case ShowResult.Skipped:
                    break;
                case ShowResult.Failed:
                    break;
            }
        }

        void HandleShowResultRewarded(ShowResult result)
        {
            switch (result)
            {
                case ShowResult.Finished:
                    QuickAds.instance.ReportOnScreen("The player has fully watched the Unity rewarded video");
                    QuickAds.instance.rewardedVideoIsShown.Invoke();
                    QuickAds.instance.ReportOnScreen("Rewarding event is activating");
                    break;
                case ShowResult.Skipped:
                    break;
                case ShowResult.Failed:
                    break;
            }
        }
#endif
    }
}

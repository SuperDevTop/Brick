using System;

namespace QAds
{
    public interface IQInterstitial
    {
        void RequestInterstitial();
        void ShowInterstitial(Action<bool> success);

        void LoadInterstitialCallbacks();
        void DisableInterstitialCallbacks();
    }
}

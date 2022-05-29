using System;

namespace QAds
{
    public interface IQRewarded
    {
        void RequestRewardedAd();
        void ShowRewardedAd(Action<bool> success);

        void LoadRewardedCallbacks();
        void DisableRewardedCallbacks();
    }
}

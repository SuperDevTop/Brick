using System;

namespace QAds
{
    public interface IQBanners
    {
        void ShowBanner(Action<bool> success);
        void RemoveBanner();

        void LoadBannerCallbacks();
        void DisableBannerCallbacks();
    }
}

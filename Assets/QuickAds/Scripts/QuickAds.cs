using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace QAds
{
    public enum ActiveBannerServicesEnum
    {
#if QUICK_ADS_ADMOB
        Admob,
#endif        
    }
    public enum ActiveInterstitialServicesEnum
    {
#if QUICK_ADS_ADMOB
        Admob,
#endif
#if QUICK_ADS_UNITY
        Unity,
#endif
#if QUICK_ADS_CHARTBOOST
        Chartboost
#endif
    }
    public enum ActiveRewardedServicesEnum
    {
#if QUICK_ADS_ADMOB
        Admob,
#endif
#if QUICK_ADS_UNITY
        Unity,
#endif
#if QUICK_ADS_CHARTBOOST
        Chartboost
#endif
    }

    public class QuickAds : MonoBehaviour
    {    
        #region ADMOB FIELDS
        public string AdmobAndroidAppId;
        public string AdmobIOSAppId;

        public string AdmobAndroidBannerId;
        public string AdmobIOSBannerId;

        public string AdmobAndroidInterstitialId;
        public string AdmobIOSInterstitialId;

        public string AdmobAndroidRewardedId;
        public string AdmobIOSRewardedId;

        public string AdmobAndroidBannerIdTEST = "ca-app-pub-3940256099942544/6300978111";
        public string AdmobIOSBannerIdTEST = "ca-app-pub-3940256099942544/2934735716";

        public string AdmobAndroidInterstitialIdTEST = "ca-app-pub-3940256099942544/1033173712";
        public string AdmobIOSInterstitialIdTEST = "ca-app-pub-3940256099942544/4411468910";

        public string AdmobAndroidRewardedIdTEST = "ca-app-pub-3940256099942544/5224354917";
        public string AdmobIOSRewardedIdTEST = "ca-app-pub-3940256099942544/1712485313";

        public AdmobBannerPositions admobBannerPosition;
        public AdmobBannerSize admobBannerSize;
        #endregion

        #region Chartboost fields
        public string ChartboostIOSId;
        public string ChartboostIOSSign;
        public string ChartboostAndroidId;
        public string ChartboostAndroidSign;
        public string ChartboostAmazonId;
        public string ChartboostAmazonSign;
        #endregion

        #region UNITY FIELDS
        public string unityAndroidId;
        public string unityIOSId;
        public string unityInterstitialVideoId;
        public string unityRewardedVideoId;
        #endregion

        #region FIELDS
        public List<ActiveBannerServicesEnum> chosenBannerServicesList = new List<ActiveBannerServicesEnum>();
        public List<ActiveInterstitialServicesEnum> chosenInterstitialServicesList = new List<ActiveInterstitialServicesEnum>();
        public List<ActiveRewardedServicesEnum> chosenRewardedServicesList = new List<ActiveRewardedServicesEnum>();

        public bool debugMode;
        public bool testMode = true;
        public bool initializeAdsOnLoad = true;
        public bool bannersIsActive;
        public bool interstitialIsActive;
        public bool rewardedIsActive;

        public bool loadBannerAtRun = false;
        public bool loadInterstitialAtRun = false;
        public bool loadRewardedAtRun = false;

        public bool randomInterstitialList = false;
        public bool randomRewardedList = false;

        public UnityEvent rewardedVideoIsShown;

        public bool isInitialized = false;

        public static QuickAds instance;
#endregion

        private void Awake()    ///////// SET INSTANCE AND SINGELTON /////////
        {
            if (instance == null)
                instance = this;
            else
                Destroy(gameObject);
            DontDestroyOnLoad(gameObject);

            if (initializeAdsOnLoad)
                Initialize();

            if (!debugMode)
                GetComponent<DebugScript>().enabled = false;
            else
                GetComponent<DebugScript>().enabled = true;
        }        

        public void Initialize()    ///////// INITIALIZE ADS /////////
        {
            if (!isInitialized)
            {
                isInitialized = true;
                ReportOnScreen("'Quick Ads' initialization");
                InitializeAdServices();
                LoadAllActiveAds();
                ShowAdsAtRun();
            }
        }

        void ShowAdsAtRun()
        {
            if (loadBannerAtRun)
                Invoke("ShowBanner", 1);
            if (loadInterstitialAtRun)
                Invoke("ShowInterstitialAd", 2);
            if (loadRewardedAtRun)
                Invoke("ShowRewardedAd", 2);
        }

        void InitializeAdServices() ///////// INITIALIZE AD SERVICES /////////
        {
            List<IQAdService> activeAdServicesList = new List<IQAdService>();

#if QUICK_ADS_ADMOB
            activeAdServicesList.Add(GetComponent<QAdmob>());
#endif
#if QUICK_ADS_UNITY
            activeAdServicesList.Add(GetComponent<QUnity>());
#endif
#if QUICK_ADS_CHARTBOOST
            activeAdServicesList.Add(GetComponent<QChartboost>());
#endif

            foreach (IQAdService adSer in activeAdServicesList)
            {
                adSer.InitializeAdService();
            }
        }

        void LoadAllActiveAds() ///////// LOAD ALL ACTIVE ADS /////////
        {
            if (interstitialIsActive)
                LoadAllInterstitialAds();
            if (rewardedIsActive)
                LoadAllRewardedAds();
        }

        void LoadAllInterstitialAds()   ///////// LOAD ALL INTERSTITIAL ADS /////////
        {
            List<IQInterstitial> interstList = new List<IQInterstitial>();
#if QUICK_ADS_ADMOB
            interstList.Add(GetComponent<QAdmob>());
#endif
#if QUICK_ADS_UNITY
            interstList.Add(GetComponent<QUnity>());
#endif
#if QUICK_ADS_CHARTBOOST
            interstList.Add(GetComponent<QChartboost>());
#endif

            foreach (IQInterstitial iL in interstList)
            {
                iL.RequestInterstitial();
            }
        }

        public void ShowBanner()    ///////// SHOW BANNER /////////
        {
            if (bannersIsActive)
            {
                if (isInitialized)
                {
                    bool bannerWasShowed = false;

                    for (int i = 0; i < chosenBannerServicesList.Count; i++)
                    {
#if QUICK_ADS_ADMOB
                    if (chosenBannerServicesList[i] == ActiveBannerServicesEnum.Admob && !bannerWasShowed)
                    {
                        ReportOnScreen("Attempt to show Admob banner");
                        GetComponent<QAdmob>().ShowBanner((bool success) =>
                        {
                            bannerWasShowed = success;
                            if (bannerWasShowed)
                            {
                                return;
                            }
                        });
                    }
#endif
                    }
                    if (!bannerWasShowed)
                    {
                        ReportOnScreen("There are no available banners to show");
                    }
                }
                else
                    ReportOnScreen("'Quick Ads' is not initialized. The ad can not be shown");
            }
        }

        public void RemoveBanner()
        {
            List<IQBanners> bannerList = new List<IQBanners>();
#if QUICK_ADS_ADMOB
            bannerList.Add(GetComponent<QAdmob>());
#endif
            foreach (IQBanners bL in bannerList)
            {
                bL.RemoveBanner();
            }
        }

        public void ShowInterstitialAd()    ///////// SHOW INTERSTITIAL /////////
        {
            if (interstitialIsActive)
            {
                if (isInitialized)
                {
                    bool interstitialWasShowed = false;
                    if (randomInterstitialList)
                        ShuffleInterstitialList();

                    for (int i = 0; i < chosenInterstitialServicesList.Count; i++)
                    {
#if QUICK_ADS_ADMOB
                    if (chosenInterstitialServicesList[i] == ActiveInterstitialServicesEnum.Admob && !interstitialWasShowed)
                    {
                        ReportOnScreen("Attempt to show Admob interstitial ad");
                        GetComponent<QAdmob>().ShowInterstitial((bool success) =>
                        {
                            interstitialWasShowed = success;
                            if (interstitialWasShowed)
                            {
                                ReportOnScreen("Attempt to show Admob interstitial ad was successful");
                                return;
                            }
                            else
                                ReportOnScreen("Attempt to show Admob interstitial ad failed");
                        });
                    }
#endif
#if QUICK_ADS_UNITY
                        if (chosenInterstitialServicesList[i] == ActiveInterstitialServicesEnum.Unity && !interstitialWasShowed)
                        {
                            ReportOnScreen("Attempt to show Unity interstitial ad");
                            GetComponent<QUnity>().ShowInterstitial((bool success) =>
                            {
                                interstitialWasShowed = success;
                                if (interstitialWasShowed)
                                {
                                    ReportOnScreen("Attempt to show Unity interstitial ad was successful");
                                    return;
                                }
                                else
                                    ReportOnScreen("Attempt to show Unity interstitial ad failed");
                            });
                        }
#endif
#if QUICK_ADS_CHARTBOOST
                        if (chosenInterstitialServicesList[i] == ActiveInterstitialServicesEnum.Chartboost && !interstitialWasShowed)
                        {
                            ReportOnScreen("Attempt to show Chartboost interstitial ad");
                            GetComponent<QChartboost>().ShowInterstitial((bool success) =>
                            {
                                interstitialWasShowed = success;
                                if (interstitialWasShowed)
                                {
                                    ReportOnScreen("Attempt to show Chartboost interstitial ad was successful");
                                    return;
                                }
                                else
                                    ReportOnScreen("Attempt to show Chartboost interstitial ad failed");
                            });
                        }
#endif
                    }
                    if (!interstitialWasShowed)
                    {
                        ReportOnScreen("There are no available interstitial ads to show");
                    }
                }
                else
                    ReportOnScreen("'Quick Ads' is not initialized. The ad can not be shown");
            }
        }

        public void ShowRewardedAd()    ///////// SHOW REWARDED /////////
        {
            if (rewardedIsActive)
            {
                if (isInitialized)
                {
                    bool rewardedWasShowed = false;
                    if (randomRewardedList)
                        ShuffleRewardedList();

                    for (int i = 0; i < chosenRewardedServicesList.Count; i++)
                    {
#if QUICK_ADS_ADMOB
                    if (chosenRewardedServicesList[i] == ActiveRewardedServicesEnum.Admob && !rewardedWasShowed)
                    {
                        ReportOnScreen("Attempt to show Admob rewarded ad");
                        GetComponent<QAdmob>().ShowRewardedAd((bool success) =>
                        {
                            rewardedWasShowed = success;
                            if (rewardedWasShowed)
                            {
                                ReportOnScreen("Attempt to show Admob rewarded ad was successful");
                                return;
                            }
                            else
                                ReportOnScreen("Attempt to show Admob rewarded ad failed");
                        });
                    }
#endif
#if QUICK_ADS_UNITY
                        if (chosenRewardedServicesList[i] == ActiveRewardedServicesEnum.Unity && !rewardedWasShowed)
                        {
                            ReportOnScreen("Attempt to show Unity rewarded ad");
                            GetComponent<QUnity>().ShowRewardedAd((bool success) =>
                            {
                                rewardedWasShowed = success;
                                if (rewardedWasShowed)
                                {
                                    ReportOnScreen("Attempt to show Unity rewarded ad was successful");
                                    return;
                                }
                                else
                                    ReportOnScreen("Attempt to show Unity rewarded ad failed");
                            });
                        }
#endif
#if QUICK_ADS_CHARTBOOST
                        if (chosenRewardedServicesList[i] == ActiveRewardedServicesEnum.Chartboost && !rewardedWasShowed)
                        {
                            ReportOnScreen("Attempt to show Chartboost rewarded ad");
                            GetComponent<QChartboost>().ShowRewardedAd((bool success) =>
                            {
                                rewardedWasShowed = success;
                                if (rewardedWasShowed)
                                {
                                    ReportOnScreen("Attempt to show Chartboost rewarded ad was successful");
                                    return;
                                }
                                else
                                    ReportOnScreen("Attempt to show Chartboost rewarded ad failed");
                            });
                        }
#endif
                    }
                    if (!rewardedWasShowed)
                    {
                        ReportOnScreen("There are no available rewarded ads to show");
                    }
                }
                else
                    ReportOnScreen("'Quick Ads' is not initialized. The ad can not be shown");
            }
        }        

        void LoadAllRewardedAds()           /////////// REQUEST FOR ALL REWARDED ADS ////////////
        {
            List<IQRewarded> rewarList = new List<IQRewarded>();
#if QUICK_ADS_ADMOB
            rewarList.Add(GetComponent<QAdmob>());
#endif
#if QUICK_ADS_UNITY
            rewarList.Add(GetComponent<QUnity>());
#endif
#if QUICK_ADS_CHARTBOOST
            rewarList.Add(GetComponent<QChartboost>());
#endif


            foreach (IQRewarded rL in rewarList)
            {
                rL.RequestRewardedAd();
            }
        }

        public void ReportOnScreen(string text)
        {
#if !UNITY_EDITOR
            if (debugMode)
                DebugScript.ShowOnScreen(text);
#endif
        }        

        void ShuffleInterstitialList()
        {
            for (int i = 0; i < chosenInterstitialServicesList.Count; i++)
            {
                var temp = chosenInterstitialServicesList[i];
                int randomIndex = UnityEngine.Random.Range(i, chosenInterstitialServicesList.Count);
                chosenInterstitialServicesList[i] = chosenInterstitialServicesList[randomIndex];
                chosenInterstitialServicesList[randomIndex] = temp;
            }
        }
        void ShuffleRewardedList()
        {
            for (int i = 0; i < chosenRewardedServicesList.Count; i++)
            {
                var temp = chosenRewardedServicesList[i];
                int randomIndex = UnityEngine.Random.Range(i, chosenRewardedServicesList.Count);
                chosenRewardedServicesList[i] = chosenRewardedServicesList[randomIndex];
                chosenRewardedServicesList[randomIndex] = temp;
            }
        }
    }    
}

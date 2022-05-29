using UnityEngine;

namespace QAds
{
    public class QData : MonoBehaviour
    {
        public static bool AdmobIsEnable
        {
            get
            {
                if (PlayerPrefs.GetString("AdmobIsEnable") == "true")
                    return true;
                else
                    return false;
            }
            set
            {
                if (value == true)
                    PlayerPrefs.SetString("AdmobIsEnable", "true");
                else
                    PlayerPrefs.SetString("AdmobIsEnable", "false");
                PlayerPrefs.Save();
            }
        }
        public static bool UnityAdsIsEnable
        {
            get
            {
                if (PlayerPrefs.GetString("UnityAdsIsEnable") == "true")
                    return true;
                else
                    return false;
            }
            set
            {
                if (value == true)
                    PlayerPrefs.SetString("UnityAdsIsEnable", "true");
                else
                    PlayerPrefs.SetString("UnityAdsIsEnable", "false");
                PlayerPrefs.Save();
            }
        }
        public static bool ChartboostIsEnable
        {
            get
            {
                if (PlayerPrefs.GetString("ChartboostIsEnable") == "true")
                    return true;
                else
                    return false;
            }
            set
            {
                if (value == true)
                    PlayerPrefs.SetString("ChartboostIsEnable", "true");
                else
                    PlayerPrefs.SetString("ChartboostIsEnable", "false");
                PlayerPrefs.Save();
            }
        }
    }
}

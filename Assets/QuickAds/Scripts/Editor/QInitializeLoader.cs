using UnityEditor;
using UnityEngine;

namespace QAds
{
    [InitializeOnLoad]
    public class QInitializeLoader : Editor
    {
        static QInitializeLoader()
        {
            CheckNetworkAvailability();
        }

        static void CheckNetworkAvailability()
        {
            if (QData.AdmobIsEnable)
                QDefineSymbols.AddDefineSymbol(QDefineSymbols.admobSymbol, true);
            if (QData.UnityAdsIsEnable)
                QDefineSymbols.AddDefineSymbol(QDefineSymbols.unityAdsSymbol, true);
            if (QData.ChartboostIsEnable)
                QDefineSymbols.AddDefineSymbol(QDefineSymbols.chartboostSymbol, true);
        }

        [MenuItem("Tools/Quick Ads/Add to the scene")]
        static void AddQuickAdsToScene()
        {
            var prefab = (GameObject)Instantiate(Resources.Load("Quick_Ads"));
            Undo.RegisterCreatedObjectUndo(prefab, "Added Quick Ads");
            prefab.name = "Quick_Ads";
        }
    }     
}


using System.Collections.Generic;
using UnityEditor;
using System;
using System.Linq;

namespace QAds
{
    public class QDefineSymbols : Editor
    {
        public static readonly string unityAdsSymbol = "QUICK_ADS_UNITY";
        public static readonly string admobSymbol = "QUICK_ADS_ADMOB";
        public static readonly string chartboostSymbol = "QUICK_ADS_CHARTBOOST";
        
        public static IEnumerable<T> GetBuildTargetGroupValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        //Add new Symbol
        public static void AddDefineSymbol(string symbol, bool isActivate)
        {                        
            foreach (BuildTargetGroup buildTargetGroup in GetBuildTargetGroupValues<BuildTargetGroup>())
            {
                if (EditorUserBuildSettings.activeBuildTarget.ToString().Contains(buildTargetGroup.ToString()))
                {
                    AddSymbol(symbol, buildTargetGroup, isActivate);
                }                
            }            
        }

        static void AddSymbol(string symbol, BuildTargetGroup buildTargetGroup, bool isActivate)
        {
            
            var s = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);

            if (isActivate)
            {
                if (!s.Contains(symbol))
                    s = symbol + ";" + s;
            }
            else
            {
                s = s.Replace(symbol +";", "");
                s = s.Replace(symbol, "");
            }
            
            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, s);
        }
    }
}

using UnityEditor;
using UnityEngine;
using UnityEditorInternal;

namespace QAds
{
    [CustomEditor(typeof(QuickAds))]
    public class QuickAdsEditor : Editor
    {
        #region FIELDS
        SerializedObject targetSer;

        QuickAds targetScript;
        SerializedProperty rewardedVideoIsShownEvent_Prop;
        ReorderableList bannerReorderableList, interstitialReorderableList, rewardedReorderableList;

        GUIStyle labelGuiStyle = new GUIStyle();
        GUIStyle networkGuiStyle = new GUIStyle();
        GUIStyle tooltipGuiStyle = new GUIStyle();
        GUIStyle supportGuiStyle = new GUIStyle();

        bool showAdmobSettings = false;
        bool showUnitySettings = false;
        bool showChartboostSettings = false;
#endregion

        private void OnEnable()
        {
            SetGuistyles();
            targetScript = (QuickAds)target;
            targetSer = new SerializedObject(target);
            rewardedVideoIsShownEvent_Prop = serializedObject.FindProperty("rewardedVideoIsShown");

            CreateReorderableLists();

            bannerReorderableList.onCanAddCallback = (ReorderableList l) => { return l.count < System.Enum.GetValues(typeof(ActiveBannerServicesEnum)).Length; };
            interstitialReorderableList.onCanAddCallback = (ReorderableList l) => { return l.count < System.Enum.GetValues(typeof(ActiveInterstitialServicesEnum)).Length; };
            rewardedReorderableList.onCanAddCallback = (ReorderableList l) => { return l.count < System.Enum.GetValues(typeof(ActiveRewardedServicesEnum)).Length; };
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();   /////////// BEGIN CHECK //////////////
            Undo.RecordObject(target, "Quick Ads");
            {
                serializedObject.Update();
                GUILayout.Space(5);    /////////// SPACE //////////////
                EditorGUILayout.LabelField("SUPPORT: QUICKADS.UNITY@GMAIL.COM", supportGuiStyle);
                EditorGUILayout.BeginHorizontal("BOX");
                targetScript.debugMode = EditorGUILayout.Toggle("DEBUG MODE*", targetScript.debugMode);   /////////// DEBUG //////////////
                targetScript.initializeAdsOnLoad = EditorGUILayout.Toggle("INITIALIZE AT LAUNCH", targetScript.initializeAdsOnLoad);   /////////// INITIALIZE AT RUN //////////////
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.LabelField("*debug mode is active on the device only", tooltipGuiStyle);
                GUILayout.Space(10);    /////////// SPACE //////////////
                DrawBannerBox();        /////////// BANNERBOX ///////////            
                GUILayout.Space(10);    /////////// SPACE //////////////
                DrawInterstitialBox();        /////////// INTERSTITIAL BOX ///////////  
                GUILayout.Space(10);    /////////// SPACE //////////////
                DrawRewardedBox();      /////////// REWARDED BOX ///////////
                GUILayout.Space(15);    /////////// SPACE //////////////
                DrawAdmobBox();
                GUILayout.Space(8);    /////////// SPACE //////////////
                DrawUnityBox();         ////////// UNITY BOX ///////////
                GUILayout.Space(8);    /////////// SPACE //////////////
                DrawChartboostBox();         ////////// CHARTBOOST BOX ///////////

                serializedObject.ApplyModifiedProperties();
            }
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);   /////////// END CHECK //////////////
            }
            GUILayout.Space(15);    /////////// SPACE //////////////
            DrawPlaginsButtons();   /////////// PLAGINS ////////////
            GUILayout.Space(10);    /////////// SPACE //////////////            
        }

        void SetGuistyles()
        {
            labelGuiStyle.normal.textColor = Color.blue;
            labelGuiStyle.fontSize = 13;
            labelGuiStyle.fontStyle = FontStyle.BoldAndItalic;
            labelGuiStyle.alignment = TextAnchor.LowerRight;

            networkGuiStyle.normal.textColor = Color.red;
            networkGuiStyle.fontSize = 13;
            networkGuiStyle.fontStyle = FontStyle.Bold;
            networkGuiStyle.alignment = TextAnchor.LowerCenter;

            tooltipGuiStyle.fontSize = 10;
            tooltipGuiStyle.alignment = TextAnchor.MiddleLeft;

            supportGuiStyle.alignment = TextAnchor.LowerRight;
            supportGuiStyle.fontStyle = FontStyle.Italic;
            supportGuiStyle.fontSize = 11;
        }

        void CreateReorderableLists()
        {
            bannerReorderableList = new ReorderableList(targetSer, serializedObject.FindProperty("chosenBannerServicesList"), true, true, true, true);
            bannerReorderableList.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Used ad networks"); };
            DrawReorderableList(bannerReorderableList);

            interstitialReorderableList = new ReorderableList(targetSer, serializedObject.FindProperty("chosenInterstitialServicesList"), true, true, true, true);
            interstitialReorderableList.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Used ad networks"); };
            DrawReorderableList(interstitialReorderableList);

            rewardedReorderableList = new ReorderableList(targetSer, serializedObject.FindProperty("chosenRewardedServicesList"), true, true, true, true);
            rewardedReorderableList.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Used ad networks"); };
            DrawReorderableList(rewardedReorderableList);
        }

        void DrawReorderableList(ReorderableList rList)
        {
            rList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                rect.y += 2;
                rect.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(rect, rList.serializedProperty.GetArrayElementAtIndex(index), new GUIContent(index + 1 + "st in the line"));
            };
        }

        bool EnableAdmob
        {
            get
            {
                return QData.AdmobIsEnable;
            }
            set
            {
                if (value == QData.AdmobIsEnable)
                    return;
                QData.AdmobIsEnable = value;
                QDefineSymbols.AddDefineSymbol(QDefineSymbols.admobSymbol, value);
            }
        }
        bool EnableUnityAds
        {
            get
            {
                return QData.UnityAdsIsEnable;
            }
            set
            {
                if (value == QData.UnityAdsIsEnable)
                    return;
                QData.UnityAdsIsEnable = value;
                QDefineSymbols.AddDefineSymbol(QDefineSymbols.unityAdsSymbol, value);
            }
        }
        bool EnableChartboost
        {
            get
            {
                return QData.ChartboostIsEnable;
            }
            set
            {
                if (value == QData.ChartboostIsEnable)
                    return;
                QData.ChartboostIsEnable = value;
                QDefineSymbols.AddDefineSymbol(QDefineSymbols.chartboostSymbol, value);
            }
        }
        
        void DrawBannerBox()
        {
            EditorGUILayout.LabelField("BANNER ADS    ", labelGuiStyle); /////////// BANNER ADS LABEL //////////////
            EditorGUILayout.BeginVertical("BOX");
            GUILayout.Space(5);    /////////// SPACE //////////////
            targetScript.bannersIsActive = EditorGUILayout.BeginToggleGroup("Use Banner Ads", targetScript.bannersIsActive); /////////// BANNERS IS ACTIVE GROUP //////////////
            GUILayout.Space(5);    /////////// SPACE //////////////

            if (targetScript.bannersIsActive)
            {
                targetScript.loadBannerAtRun = EditorGUILayout.Toggle("Show at launch", targetScript.loadBannerAtRun); /////////// SHOW BANNER AT RUN //////////////
                targetScript.admobBannerPosition = (AdmobBannerPositions)EditorGUILayout.EnumPopup("Banner position", targetScript.admobBannerPosition);    /////////// ADMOB BANNER POSITION //////////////
                targetScript.admobBannerSize = (AdmobBannerSize)EditorGUILayout.EnumPopup("Banner size", targetScript.admobBannerSize);    /////////// ADMOB BANNER SIZE //////////////
                GUILayout.Space(10);
                bannerReorderableList.DoLayoutList();    /////////// REORDABLE BANNER LIST //////////////
            }
            EditorGUILayout.EndToggleGroup();
            EditorGUILayout.EndVertical();
        }

        void DrawInterstitialBox()
        {
            EditorGUILayout.LabelField("INTERSTITIAL ADS    ", labelGuiStyle); /////////// INTERSTITIAL ADS LABEL //////////////
            EditorGUILayout.BeginVertical("BOX");
            GUILayout.Space(5);    /////////// SPACE //////////////
            targetScript.interstitialIsActive = EditorGUILayout.BeginToggleGroup("Use Interstitial Ads", targetScript.interstitialIsActive); /////////// INTERSTITIAL IS ACTIVE GROUP //////////////
            GUILayout.Space(5);    /////////// SPACE //////////////

            if (targetScript.interstitialIsActive)
            {
                EditorGUILayout.BeginHorizontal();
                targetScript.loadInterstitialAtRun = EditorGUILayout.Toggle("Show at launch", targetScript.loadInterstitialAtRun); /////////// SHOW INTERSTITIAL AT RUN //////////////
                targetScript.randomInterstitialList = EditorGUILayout.Toggle("Random order", targetScript.randomInterstitialList); /////////// RANDOM INTERSTITIAL //////////////
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(5);
                interstitialReorderableList.DoLayoutList();    /////////// REORDABLE INTERSTITIAL LIST //////////////
            }
            EditorGUILayout.EndToggleGroup();
            EditorGUILayout.EndVertical();
        }

        void DrawRewardedBox()
        {
            EditorGUILayout.LabelField("REWARDED ADS    ", labelGuiStyle); /////////// REWARDED ADS LABEL //////////////
            EditorGUILayout.BeginVertical("BOX");
            GUILayout.Space(5);    /////////// SPACE //////////////
            targetScript.rewardedIsActive = EditorGUILayout.BeginToggleGroup("Use Rewarded Ads", targetScript.rewardedIsActive); /////////// REWARDED IS ACTIVE GROUP //////////////
            GUILayout.Space(5);    /////////// SPACE //////////////

            if (targetScript.rewardedIsActive)
            {
                EditorGUILayout.BeginHorizontal();
                targetScript.loadRewardedAtRun = EditorGUILayout.Toggle("Show at launch", targetScript.loadRewardedAtRun); /////////// SHOW REWARDED AD AT RUN //////////////
                targetScript.randomRewardedList = EditorGUILayout.Toggle("Random order", targetScript.randomRewardedList); /////////// RANDOM rewarded //////////////
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(5);
                rewardedReorderableList.DoLayoutList();    /////////// REORDABLE INTERSTITIAL LIST //////////////
                GUILayout.Space(10);
                EditorGUILayout.PropertyField(rewardedVideoIsShownEvent_Prop);  /////////// SUCCESSFUL REWARDED VIDEO EVENT //////////////
            }
            EditorGUILayout.EndToggleGroup();
            EditorGUILayout.EndVertical();
        }

        void DrawAdmobBox()
        {
            EditorGUILayout.LabelField("ADMOB section    ", networkGuiStyle); /////////// ADMOB LABEL //////////////
            EditorGUILayout.BeginVertical("BOX");
            GUILayout.Space(10);    /////////// SPACE //////////////
            if (GUILayout.Button(EnableAdmob ? "DISABLE ADMOB" : "ENABLE ADMOB"))
            {
                if (!EnableAdmob)
                {
                    EditorApplication.Beep();
                    if (EditorUtility.DisplayDialog("Warning!", "Enable a network only if you have already downloaded the appropriate plugin.", "OK. Enable it", "Cancel"))
                        EnableAdmob = !EnableAdmob;
                }
                else
                    EnableAdmob = !EnableAdmob;
            }
            if (EnableAdmob)
            {
                GUILayout.Space(5);    /////////// SPACE //////////////
                EditorGUI.indentLevel++;
                showAdmobSettings = EditorGUILayout.Foldout(showAdmobSettings, "SETTINGS", true);
                if (showAdmobSettings)
                {
                    EditorGUI.indentLevel++;
                    DrawAdmobSettings();
                    EditorGUI.indentLevel--;
                }
                EditorGUI.indentLevel--;
            }
            GUILayout.Space(10);    /////////// SPACE //////////////
            EditorGUILayout.EndVertical();
        }

        void DrawAdmobSettings()
        {
            GUILayout.Space(5);    /////////// SPACE //////////////
            targetScript.testMode = EditorGUILayout.Toggle("Test mode*", targetScript.testMode);   /////////// TEST MODE //////////////
            EditorGUILayout.LabelField("*use test mode during development",tooltipGuiStyle);
            GUILayout.Space(8);    /////////// SPACE //////////////
            targetScript.AdmobAndroidAppId = EditorGUILayout.TextField("App Android ID", targetScript.AdmobAndroidAppId); /////////// ADMOB APP ID FIELDS //////////////
            targetScript.AdmobIOSAppId = EditorGUILayout.TextField("App IOS ID", targetScript.AdmobIOSAppId);
            GUILayout.Space(5);    /////////// SPACE //////////////
            targetScript.AdmobAndroidBannerId = EditorGUILayout.TextField("Banner Android ID", targetScript.AdmobAndroidBannerId); /////////// ADMOB BANNER ID FIELDS //////////////
            targetScript.AdmobIOSBannerId = EditorGUILayout.TextField("Banner IOS ID", targetScript.AdmobIOSBannerId);
            GUILayout.Space(5);    /////////// SPACE //////////////
            targetScript.AdmobAndroidInterstitialId = EditorGUILayout.TextField("Interstitial ad Android ID", targetScript.AdmobAndroidInterstitialId); /////////// ADMOB INTERSTITIAL ID FIELDS //////////////
            targetScript.AdmobIOSInterstitialId = EditorGUILayout.TextField("Interstitial ad IOS ID", targetScript.AdmobIOSInterstitialId);
            GUILayout.Space(5);    /////////// SPACE //////////////
            targetScript.AdmobAndroidRewardedId = EditorGUILayout.TextField("Rewarded ad Android ID", targetScript.AdmobAndroidRewardedId); /////////// ADMOB REWARDED ID FIELDS //////////////
            targetScript.AdmobIOSRewardedId = EditorGUILayout.TextField("Rewarded ad IOS ID", targetScript.AdmobIOSRewardedId);
        }

        void DrawUnityBox()
        {
            EditorGUILayout.LabelField("UNITY ADS section    ", networkGuiStyle); /////////// UNITY LABEL //////////////
            EditorGUILayout.BeginVertical("BOX");
            GUILayout.Space(10);    /////////// SPACE //////////////
            if (GUILayout.Button(EnableUnityAds ? "DISABLE UNITY ADS" : "ENABLE UNITY ADS"))
            {
                if (!EnableUnityAds)
                {
                    EditorApplication.Beep();
                    if (EditorUtility.DisplayDialog("Warning!", "Enable a network only if you have already downloaded the appropriate plugin.", "OK. Enable it", "Cancel"))
                        EnableUnityAds = !EnableUnityAds;
                }
                else
                    EnableUnityAds = !EnableUnityAds;
            }
            if (EnableUnityAds)
            {
                GUILayout.Space(5);    /////////// SPACE //////////////
                EditorGUI.indentLevel++;
                showUnitySettings = EditorGUILayout.Foldout(showUnitySettings, "SETTINGS", true);
                if (showUnitySettings)
                {
                    EditorGUI.indentLevel++;
                    DrawUnitySettings();
                    EditorGUI.indentLevel--;
                }
                EditorGUI.indentLevel--;
            }
            GUILayout.Space(10);    /////////// SPACE //////////////
            EditorGUILayout.EndVertical();
        }

        void DrawUnitySettings()
        {
            GUILayout.Space(5);    /////////// SPACE //////////////
            targetScript.unityAndroidId = EditorGUILayout.TextField("App Android ID", targetScript.unityAndroidId); /////////// UNITY APP ID FIELDS //////////////
            targetScript.unityIOSId = EditorGUILayout.TextField("App IOS ID", targetScript.unityIOSId);
            GUILayout.Space(5);    /////////// SPACE //////////////
            targetScript.unityInterstitialVideoId = EditorGUILayout.TextField("Video placement ID", targetScript.unityInterstitialVideoId);
            targetScript.unityRewardedVideoId = EditorGUILayout.TextField("Rewarded video placement ID", targetScript.unityRewardedVideoId);
        }

        void DrawChartboostBox()
        {
            EditorGUILayout.LabelField("CHARTBOOST section    ", networkGuiStyle); /////////// CHARTBOOST LABEL //////////////
            EditorGUILayout.BeginVertical("BOX");
            GUILayout.Space(10);    /////////// SPACE //////////////
            if (GUILayout.Button(EnableChartboost ? "DISABLE CHARTBOOST" : "ENABLE CHARTBOOST"))
            {
                if (!EnableChartboost)
                {
                    EditorApplication.Beep();
                    if (EditorUtility.DisplayDialog("Warning!", "Enable a network only if you have already downloaded the appropriate plugin.", "OK. Enable it", "Cancel"))
                        EnableChartboost = !EnableChartboost;
                }
                else
                    EnableChartboost = !EnableChartboost;
            }
            if (EnableChartboost)
            {
                GUILayout.Space(5);    /////////// SPACE //////////////
                EditorGUI.indentLevel++;
                showChartboostSettings = EditorGUILayout.Foldout(showChartboostSettings, "SETTINGS", true);
                if (showChartboostSettings)
                {
                    EditorGUI.indentLevel++;
                    DrawChartboostSettings();
                    EditorGUI.indentLevel--;
                }
                EditorGUI.indentLevel--;
            }
            GUILayout.Space(10);    /////////// SPACE //////////////
            EditorGUILayout.EndVertical();
        }

        void DrawChartboostSettings()
        {
            GUILayout.Space(5);    /////////// SPACE //////////////
            targetScript.ChartboostAndroidId = EditorGUILayout.TextField("App Google Play ID", targetScript.ChartboostAndroidId);
            targetScript.ChartboostAndroidSign = EditorGUILayout.TextField("App Google Play Signature", targetScript.ChartboostAndroidSign);
            GUILayout.Space(5);    /////////// SPACE //////////////
            targetScript.ChartboostIOSId = EditorGUILayout.TextField("App IOS ID", targetScript.ChartboostIOSId);
            targetScript.ChartboostIOSSign = EditorGUILayout.TextField("App IOS Signature", targetScript.ChartboostIOSSign);
            GUILayout.Space(5);    /////////// SPACE //////////////
            targetScript.ChartboostAmazonId = EditorGUILayout.TextField("App Amazon ID", targetScript.ChartboostAmazonId);
            targetScript.ChartboostAmazonSign = EditorGUILayout.TextField("App Amazon Signature", targetScript.ChartboostAmazonSign);
        }

        void DrawPlaginsButtons()
        {
            EditorGUILayout.LabelField("NETWORKS PLAGINS    ", labelGuiStyle); /////////// PLAGINS LABEL //////////////
            EditorGUILayout.BeginVertical("BOX");
            GUILayout.Space(8);
            if (GUILayout.Button("DOWNLOAD ADMOB SDK", GUILayout.Height(20)))
            {
                Application.OpenURL("https://github.com/googleads/googleads-mobile-unity/releases");
            }
            GUILayout.Space(8);
            
            if (GUILayout.Button("DOWNLOAD CHARTBOOST SDK", GUILayout.Height(20)))
            {
                Application.OpenURL("https://answers.chartboost.com/en-us/articles/download");
            }
            GUILayout.Space(8);

            if (GUILayout.Button("DOWNLOAD UNITY ADS SDK*", GUILayout.Height(20)))
            {
                Application.OpenURL("https://www.assetstore.unity3d.com/en/#!/content/66123");
            }
            
            GUILayout.Space(8);
            EditorGUILayout.EndVertical();
            EditorGUILayout.LabelField("*if you use UNITY 2017.2 or later you don't have to install this SDK", tooltipGuiStyle);
        }
    }
}

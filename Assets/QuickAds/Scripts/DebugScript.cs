using UnityEngine;

namespace QAds
{
    public class DebugScript : MonoBehaviour
    {
        static string debugText;
        static int linesNumber = 0;
        public int fontSize;

        public static void ShowOnScreen(string text)
        {
            debugText += "\n" + text;
            linesNumber++;
            if (linesNumber > 12)
            {
                int index = debugText.IndexOf("\n");
                debugText = debugText.Substring(index + 1);
                linesNumber--;
            }
        }

        private void OnGUI()
        {
            GUI.color = Color.yellow;
            GUI.skin.label.fontSize = fontSize;
            GUILayout.Label(debugText);
        }
    }
}

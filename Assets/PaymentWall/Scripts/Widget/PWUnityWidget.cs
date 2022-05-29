using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Paymentwall{
	public class PWUnityWidget{
		private PWWidget _widget;

		public PWUnityWidget(PWWidget widget){
			this._widget = widget;
		}

		public IEnumerator callWidgetWebView(GameObject containerObj,Canvas canvas){
			string uri = this._widget.GetUrl ();
#if UNITY_EDITOR_WIN || UNITY_EDITOR_WIN || UNITY_WEBPLAYER
			Application.OpenURL(uri);
#elif UNITY_ANDROID || UNITY_IOS || UNITY_EDITOR_OSX
			PWWebViewImpl wView = containerObj.AddComponent<PWWebViewImpl>();
			wView.Init((msg)=>{
				// output Init Message from iOS, Android
			});
			RectTransform rootCanvasRect = containerObj.GetComponent<RectTransform>();
			Vector2 posCenter = rootCanvasRect.rect.center;
			Vector2 sRect = rootCanvasRect.rect.size;
			wView.SetMargins(50,50,50,50);
			//wView.SetCenterPositionWithScale(posCenter,sRect* canvas.scaleFactor);
			wView.LoadURL(uri.Replace(" ","%20"));
			wView.SetVisibility(true);
#endif
			yield break;
		}
	}
}

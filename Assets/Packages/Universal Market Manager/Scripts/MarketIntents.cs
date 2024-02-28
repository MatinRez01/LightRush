using UnityEngine;

namespace UMM
{
	public static class MarketIntents
	{
		public static void OpenApp()
		{
#if UNITY_ANDROID
			var packageName = Application.identifier;

#if UMM_MYKET
			Myket();

			void Myket()
			{
				var intentClass = new AndroidJavaClass("android.content.Intent");
				var intentObject = new AndroidJavaObject("android.content.Intent");
				var uriClass = new AndroidJavaClass("android.net.Uri");

				intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_VIEW"));
				intentObject.Call<AndroidJavaObject>("setData", uriClass.CallStatic<AndroidJavaObject>("parse", "myket://details?id=" + packageName));
				intentObject.Call<AndroidJavaObject>("setPackage", "ir.mservices.market");

				var unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
				var currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
				currentActivity.Call("startActivity", intentObject);
			}
#elif UMM_BAZAAR
			CafeBazaar();

			void CafeBazaar()
			{
				var intentClass = new AndroidJavaClass("android.content.Intent");
				var intentObject = new AndroidJavaObject("android.content.Intent");
				var uriClass = new AndroidJavaClass("android.net.Uri");

				intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_VIEW"));
				intentObject.Call<AndroidJavaObject>("setData", uriClass.CallStatic<AndroidJavaObject>("parse", "bazaar://details?id=" + packageName));
				intentObject.Call<AndroidJavaObject>("setPackage", "com.farsitel.bazaar");

				var unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
				var currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
				currentActivity.Call("startActivity", intentObject);
			}
#endif
#endif
		}

		public static void OpenComments()
		{
#if UNITY_ANDROID
			var packageName = Application.identifier;

#if UMM_MYKET
			Myket();

			void Myket()
			{
                var intentClass = new AndroidJavaClass("android.content.Intent");
                var intentObject = new AndroidJavaObject("android.content.Intent");
                var uriClass = new AndroidJavaClass("android.net.Uri");

                intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_VIEW"));
                intentObject.Call<AndroidJavaObject>("setData", uriClass.CallStatic<AndroidJavaObject>("parse", "myket://comment?id=" + packageName));
                intentObject.Call<AndroidJavaObject>("setPackage", "ir.mservices.market");

                var unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                var currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
                currentActivity.Call("startActivity", intentObject);
            }
#elif UMM_BAZAAR
			CafeBazaar();

			void CafeBazaar()
			{
				var intentClass = new AndroidJavaClass("android.content.Intent");
				var intentObject = new AndroidJavaObject("android.content.Intent");
				var uriClass = new AndroidJavaClass("android.net.Uri");

				intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_EDIT"));
				intentObject.Call<AndroidJavaObject>("setData", uriClass.CallStatic<AndroidJavaObject>("parse", "bazaar://details?id=" + packageName));
				intentObject.Call<AndroidJavaObject>("setPackage", "com.farsitel.bazaar");

				var unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
				var currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
				currentActivity.Call("startActivity", intentObject);
			}
#endif
#endif
		}

		public static void OpenDeveloperApps()
		{
#if UNITY_ANDROID
#if UMM_MYKET
			Myket();

			void Myket()
			{
                var packageName = Application.identifier;

                var intentClass = new AndroidJavaClass("android.content.Intent");
                var intentObject = new AndroidJavaObject("android.content.Intent");
                var uriClass = new AndroidJavaClass("android.net.Uri");

                intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_VIEW"));
                intentObject.Call<AndroidJavaObject>("setData", uriClass.CallStatic<AndroidJavaObject>("parse", "myket://developer/" + packageName));
                intentObject.Call<AndroidJavaObject>("setPackage", "ir.mservices.market");

                var unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                var currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
                currentActivity.Call("startActivity", intentObject);
            }
#elif UMM_BAZAAR
			CafeBazaar();

			void CafeBazaar()
			{
				var developerId = MarketSettings.Instance.GetBazaarDeveloperId;

				if (developerId == null)
					throw new System.NullReferenceException($"{nameof(MarketSettings.Instance.GetBazaarDeveloperId)} is empty. Fill it's field in '{MarketSettings.TAB_ADDRESS}'.");

				var intentClass = new AndroidJavaClass("android.content.Intent");
				var intentObject = new AndroidJavaObject("android.content.Intent");
				var uriClass = new AndroidJavaClass("android.net.Uri");

				intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_VIEW"));
				intentObject.Call<AndroidJavaObject>("setData", uriClass.CallStatic<AndroidJavaObject>("parse", "bazaar://collection?slug=by_author&aid=" + developerId));
				intentObject.Call<AndroidJavaObject>("setPackage", "com.farsitel.bazaar");

				var unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
				var currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
				currentActivity.Call("startActivity", intentObject);
			}
#endif
#endif
		}

		public static void OpenMarketLogin()
		{
#if UNITY_ANDROID
#if UMM_BAZAAR
			CafeBazaar();

			void CafeBazaar()
			{
				var intentClass = new AndroidJavaClass("android.content.Intent");
				var intentObject = new AndroidJavaObject("android.content.Intent");
				var uriClass = new AndroidJavaClass("android.net.Uri");

				intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_VIEW"));
				intentObject.Call<AndroidJavaObject>("setData", uriClass.CallStatic<AndroidJavaObject>("parse", "bazaar://login"));
				intentObject.Call<AndroidJavaObject>("setPackage", "com.farsitel.bazaar");

				var unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
				var currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
				currentActivity.Call("startActivity", intentObject);
			}
#endif
#endif
		}
	}
}
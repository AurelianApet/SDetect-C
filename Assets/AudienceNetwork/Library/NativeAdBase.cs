using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using AudienceNetwork.Utility;

namespace AudienceNetwork
{
    public delegate void FBNativeAdBridgeCallback();
    public delegate void FBNativeAdBridgeErrorCallback(string error);
    internal delegate void FBNativeAdBridgeExternalCallback(int uniqueId);
    internal delegate void FBNativeAdBridgeErrorExternalCallback(int uniqueId, string error);

    public abstract class NativeAdBase
    {
        internal int uniqueId;
        protected bool isLoaded;
        protected AdHandler handler;

        internal NativeAdType nativeAdType;

        public string PlacementId { get; private set; }
        public string AdvertiserName { get; private set; }
        public string Headline { get; private set; }
        public string LinkDescription { get; private set; }
        public string SponsoredTranslation { get; private set; }
        public string AdTranslation { get; private set; }
        public string PromotedTranslation { get; private set; }
        public string Body { get; private set; }
        public string CallToAction { get; private set; }
        public string SocialContext { get; private set; }
        public string AdChoicesImageURL { get; private set; }
        public string AdChoicesText { get; private set; }
        public string AdChoicesLinkURL { get; private set; }

        private FBNativeAdBridgeCallback nativeAdDidLoad;
        private FBNativeAdBridgeCallback nativeAdWillLogImpression;
        private FBNativeAdBridgeErrorCallback nativeAdDidFailWithError;
        private FBNativeAdBridgeCallback nativeAdDidClick;
        private FBNativeAdBridgeCallback nativeAdDidFinishHandlingClick;
        private FBNativeAdBridgeCallback nativeAdDidDownloadMedia;

        public NativeAdBase(string placementId)
        {
            this.PlacementId = placementId;
            NativeAdBridgeInstance().OnLoad(uniqueId, NativeAdDidLoad);
            NativeAdBridgeInstance().OnImpression(uniqueId, NativeAdWillLogImpression);
            NativeAdBridgeInstance().OnClick(uniqueId, NativeAdDidClick);
            NativeAdBridgeInstance().OnError(uniqueId, NativeAdDidFailWithError);
            NativeAdBridgeInstance().OnFinishedClick(uniqueId, NativeAdDidFinishHandlingClick);
            NativeAdBridgeInstance().OnMediaDownloaded(uniqueId, NativeAdDidDownloadMedia);
        }

        internal virtual NativeAdBridge NativeAdBridgeInstance()
        {
            return NativeAdBridge.Instance;
        }

        ~NativeAdBase()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(Boolean iAmBeingCalledFromDisposeAndNotFinalize)
        {
            if (this.handler) {
                this.handler.removeFromParent();
            }
            NativeAdBridgeInstance().Release(uniqueId);
        }

        public override string ToString()
        {
            return string.Format(
                       "[NativeAd: " +
                       "PlacementId={0}, " +
                       "AdvertiserName={1}, " +
                       "Body={2}",
                       PlacementId,
                       AdvertiserName,
                       Body);
        }

        public virtual void LoadAd()
        {
            NativeAdBridgeInstance().Load(this.uniqueId);
        }

        public virtual void LoadAd(String bidPayload)
        {
            NativeAdBridgeInstance().Load(this.uniqueId, bidPayload);
        }

        public virtual void SetExtraHints(ExtraHints extraHints)
        {
            NativeAdBridgeInstance().SetExtraHints(this.uniqueId, extraHints);
        }

        protected int baseRegisterGameObjectsForInteraction(RectTransform mediaViewRectTransform, RectTransform ctaRectTransform,
                RectTransform iconViewRectTransform = null, Camera camera = null)
        {
            if (camera == null) {
                camera = Camera.main;
            }

            Rect mediaViewRect = getGameObjectRect(mediaViewRectTransform, camera);
            Rect iconViewRect = getGameObjectRect(iconViewRectTransform, camera);
            Rect ctaViewRect = getGameObjectRect(ctaRectTransform, camera);

            return RegisterGameObjectsForInteraction(mediaViewRect, iconViewRect, ctaViewRect);
        }

        protected virtual int RegisterGameObjectsForInteraction(Rect mediaViewRect, Rect iconViewRect, Rect ctaViewRect)
        {
            return NativeAdBridgeInstance().RegisterGameObjectsForInteraction(this.uniqueId, mediaViewRect, iconViewRect, ctaViewRect);
        }

        private Rect getGameObjectRect(RectTransform rectTransform, Camera camera)
        {
            if (rectTransform == null) {
                return Rect.zero;
            }

            Vector3[] worldCorners = new Vector3[4];
            Canvas canvas = getCanvas(handler.gameObject);

            rectTransform.GetWorldCorners(worldCorners);
            Vector3 gameObjectBottomLeft = worldCorners [0];
            Vector3 gameObjectTopRight = worldCorners [2];
            Vector3 cameraBottomLeft = camera.pixelRect.min;
            Vector3 cameraTopRight = camera.pixelRect.max;

            if (canvas.renderMode != RenderMode.ScreenSpaceOverlay) {
                gameObjectBottomLeft = camera.WorldToScreenPoint(gameObjectBottomLeft);
                gameObjectTopRight = camera.WorldToScreenPoint(gameObjectTopRight);
            }

            return new Rect(Mathf.Round(gameObjectBottomLeft.x),
                            Mathf.Floor((cameraTopRight.y - gameObjectTopRight.y)),
                            Mathf.Ceil((gameObjectTopRight.x - gameObjectBottomLeft.x)),
                            Mathf.Round((gameObjectTopRight.y - gameObjectBottomLeft.y)));
        }

        private Canvas getCanvas(GameObject gameObject)
        {
            if (gameObject.GetComponent<Canvas>() != null) {
                return gameObject.GetComponent<Canvas>();
            } else {
                if (gameObject.transform.parent != null) {
                    return getCanvas(gameObject.transform.parent.gameObject);
                }
            }
            return null;
        }

        public virtual bool IsValid()
        {
            return (this.isLoaded && NativeAdBridgeInstance().IsValid(this.uniqueId));
        }

        public void RegisterGameObject(GameObject gameObject)
        {
            this.createHandler(gameObject);
        }

        private void createHandler(GameObject gameObject)
        {
            this.handler = gameObject.AddComponent<AdHandler> ();
        }

        internal virtual void loadAdFromData()
        {
            if (this.handler == null) {
                throw new InvalidOperationException("Native ad was loaded before it was registered. " +
                                                    "Ensure RegisterGameObjectForImpression () are called.");
            }
            int uniqueId = this.uniqueId;
            this.AdvertiserName = NativeAdBridgeInstance().GetAdvertiserName(uniqueId);
            this.Headline = NativeAdBridgeInstance().GetHeadline(uniqueId);
            this.LinkDescription = NativeAdBridgeInstance().GetLinkDescription(uniqueId);
            this.SponsoredTranslation = NativeAdBridgeInstance().GetSponsoredTranslation(uniqueId);
            this.AdTranslation = NativeAdBridgeInstance().GetAdTranslation(uniqueId);
            this.PromotedTranslation = NativeAdBridgeInstance().GetPromotedTranslation(uniqueId);
            this.Body = NativeAdBridgeInstance().GetBody(uniqueId);
            this.CallToAction = NativeAdBridgeInstance().GetCallToAction(uniqueId);
            this.SocialContext = NativeAdBridgeInstance().GetSocialContext(uniqueId);
            this.CallToAction = NativeAdBridgeInstance().GetCallToAction(uniqueId);
            this.AdChoicesImageURL = NativeAdBridgeInstance().GetAdChoicesImageURL(uniqueId);
            this.AdChoicesText = NativeAdBridgeInstance().GetAdChoicesText(uniqueId);
            this.AdChoicesLinkURL = NativeAdBridgeInstance().GetAdChoicesLinkURL(uniqueId);
            this.isLoaded = true;

            if (this.NativeAdDidLoad != null) {
                this.handler.executeOnMainThread(() => {
                    this.NativeAdDidLoad();
                });
            }
        }

        internal void executeOnMainThread(Action action)
        {
            if (this.handler) {
                this.handler.executeOnMainThread(action);
            }
        }

        public static implicit operator bool (NativeAdBase obj)
        {
            return !(object.ReferenceEquals(obj, null));
        }

        public FBNativeAdBridgeCallback NativeAdDidLoad {
            internal get {
                return this.nativeAdDidLoad;
            }
            set {
                this.nativeAdDidLoad = value;
                NativeAdBridgeInstance().OnLoad(uniqueId, nativeAdDidLoad);
            }
        }

        public FBNativeAdBridgeCallback NativeAdWillLogImpression {
            internal get {
                return this.nativeAdWillLogImpression;
            }
            set {
                this.nativeAdWillLogImpression = value;
                NativeAdBridgeInstance().OnImpression(uniqueId, nativeAdWillLogImpression);
            }
        }

        public FBNativeAdBridgeErrorCallback NativeAdDidFailWithError {
            internal get {
                return this.nativeAdDidFailWithError;
            }
            set {
                this.nativeAdDidFailWithError = value;
                NativeAdBridgeInstance().OnError(uniqueId, nativeAdDidFailWithError);
            }
        }

        public FBNativeAdBridgeCallback NativeAdDidClick {
            internal get {
                return this.nativeAdDidClick;
            }
            set {
                this.nativeAdDidClick = value;
                NativeAdBridgeInstance().OnClick(uniqueId, nativeAdDidClick);
            }
        }

        public FBNativeAdBridgeCallback NativeAdDidFinishHandlingClick {
            internal get {
                return this.nativeAdDidFinishHandlingClick;
            }
            set {
                this.nativeAdDidFinishHandlingClick = value;
                NativeAdBridgeInstance().OnFinishedClick(uniqueId, nativeAdDidFinishHandlingClick);
            }
        }

        public FBNativeAdBridgeCallback NativeAdDidDownloadMedia {
            internal get {
                return this.nativeAdDidDownloadMedia;
            }
            set {
                this.nativeAdDidDownloadMedia = value;
                NativeAdBridgeInstance().OnMediaDownloaded(uniqueId, nativeAdDidDownloadMedia);
            }
        }
    }

    enum NativeAdType { NativeAd, NativeBannerAd };

    internal class NativeAdBridge
    {
        public static NativeAdBridge Instance;
        private List<NativeAdBase> nativeAds = new List<NativeAdBase> ();

        static NativeAdBridge()
        {
            Instance = NativeAdBridge.createInstance();
        }

        private static NativeAdBridge createInstance()
        {
            if (Application.platform != RuntimePlatform.OSXEditor) {
#if UNITY_IOS
                return new NativeAdBridgeIOS();
#elif UNITY_ANDROID
                return new NativeAdBridgeAndroid();
#else
                return new NativeAdBridge();
#endif
            } else {
                return new NativeAdBridge();
            }
        }

        public virtual int Create(string placementId, NativeAdBase nativeAd)
        {
            nativeAds.Add(nativeAd);
            return nativeAds.Count - 1;
        }

        public virtual int Load(int uniqueId)
        {
            NativeAdBase nativeAd = this.nativeAds [uniqueId];
            nativeAd.loadAdFromData();
            return uniqueId;
        }

        public virtual int Load(int uniqueId, String bidPayload)
        {
            NativeAdBase nativeAd = this.nativeAds[uniqueId];
            nativeAd.loadAdFromData();
            return uniqueId;
        }

        public virtual void SetExtraHints(int uniqueId, ExtraHints extraHints)
        {
        }

        public virtual int RegisterGameObjectsForInteraction(int uniqueId, Rect mediaViewRect, Rect iconViewRect, Rect ctaViewRect)
        {
            return -1;
        }

        public virtual bool IsValid(int uniqueId)
        {
            return true;
        }

        public virtual string GetAdvertiserName(int uniqueId)
        {
            return "Facebook";
        }

        public virtual string GetHeadline(int uniqueId)
        {
            return "A Facebook Ad";
        }

        public virtual string GetLinkDescription(int uniqueId)
        {
            return "Facebook.com";
        }

        public virtual string GetSponsoredTranslation(int uniqueId)
        {
            return "Sponsored Translation";
        }

        public virtual string GetAdTranslation(int uniqueId)
        {
            return "Ad Translation";
        }

        public virtual string GetPromotedTranslation(int uniqueId)
        {
            return "Promoted Translation";
        }

        public virtual string GetBody(int uniqueId)
        {
            return "Your ad integration works. Woohoo!";
        }

        public virtual string GetCallToAction(int uniqueId)
        {
            return "Install Now";
        }

        public virtual string GetSocialContext(int uniqueId)
        {
            return "Available on the App Store";
        }

        public virtual string GetAdChoicesImageURL(int uniqueId)
        {
            return "https://www.facebook.com/images/ad_network/ad_choices.png";
        }

        public virtual string GetAdChoicesText(int uniqueId)
        {
            return "AdChoices";
        }

        public virtual string GetAdChoicesLinkURL(int uniqueId)
        {
            return "https://m.facebook.com/ads/ad_choices/";
        }

        public virtual void Release(int uniqueId)
        {
        }

        public virtual void OnLoad(int uniqueId,
                                   FBNativeAdBridgeCallback callback)
        {
        }

        public virtual void OnImpression(int uniqueId,
                                         FBNativeAdBridgeCallback callback)
        {
        }

        public virtual void OnClick(int uniqueId,
                                    FBNativeAdBridgeCallback callback)
        {
        }

        public virtual void OnError(int uniqueId,
                                    FBNativeAdBridgeErrorCallback callback)
        {
        }

        public virtual void OnFinishedClick(int uniqueId,
                                            FBNativeAdBridgeCallback callback)
        {
        }

        public virtual void OnMediaDownloaded(int uniqueId,
                                              FBNativeAdBridgeCallback callback)
        {
        }
    }

#if UNITY_ANDROID
    internal class NativeAdBridgeAndroid : NativeAdBridge
    {
        protected static Dictionary<int, NativeAdContainer> nativeAds = new Dictionary<int, NativeAdContainer>();
        protected static int lastKey = 0;

        protected AndroidJavaObject nativeAdForNativeAdId(int uniqueId)
        {
            NativeAdContainer nativeAdContainer = null;
            bool success = NativeAdBridgeAndroid.nativeAds.TryGetValue(uniqueId, out nativeAdContainer);
            if (success) {
                return nativeAdContainer.bridgedNativeAd;
            } else {
                return null;
            }
        }

        protected NativeAdContainer nativeAdContainerForNativeAdId(int uniqueId)
        {
            NativeAdContainer nativeAdContainer = null;
            bool success = NativeAdBridgeAndroid.nativeAds.TryGetValue(uniqueId, out nativeAdContainer);
            if (success) {
                return nativeAdContainer;
            } else {
                return null;
            }
        }

        protected string getStringForNativeAdId(int uniqueId, string method)
        {
            AndroidJavaObject nativeAd = this.nativeAdForNativeAdId(uniqueId);
            if (nativeAd != null) {
                return nativeAd.Call<string> (method);
            } else {
                return null;
            }
        }

        override public int Create(string placementId, NativeAdBase nativeAd)
        {
            AdUtility.prepare();
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");

            String androidAdClass = "";
            if (nativeAd.nativeAdType == NativeAdType.NativeAd) {
                androidAdClass = "com.facebook.ads.NativeAd";
            } else if (nativeAd.nativeAdType == NativeAdType.NativeBannerAd) {
                androidAdClass = "com.facebook.ads.NativeBannerAd";
            }

            AndroidJavaObject bridgedNativeAd = new AndroidJavaObject(androidAdClass, context, placementId);

            NativeAdBridgeListenerProxy proxy = new NativeAdBridgeListenerProxy(nativeAd, bridgedNativeAd);
            bridgedNativeAd.Call("setAdListener", proxy);

            NativeAdContainer nativeAdContainer = new NativeAdContainer(nativeAd);
            nativeAdContainer.bridgedNativeAd = bridgedNativeAd;
            nativeAdContainer.listenerProxy = proxy;
            nativeAdContainer.context = context;

            int key = NativeAdBridgeAndroid.lastKey;
            NativeAdBridgeAndroid.nativeAds.Add(key, nativeAdContainer);
            NativeAdBridgeAndroid.lastKey++;
            return key;
        }

        public override int Load(int uniqueId)
        {
            AdUtility.prepare();
            AndroidJavaObject nativeAd = this.nativeAdForNativeAdId(uniqueId);
            if (nativeAd != null) {
                nativeAd.Call("loadAd");
            }
            return uniqueId;
        }

        public override int Load(int uniqueId, String bidPayload)
        {
            AdUtility.prepare();
            AndroidJavaObject nativeAd = this.nativeAdForNativeAdId(uniqueId);
            if (nativeAd != null)
            {
                nativeAd.Call("loadAdFromBid", bidPayload);
            }
            return uniqueId;
        }

        public override void SetExtraHints(int uniqueId, ExtraHints extraHints)
        {
            AdUtility.prepare();
            AndroidJavaObject nativeAd = this.nativeAdForNativeAdId(uniqueId);

            if (nativeAd != null)
            {
                nativeAd.Call("setExtraHints", extraHints.getAndroidObject());
            }
        }

        public override int RegisterGameObjectsForInteraction(int uniqueId, Rect mediaViewRect, Rect iconViewRect, Rect ctaViewRect)
        {
            NativeAdContainer nativeAdContainer = this.nativeAdContainerForNativeAdId(uniqueId);
            AndroidJavaObject nativeAd = nativeAdContainer.bridgedNativeAd;
            if (nativeAd != null) {
                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaObject context = nativeAdContainer.context;
                activity.Call("runOnUiThread", new AndroidJavaRunnable(() => {
                    AndroidJavaClass R = new AndroidJavaClass("android.R$id");
                    AndroidJavaObject unityView = activity.Call<AndroidJavaObject> ("findViewById", R.GetStatic<int> ("content"));

                    AndroidJavaObject iconView = createViewFromRect(iconViewRect, "com.facebook.ads.AdIconView", context);
                    nativeAdContainer.iconView = iconView;
                    AndroidJavaObject ctaView = createViewFromRect(ctaViewRect, "android/view/View", context);
                    nativeAdContainer.ctaView = ctaView;
                    unityView.Call("addView", iconView);
                    unityView.Call("addView", ctaView);
                    AndroidJavaObject ctaViews = new AndroidJavaObject("java.util.ArrayList");
                    ctaViews.Call<bool>("add", ctaView);

                    if (mediaViewRect != Rect.zero) {
                        AndroidJavaObject mediaView = createViewFromRect(mediaViewRect, "com.facebook.ads.MediaView", context);
                        nativeAdContainer.mediaView = mediaView;
                        unityView.Call("addView", mediaView);
                        nativeAd.Call("registerViewForInteraction", unityView, mediaView, iconView, ctaViews);
                    } else {
                        nativeAd.Call("registerViewForInteraction", unityView, iconView, ctaViews);
                    }
                }));
            }
            return uniqueId;
        }

        protected AndroidJavaObject createViewFromRect(Rect rect, string type, AndroidJavaObject context)
        {
            AndroidJavaObject view = new AndroidJavaObject(type, context);
            view.Call("setX", rect.x);
            view.Call("setY", rect.y);
            AndroidJavaObject layoutParams = new AndroidJavaObject("android/view/ViewGroup$LayoutParams",
                    (int)rect.width, (int)rect.height);

            view.Call("setLayoutParams", layoutParams);
            return view;
        }

        public override bool IsValid(int uniqueId)
        {
            AndroidJavaObject nativeAd = this.nativeAdForNativeAdId(uniqueId);
            if (nativeAd != null) {
                return nativeAd.Call<bool> ("isAdLoaded");
            } else {
                return false;
            }
        }

        public override string GetAdvertiserName(int uniqueId)
        {
            return this.getStringForNativeAdId(uniqueId, "getAdvertiserName");
        }

        public override string GetHeadline(int uniqueId)
        {
            return this.getStringForNativeAdId(uniqueId, "getAdHeadline");
        }

        public override string GetLinkDescription(int uniqueId)
        {
            return this.getStringForNativeAdId(uniqueId, "getAdLinkDescription");
        }

        public override string GetSponsoredTranslation(int uniqueId)
        {
            return this.getStringForNativeAdId(uniqueId, "getSponsoredTranslation");
        }

        public override string GetAdTranslation(int uniqueId)
        {
            return this.getStringForNativeAdId(uniqueId, "getAdTranslation");
        }

        public override string GetPromotedTranslation(int uniqueId)
        {
            return this.getStringForNativeAdId(uniqueId, "getPromotedTranslation");
        }

        public override string GetBody(int uniqueId)
        {
            return this.getStringForNativeAdId(uniqueId, "getAdBodyText");
        }

        public override string GetCallToAction(int uniqueId)
        {
            return this.getStringForNativeAdId(uniqueId, "getAdCallToAction");
        }

        public override string GetSocialContext(int uniqueId)
        {
            return this.getStringForNativeAdId(uniqueId, "getAdSocialContext");
        }

        public override string GetAdChoicesImageURL(int uniqueId)
        {
            return this.getStringForNativeAdId(uniqueId, "getAdChoicesImageUrl");
        }

        public override string GetAdChoicesText(int uniqueId)
        {
            return this.getStringForNativeAdId(uniqueId, "getAdChoicesText");
        }

        public override string GetAdChoicesLinkURL(int uniqueId)
        {
            return this.getStringForNativeAdId(uniqueId, "getAdChoicesLinkUrl");
        }

        private string getId(int uniqueId)
        {
            AndroidJavaObject nativeAd = this.nativeAdForNativeAdId(uniqueId);
            if (nativeAd != null) {
                return nativeAd.Call<string> ("getId");
            } else {
                return null;
            }
        }

        public override void Release(int uniqueId)
        {
            NativeAdContainer nativeAdContainer = this.nativeAdContainerForNativeAdId(uniqueId);
            AndroidJavaObject nativeAd = nativeAdContainer.bridgedNativeAd;
            NativeAdBridgeAndroid.nativeAds.Remove(uniqueId);
            if (nativeAd != null) {
                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                activity.Call("runOnUiThread", new AndroidJavaRunnable(() => {
                    AndroidJavaObject parent = nativeAdContainer.ctaView.Call<AndroidJavaObject> ("getParent");
                    nativeAd.Call("destroy");
                    parent.Call("removeView", nativeAdContainer.mediaView);
                    parent.Call("removeView", nativeAdContainer.iconView);
                    parent.Call("removeView", nativeAdContainer.ctaView);
                }));
            };
        }

        public override void OnLoad(int uniqueId, FBNativeAdBridgeCallback callback) {}
        public override void OnImpression(int uniqueId, FBNativeAdBridgeCallback callback) {}
        public override void OnClick(int uniqueId, FBNativeAdBridgeCallback callback) {}
        public override void OnError(int uniqueId, FBNativeAdBridgeErrorCallback callback) {}
        public override void OnFinishedClick(int uniqueId, FBNativeAdBridgeCallback callback) {}
        public override void OnMediaDownloaded(int uniqueId, FBNativeAdBridgeCallback callback) {}

    }

#endif

#if UNITY_IOS
    internal class NativeAdBridgeIOS : NativeAdBridge
    {

        private static Dictionary<int, NativeAdContainer> nativeAds = new Dictionary<int, NativeAdContainer>();

        private static NativeAdContainer nativeAdContainerForNativeAdId(int uniqueId)
        {
            NativeAdContainer nativeAd = null;
            bool success = NativeAdBridgeIOS.nativeAds.TryGetValue(uniqueId, out nativeAd);
            if (success) {
                return nativeAd;
            } else {
                return null;
            }
        }

        [DllImport("__Internal")]
        private static extern int FBNativeAdBridgeCreate(string placementId);

        [DllImport("__Internal")]
        private static extern int FBNativeAdBridgeLoad(int uniqueId);

        [DllImport("__Internal")]
        private static extern int FBNativeAdBridgeLoadWithBidPayload(int uniqueId, string bidPayload);

        [DllImport("__Internal")]
        private static extern int FBNativeAdBridgeRegisterViewsForInteraction(int uniqueId,
                int mediaViewX, int mediaViewY, int mediaViewWidth, int mediaViewHeight,
                int iconViewX, int iconViewY, int iconViewWidth, int iconViewHeight,
                int ctaViewX, int ctaViewY, int ctaViewWidth, int ctaViewHeight);

        [DllImport("__Internal")]
        private static extern bool FBNativeAdBridgeIsValid(int uniqueId);

        [DllImport("__Internal")]
        private static extern string FBNativeAdBridgeGetAdvertiserName(int uniqueId);

        [DllImport("__Internal")]
        private static extern string FBNativeAdBridgeGetHeadline(int uniqueId);

        [DllImport("__Internal")]
        private static extern string FBNativeAdBridgeGetLinkDescription(int uniqueId);

        [DllImport("__Internal")]
        private static extern string FBNativeAdBridgeGetSponsoredTranslation(int uniqueId);

        [DllImport("__Internal")]
        private static extern string FBNativeAdBridgeGetAdTranslation(int uniqueId);

        [DllImport("__Internal")]
        private static extern string FBNativeAdBridgeGetPromotedTranslation(int uniqueId);

        [DllImport("__Internal")]
        private static extern string FBNativeAdBridgeGetBody(int uniqueId);

        [DllImport("__Internal")]
        private static extern string FBNativeAdBridgeGetCallToAction(int uniqueId);

        [DllImport("__Internal")]
        private static extern string FBNativeAdBridgeGetSocialContext(int uniqueId);

        [DllImport("__Internal")]
        private static extern string FBNativeAdBridgeGetAdChoicesImageURL(int uniqueId);

        [DllImport("__Internal")]
        private static extern string FBNativeAdBridgeGetAdChoicesText(int uniqueId);

        [DllImport("__Internal")]
        private static extern string FBNativeAdBridgeGetAdChoicesLinkURL(int uniqueId);

        [DllImport("__Internal")]
        private static extern void FBNativeAdBridgeSetExtraHints(int uniqueId, string extraHints);

        [DllImport("__Internal")]
        private static extern void FBNativeAdBridgeRelease(int uniqueId);

        [DllImport("__Internal")]
        private static extern void FBNativeAdBridgeOnLoad(int uniqueId,
                FBNativeAdBridgeExternalCallback callback);

        [DllImport("__Internal")]
        private static extern void FBNativeAdBridgeOnImpression(int uniqueId,
                FBNativeAdBridgeExternalCallback callback);

        [DllImport("__Internal")]
        private static extern void FBNativeAdBridgeOnClick(int uniqueId,
                FBNativeAdBridgeExternalCallback callback);

        [DllImport("__Internal")]
        private static extern void FBNativeAdBridgeOnError(int uniqueId,
                FBNativeAdBridgeErrorExternalCallback callback);

        [DllImport("__Internal")]
        private static extern void FBNativeAdBridgeOnFinishedClick(int uniqueId,
                FBNativeAdBridgeExternalCallback callback);

        [DllImport("__Internal")]
        private static extern void FBNativeAdBridgeOnMediaDownloaded(int uniqueId,
                FBNativeAdBridgeExternalCallback callback);

        public override int Create(string placementId, NativeAdBase nativeAd)
        {
            int uniqueId = NativeAdBridgeIOS.FBNativeAdBridgeCreate(placementId);
            NativeAdBridgeIOS.nativeAds.Add(uniqueId, new NativeAdContainer(nativeAd));
            NativeAdBridgeIOS.FBNativeAdBridgeOnLoad(uniqueId, nativeAdDidLoadBridgeCallback);
            NativeAdBridgeIOS.FBNativeAdBridgeOnImpression(uniqueId, nativeAdWillLogImpressionmpressionBridgeCallback);
            NativeAdBridgeIOS.FBNativeAdBridgeOnClick(uniqueId, nativeAdDidClickBridgeCallback);
            NativeAdBridgeIOS.FBNativeAdBridgeOnError(uniqueId, nativeAdDidFailWithErrorBridgeCallback);
            NativeAdBridgeIOS.FBNativeAdBridgeOnFinishedClick(uniqueId, nativeAdDidFinishHandlingClickBridgeCallback);
            NativeAdBridgeIOS.FBNativeAdBridgeOnMediaDownloaded(uniqueId, nativeAdDidDownloadMediaBridgeCallback);

            return uniqueId;
        }

        public override int Load(int uniqueId)
        {
            return NativeAdBridgeIOS.FBNativeAdBridgeLoad(uniqueId);
        }

        public override int Load(int uniqueId, string bidPayload)
        {
            return NativeAdBridgeIOS.FBNativeAdBridgeLoadWithBidPayload(uniqueId, bidPayload);
        }

        public override int RegisterGameObjectsForInteraction(int uniqueId, Rect mediaViewRect, Rect iconViewRect, Rect ctaViewRect)
        {
            return NativeAdBridgeIOS.FBNativeAdBridgeRegisterViewsForInteraction(uniqueId,
                    (int)mediaViewRect.x, (int)mediaViewRect.y, (int)mediaViewRect.width, (int)mediaViewRect.height,
                    (int)iconViewRect.x, (int)iconViewRect.y, (int)iconViewRect.width, (int)iconViewRect.height,
                    (int)ctaViewRect.x, (int)ctaViewRect.y, (int)ctaViewRect.width, (int)ctaViewRect.height);
        }

        public override bool IsValid(int uniqueId)
        {
            return NativeAdBridgeIOS.FBNativeAdBridgeIsValid(uniqueId);
        }

        public override string GetAdvertiserName(int uniqueId)
        {
            return NativeAdBridgeIOS.FBNativeAdBridgeGetAdvertiserName(uniqueId);
        }

        public override string GetHeadline(int uniqueId)
        {
            return NativeAdBridgeIOS.FBNativeAdBridgeGetHeadline(uniqueId);
        }

        public override string GetLinkDescription(int uniqueId)
        {
            return NativeAdBridgeIOS.FBNativeAdBridgeGetLinkDescription(uniqueId);
        }

        public override string GetSponsoredTranslation(int uniqueId)
        {
            return NativeAdBridgeIOS.FBNativeAdBridgeGetSponsoredTranslation(uniqueId);
        }

        public override string GetAdTranslation(int uniqueId)
        {
            return NativeAdBridgeIOS.FBNativeAdBridgeGetAdTranslation(uniqueId);
        }

        public override string GetPromotedTranslation(int uniqueId)
        {
            return NativeAdBridgeIOS.FBNativeAdBridgeGetPromotedTranslation(uniqueId);
        }

        public override string GetBody(int uniqueId)
        {
            return NativeAdBridgeIOS.FBNativeAdBridgeGetBody(uniqueId);
        }

        public override string GetCallToAction(int uniqueId)
        {
            return NativeAdBridgeIOS.FBNativeAdBridgeGetCallToAction(uniqueId);
        }

        public override string GetSocialContext(int uniqueId)
        {
            return NativeAdBridgeIOS.FBNativeAdBridgeGetSocialContext(uniqueId);
        }

        public override string GetAdChoicesImageURL(int uniqueId)
        {
            return NativeAdBridgeIOS.FBNativeAdBridgeGetAdChoicesImageURL(uniqueId);
        }

        public override string GetAdChoicesText(int uniqueId)
        {
            return NativeAdBridgeIOS.FBNativeAdBridgeGetAdChoicesText(uniqueId);
        }

        public override string GetAdChoicesLinkURL(int uniqueId)
        {
            return NativeAdBridgeIOS.FBNativeAdBridgeGetAdChoicesLinkURL(uniqueId);
        }

        public override void SetExtraHints(int uniqueId, ExtraHints extraHints)
        {
            NativeAdBridgeIOS.FBNativeAdBridgeSetExtraHints(uniqueId, JsonUtility.ToJson(extraHints));
        }

        public override void Release(int uniqueId)
        {
            NativeAdBridgeIOS.nativeAds.Remove(uniqueId);
            NativeAdBridgeIOS.FBNativeAdBridgeRelease(uniqueId);
        }

        // Sets up internal managed callbacks

        public override void OnLoad(int uniqueId,
                                    FBNativeAdBridgeCallback callback)
        {
            NativeAdContainer container = NativeAdBridgeIOS.nativeAdContainerForNativeAdId(uniqueId);
            if (container) {
                container.onLoad = (delegate() {
                    container.nativeAd.loadAdFromData();
                });
            }
        }

        public override void OnImpression(int uniqueId,
                                          FBNativeAdBridgeCallback callback)
        {
            NativeAdContainer container = NativeAdBridgeIOS.nativeAdContainerForNativeAdId(uniqueId);
            if (container) {
                container.onImpression = callback;
            }
        }

        public override void OnClick(int uniqueId,
                                     FBNativeAdBridgeCallback callback)
        {
            NativeAdContainer container = NativeAdBridgeIOS.nativeAdContainerForNativeAdId(uniqueId);
            if (container) {
                container.onClick = callback;
            }
        }

        public override void OnError(int uniqueId,
                                     FBNativeAdBridgeErrorCallback callback)
        {
            NativeAdContainer container = NativeAdBridgeIOS.nativeAdContainerForNativeAdId(uniqueId);
            if (container) {
                container.onError = callback;
            }
        }

        public override void OnFinishedClick(int uniqueId, FBNativeAdBridgeCallback callback)
        {
            NativeAdContainer container = NativeAdBridgeIOS.nativeAdContainerForNativeAdId(uniqueId);
            if (container) {
                container.onFinishedClick = callback;
            }
        }

        public override void OnMediaDownloaded(int uniqueId, FBNativeAdBridgeCallback callback)
        {
            NativeAdContainer container = NativeAdBridgeIOS.nativeAdContainerForNativeAdId(uniqueId);
            if (container) {
                container.onMediaDownload = callback;
            }
        }

        // External unmanaged callbacks (must be static)

        [MonoPInvokeCallback(typeof(FBNativeAdBridgeExternalCallback))]
        private static void nativeAdDidLoadBridgeCallback(int uniqueId)
        {
            NativeAdContainer container = NativeAdBridgeIOS.nativeAdContainerForNativeAdId(uniqueId);
            if (container && container.onLoad != null) {
                container.onLoad();
            }
        }

        [MonoPInvokeCallback(typeof(FBNativeAdBridgeExternalCallback))]
        private static void nativeAdWillLogImpressionmpressionBridgeCallback(int uniqueId)
        {
            NativeAdContainer container = NativeAdBridgeIOS.nativeAdContainerForNativeAdId(uniqueId);
            if (container && container.onImpression != null) {
                container.onImpression();
            }
        }

        [MonoPInvokeCallback(typeof(FBNativeAdBridgeErrorExternalCallback))]
        private static void nativeAdDidFailWithErrorBridgeCallback(int uniqueId, string error)
        {
            NativeAdContainer container = NativeAdBridgeIOS.nativeAdContainerForNativeAdId(uniqueId);
            if (container && container.onError != null) {
                container.onError(error);
            }
        }

        [MonoPInvokeCallback(typeof(FBNativeAdBridgeExternalCallback))]
        private static void nativeAdDidClickBridgeCallback(int uniqueId)
        {
            NativeAdContainer container = NativeAdBridgeIOS.nativeAdContainerForNativeAdId(uniqueId);
            if (container && container.onClick != null) {
                container.onClick();
            }
        }

        [MonoPInvokeCallback(typeof(FBNativeAdBridgeExternalCallback))]
        private static void nativeAdDidFinishHandlingClickBridgeCallback(int uniqueId)
        {
            NativeAdContainer container = NativeAdBridgeIOS.nativeAdContainerForNativeAdId(uniqueId);
            if (container && container.onFinishedClick != null) {
                container.onFinishedClick();
            }
        }

        [MonoPInvokeCallback(typeof(FBNativeAdBridgeExternalCallback))]
        private static void nativeAdDidDownloadMediaBridgeCallback(int uniqueId)
        {
            NativeAdContainer container = NativeAdBridgeIOS.nativeAdContainerForNativeAdId(uniqueId);
            if (container && container.onMediaDownload != null) {
                container.onMediaDownload();
            }
        }

    }
#endif

    internal class NativeAdContainer
    {
        internal NativeAdBase nativeAd { get; set; }

        // iOS
        internal FBNativeAdBridgeCallback onLoad { get; set; }

        internal FBNativeAdBridgeCallback onImpression { get; set; }

        internal FBNativeAdBridgeCallback onClick { get; set; }

        internal FBNativeAdBridgeErrorCallback onError { get; set; }

        internal FBNativeAdBridgeCallback onFinishedClick { get; set; }

        internal FBNativeAdBridgeCallback onMediaDownload { get; set; }

        // Android
#if UNITY_ANDROID
        internal AndroidJavaProxy listenerProxy;
        internal AndroidJavaObject bridgedNativeAd;
        internal AndroidJavaObject context;
        internal AndroidJavaObject mediaView;
        internal AndroidJavaObject iconView;
        internal AndroidJavaObject ctaView;
#endif

        internal NativeAdContainer(NativeAdBase nativeAd)
        {
            this.nativeAd = nativeAd;
        }

        public static implicit operator bool (NativeAdContainer obj)
        {
            return !(object.ReferenceEquals(obj, null));
        }
    }

    internal class NativeAdBridgeListenerProxy : AndroidJavaProxy
    {
        private NativeAdBase nativeAd;
        protected AndroidJavaObject bridgedNativeAd;

        public NativeAdBridgeListenerProxy(NativeAdBase nativeAd, AndroidJavaObject bridgedNativeAd)
            : base("com.facebook.ads.NativeAdListener")
        {
            this.nativeAd = nativeAd;
            this.bridgedNativeAd = bridgedNativeAd;
        }

        void onError(AndroidJavaObject ad, AndroidJavaObject error)
        {
            string errorMessage = error.Call<string> ("getErrorMessage");
            this.nativeAd.executeOnMainThread(() => {
                if (nativeAd.NativeAdDidFailWithError != null) {
                    nativeAd.NativeAdDidFailWithError(errorMessage);
                }
            });
        }

        void onAdLoaded(AndroidJavaObject ad)
        {
            this.nativeAd.executeOnMainThread(() => {
                nativeAd.loadAdFromData();
            });
        }

        void onAdClicked(AndroidJavaObject ad)
        {
            this.nativeAd.executeOnMainThread(() => {
                if (nativeAd.NativeAdDidClick != null) {
                    nativeAd.NativeAdDidClick();
                }
            });
        }

        void onLoggingImpression(AndroidJavaObject ad)
        {
            this.nativeAd.executeOnMainThread(() => {
                if (nativeAd.NativeAdWillLogImpression != null) {
                    nativeAd.NativeAdWillLogImpression();
                }
            });
        }

        void onMediaDownloaded(AndroidJavaObject ad)
        {
            this.nativeAd.executeOnMainThread(() => {
                if (nativeAd.NativeAdDidDownloadMedia != null) {
                    nativeAd.NativeAdDidDownloadMedia();
                }
            });
        }
    }
}

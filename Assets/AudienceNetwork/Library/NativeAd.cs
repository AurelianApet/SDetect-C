//#define UNITY_ANDROID
//#define UNITY_IOS
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
    public sealed class NativeAd : NativeAdBase, IDisposable
    {
        public NativeAd(string placementId) : base(placementId)
        {
            nativeAdType = NativeAdType.NativeAd;
            uniqueId = NativeAdBridgeInstance().Create(placementId, this);
        }

        public int RegisterGameObjectsForInteraction(RectTransform mediaViewRectTransform, RectTransform ctaRectTransform,
                RectTransform iconViewRectTransform = null, Camera camera = null)
        {
            return baseRegisterGameObjectsForInteraction(mediaViewRectTransform, ctaRectTransform,
                    iconViewRectTransform, camera);
        }
    }
}

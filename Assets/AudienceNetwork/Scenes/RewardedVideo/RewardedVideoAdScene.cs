using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using AudienceNetwork;
using UnityEngine.SceneManagement;

public class RewardedVideoAdScene : MonoBehaviour
{

    private RewardedVideoAd rewardedVideoAd;
    private bool isLoaded;
#pragma warning disable 0414
    private bool didClose;
#pragma warning restore 0414

    // UI elements in scene
    public Text statusLabel;

    // Load button
    public void LoadRewardedVideo()
    {
        this.statusLabel.text = "Loading rewardedVideo ad...";

        // Create the rewarded video unit with a placement ID (generate your own on the Facebook app settings).
        // Use different ID for each ad placement in your app.
        this.rewardedVideoAd = new RewardedVideoAd("YOUR_PLACEMENT_ID");

        // For S2S validation you can create the rewarded video ad with the reward data
        // Refer to documentation here:
        // https://developers.facebook.com/docs/audience-network/android/rewarded-video#server-side-reward-validation
        // https://developers.facebook.com/docs/audience-network/ios/rewarded-video#server-side-reward-validation
        RewardData rewardData = new RewardData();
        rewardData.UserId = "USER_ID";
        rewardData.Currency = "REWARD_ID";
#pragma warning disable 0219
        RewardedVideoAd s2sRewardedVideoAd = new RewardedVideoAd("YOUR_PLACEMENT_ID", rewardData);
#pragma warning restore 0219

        this.rewardedVideoAd.Register(this.gameObject);

        // Set delegates to get notified on changes or when the user interacts with the ad.
        this.rewardedVideoAd.RewardedVideoAdDidLoad = (delegate ()
        {
            Debug.Log("RewardedVideo ad loaded.");
            this.isLoaded = true;
            this.didClose = false;
            this.statusLabel.text = "Ad loaded. Click show to present!";
        });
        this.rewardedVideoAd.RewardedVideoAdDidFailWithError = (delegate (string error)
        {
            Debug.Log("RewardedVideo ad failed to load with error: " + error);
            this.statusLabel.text = "RewardedVideo ad failed to load. Check console for details.";
        });
        this.rewardedVideoAd.RewardedVideoAdWillLogImpression = (delegate ()
        {
            Debug.Log("RewardedVideo ad logged impression.");
        });
        this.rewardedVideoAd.RewardedVideoAdDidClick = (delegate ()
        {
            Debug.Log("RewardedVideo ad clicked.");
        });

        // For S2S validation you need to register the following two callback
        // Refer to documentation here:
        // https://developers.facebook.com/docs/audience-network/android/rewarded-video#server-side-reward-validation
        // https://developers.facebook.com/docs/audience-network/ios/rewarded-video#server-side-reward-validation
        this.rewardedVideoAd.RewardedVideoAdDidSucceed = (delegate ()
        {
            Debug.Log("Rewarded video ad validated by server");
        });

        this.rewardedVideoAd.RewardedVideoAdDidFail = (delegate ()
        {
            Debug.Log("Rewarded video ad not validated, or no response from server");
        });

        this.rewardedVideoAd.RewardedVideoAdDidClose = (delegate ()
        {
            Debug.Log("Rewarded video ad did close.");
            this.didClose = true;
            if (this.rewardedVideoAd != null)
            {
                this.rewardedVideoAd.Dispose();
            }
        });

#if UNITY_ANDROID
        /*
         * Only relevant to Android.
         * This callback will only be triggered if the Rewarded Video activity
         * has been destroyed without being properly closed. This can happen if
         * an app with launchMode:singleTask (such as a Unity game) goes to
         * background and is then relaunched by tapping the icon.
         */
        this.rewardedVideoAd.rewardedVideoAdActivityDestroyed = (delegate ()
        {
            if (!this.didClose)
            {
                Debug.Log("Rewarded video activity destroyed without being closed first.");
                Debug.Log("Game should resume. User should not get a reward.");
            }
        });
#endif

        // Initiate the request to load the ad.
        this.rewardedVideoAd.LoadAd();
    }

    // Show button
    public void ShowRewardedVideo()
    {
        if (this.isLoaded)
        {
            this.rewardedVideoAd.Show();
            this.isLoaded = false;
            this.statusLabel.text = "";
        }
        else
        {
            this.statusLabel.text = "Ad not loaded. Click load to request an ad.";
        }
    }

    void OnDestroy()
    {
        // Dispose of rewardedVideo ad when the scene is destroyed
        if (this.rewardedVideoAd != null)
        {
            this.rewardedVideoAd.Dispose();
        }
        Debug.Log("RewardedVideoAdTest was destroyed!");
    }

    // Next button
    public void NextScene()
    {
        SceneManager.LoadScene("NativeAdScene");
    }
}

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using AudienceNetwork;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AdViewScene : MonoBehaviour
{

    private AdView adView;
    private AdPosition currentAdViewPosition;
    private ScreenOrientation currentScreenOrientation;
    public Text statusLabel;

    void OnDestroy()
    {
        // Dispose of banner ad when the scene is destroyed
        if (this.adView) {
            this.adView.Dispose();
        }
        Debug.Log("AdViewTest was destroyed!");
    }

    // Load Banner button
    public void LoadBanner()
    {
        if (this.adView) {
            this.adView.Dispose();
        }

        this.statusLabel.text = "Loading Banner...";

        // Create a banner's ad view with a unique placement ID (generate your own on the Facebook app settings).
        // Use different ID for each ad placement in your app.
        this.adView = new AdView("YOUR_PLACEMENT_ID", AdSize.BANNER_HEIGHT_50);

        this.adView.Register(this.gameObject);
        this.currentAdViewPosition = AdPosition.CUSTOM;

        // Set delegates to get notified on changes or when the user interacts with the ad.
        this.adView.AdViewDidLoad = (delegate() {
            this.currentScreenOrientation = Screen.orientation;
            Debug.Log("Banner loaded.");
            this.adView.Show(100);
            this.statusLabel.text = "Banner loaded";
        });
        adView.AdViewDidFailWithError = (delegate(string error) {
            Debug.Log("Banner failed to load with error: " + error);
            this.statusLabel.text = "Banner failed to load with error: " + error;
        });
        adView.AdViewWillLogImpression = (delegate() {
            Debug.Log("Banner logged impression.");
            this.statusLabel.text = "Banner logged impression.";
        });
        adView.AdViewDidClick = (delegate() {
            Debug.Log("Banner clicked.");
            this.statusLabel.text = "Banner clicked.";
        });

        // Initiate a request to load an ad.
        adView.LoadAd();
    }

    // Next button
    public void NextScene()
    {
        SceneManager.LoadScene("RewardedVideoAdScene");
    }

    // Change button
    // Change the position of the ad view when button is clicked
    // ad view is at top: move it to bottom
    // ad view is at bottom: move it to 100 pixels along y-axis
    // ad view is at custom position: move it to the top
    public void ChangePosition()
    {
        switch (this.currentAdViewPosition) {
        case AdPosition.TOP:
            this.setAdViewPosition(AdPosition.BOTTOM);
            break;
        case AdPosition.BOTTOM:
            this.setAdViewPosition(AdPosition.CUSTOM);
            break;
        case AdPosition.CUSTOM:
            this.setAdViewPosition(AdPosition.TOP);
            break;
        }
    }

    private void OnRectTransformDimensionsChange()
    {
        if (this.adView && Screen.orientation != this.currentScreenOrientation)
        {
            setAdViewPosition(currentAdViewPosition);
            this.currentScreenOrientation = Screen.orientation;
        }
    }

    private void setAdViewPosition(AdPosition adPosition)
    {
        switch (adPosition) {
        case AdPosition.TOP:
            this.adView.Show(AdPosition.TOP);
            this.currentAdViewPosition = AdPosition.TOP;
            break;
        case AdPosition.BOTTOM:
            this.adView.Show(AdPosition.BOTTOM);
            this.currentAdViewPosition = AdPosition.BOTTOM;
            break;
        case AdPosition.CUSTOM:
            this.adView.Show(100);
            this.currentAdViewPosition = AdPosition.CUSTOM;
            break;
        }
    }
}

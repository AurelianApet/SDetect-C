using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CaptureCamera : MonoBehaviour {
    static WebCamTexture _webCam;
    public new Camera camera;
    public int resWidth = Screen.width * 2;
    public int resHeight = Screen.height * 2;
    bool flag = false;
    // Use this for initialization
    void Start () {
        if (_webCam == null)
        {
            _webCam = new WebCamTexture();
        }
        GetComponent<Renderer>().material.mainTexture = _webCam;
        if (!_webCam.isPlaying)
            _webCam.Play();
        resHeight = (int)(Screen.width / 1080.0f * 1920.0f) * 2;
        StartCoroutine(setFlag());
    }
	
    IEnumerator setFlag()
    {
        yield return new WaitForSeconds(1.0f);
        flag = true;
    }

	// Update is called once per frame
	void Update () {
        if (!Global.isProcess && flag)
        {
            RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
            camera.targetTexture = rt;
            Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
            camera.Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
            camera.targetTexture = null;
            RenderTexture.active = null; // JC: added to avoid errors
            Destroy(rt);
            byte[] bytes = screenShot.EncodeToPNG();
            Global.imageContent = Convert.ToBase64String(bytes);
            //System.IO.File.WriteAllBytes("test" + i + ".png", bytes);
            Debug.Log("capture!");
            Global.isProcess = true;
        }
    }
}

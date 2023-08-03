using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using LitJson;
using Facebook;
using Facebook.Unity;
using System.Collections.Generic;
using SimpleJSON;
using System;

public class Login : MonoBehaviour {
    public InputField email;
    public InputField pwd;
    // Use this for initialization
    private void Awake()
    {
        Screen.SetResolution(Screen.width, (int)(Screen.width / Global.ratio), true);
        if (!FB.IsInitialized)
        {
            // Initialize the Facebook SDK
            FB.Init(InitCallback, OnHideUnity);
        }
        else
        {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
        }
    }

    private void InitCallback()
    {
        if (FB.IsInitialized)
        {
            // Signal an app activation App Event
            FB.ActivateApp();
            // Continue with Facebook SDK
            // ..
        }
        else
        {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }

    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else
        {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }
    
    void Start () {
        Global.curScene = "History";
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void login()
    {
        string email_text = email.text;
        string password = pwd.text;
        if(email_text == "")
        {
            this.transform.parent.Find("MsgBox").gameObject.SetActive(true);
            this.transform.parent.Find("MsgBox/Back/Msg").GetComponent<Text>().text = "Please input email address.";
            pwd.text = "";
        }
        else if(password == "")
        {
            this.transform.parent.Find("MsgBox").gameObject.SetActive(true);
            this.transform.parent.Find("MsgBox/Back/Msg").GetComponent<Text>().text = "Please input password.";
            email.text = "";
        }
        else
        {
            StartCoroutine(is_login(email_text, password));
        }
    }

    IEnumerator is_login(string email, string pwd)
    {
        WWWForm form = new WWWForm();
        form.AddField("email", email);
        form.AddField("pwd", pwd);
        form.AddField("logo", Global.SLogo[0].logo_name);
        form.AddField("type", 1);
        string requestURL = Global.DOMAIN + Global.login_save_api;
        WWW www = new WWW(requestURL, form);
        yield return process_login(www);
    }

    IEnumerator process_login(WWW www)
    {
        yield return www;
        if (www.error == null)
        {
            JSONNode jsonNode = SimpleJSON.JSON.Parse(www.text);
            if (jsonNode["success"].ToString() == "1")
            {
                string id = jsonNode["uid"].ToString();
                id = id.Replace("\"", "");
                Global.uid = int.Parse(id);
                Global.username = jsonNode["uname"].ToString();
                Debug.Log("login name = " + Global.username);
                Global.history = jsonNode["history"].ToString().Split(',');
                Global.curScene = "Login";
                SceneManager.LoadScene("History");
            }
            else if (jsonNode["success"].ToString() == "2")
            {
                this.transform.parent.Find("MsgBox").gameObject.SetActive(true);
                this.transform.parent.Find("MsgBox/Back/Msg").GetComponent<Text>().text = "Setting history failed.";
                email.text = "";
                pwd.text = "";
            }
            else
            {
                this.transform.parent.Find("MsgBox").gameObject.SetActive(true);
                this.transform.parent.Find("MsgBox/Back/Msg").GetComponent<Text>().text = "Login failed. Please try again.";
                email.text = "";
                pwd.text = "";
            }
        }
    }

    public void FacebookLogin()
    {
        Debug.Log("---click facebook login---");
        //FB.LogInWithPublishPermissions(new List<string>() { "public_profile", "email", "user_friends" }, AuthCallback);
        FB.LogInWithReadPermissions(new List<string>() { "public_profile", "email", "user_friends" }, AuthCallback);
    }

    private void AuthCallback(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            // AccessToken class will have session details
            var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
            // Print current access token's User ID
            Debug.Log(aToken.UserId);
            // Print current access token's granted permissions
            //foreach (string perm in aToken.Permissions)
            //{
            //    user_birthday
            //    user_location
            //    user_friends
            //    email
            //    public_profile
            //}
            FB.API("me?fields=id,name,email", Facebook.Unity.HttpMethod.GET, this.GetFacebookUsername);
        }
        else
        {
            Debug.Log("User cancelled login");
        }
    }

    protected void GetFacebookUsername(IResult result)
    {
        IDictionary dict = Facebook.MiniJSON.Json.Deserialize(result.RawResult) as IDictionary;
        string fbname = dict["name"].ToString();
        //string fbUID = dict["id"].ToString();
        string fbemail = dict["email"].ToString();
        Global.username = fbname;
        Debug.Log("Login name = " + fbname);
        StartCoroutine(fblogin_process(fbname, fbemail));
    }

    IEnumerator fblogin_process(string username, string fbemail)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("email", fbemail);
        form.AddField("type", 2);
        form.AddField("logo", Global.SLogo[0].logo_name);
        string requestURL = Global.DOMAIN + Global.login_save_api;
        WWW www = new WWW(requestURL, form);
        yield return www;
        if (www.error == null)
        {
            JSONNode jsonNode = SimpleJSON.JSON.Parse(www.text);
            if (jsonNode["success"].ToString() == "1" || jsonNode["success"].ToString() == "2")
            {
                Global.history = jsonNode["history"].ToString().Split(',');
                Global.curScene = "Login";
                SceneManager.LoadScene("History");
            }
            else
            {
                this.transform.parent.Find("MsgBox").gameObject.SetActive(true);
                this.transform.parent.Find("MsgBox/Back/Msg").GetComponent<Text>().text = "Login failed. Please try again.";
            }
        }
    }
}

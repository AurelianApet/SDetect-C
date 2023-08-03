using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using LitJson;

public class ProcesButton : MonoBehaviour {
    // Use this for initialization
    void Start () {
    }

    // Update is called once per frame
    void Update () {
	
	}

    public void onMenu()
    {
        SceneManager.LoadScene("History");
    }

    public void onReset()
    {
        Global.is_reset = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //GameObject box = GameObject.Find("UI/Box").gameObject;
        //box.transform.localPosition = new Vector3(box.transform.localPosition.x, -1410.0f, box.transform.localPosition.z);
        //box.GetComponent<Animator>().enabled = false;
    }

    public void onCancel()
	{
		this.gameObject.transform.parent.gameObject.SetActive (false);
	}

	public void onFacebook()
	{
		switch (Global.SLogo[0].logo_name.ToLower()) {
        case "pepsi":
			Application.OpenURL ("https://www.facebook.com/PepsiCanada/?brand_redir=339150749455906");
			break;
		case "nike":
			Application.OpenURL ("https://www.facebook.com/nike");
			break;
        case "mercede":
			Application.OpenURL ("https://www.facebook.com/MercedesBenz");
			break;
        case "macdonald":
			Application.OpenURL ("https://www.facebook.com/McDonalds/");
			break;
        case "apple":
			Application.OpenURL ("https://www.facebook.com/apple");
			break;
        default:
                Application.OpenURL("https://www.facebook.com/");
            break;
		}
	}

	public void onWebsite()
	{
		switch (Global.SLogo[0].logo_name.ToLower()) {
		case "pepsi":
			Application.OpenURL ("https://www.pepsi.ca/");
			break;
		case "nike":
			Application.OpenURL ("https://www.nike.com/ca/");
			break;
		case "mercede":
			Application.OpenURL ("https://www.mercedes-benz.com/en/");
			break;
		case "macdonald":
			Application.OpenURL ("https://www.mcdonalds.com");
			break;
		case "apple":
			Application.OpenURL ("https://www.apple.com/");
			break;
        default:
            Application.OpenURL("https://www.logo.com/");
            break;
        }
    }

	public void onInstagram()
	{
		switch (Global.SLogo[0].logo_name.ToLower()) {
		case "pepsi":
			Application.OpenURL ("https://www.instagram.com/pepsi/");
			break;
		case "nike":
			Application.OpenURL ("https://www.instagram.com/nike/");
			break;
		case "mercede":
			Application.OpenURL ("https://www.instagram.com/mercedesbenz/");
			break;
		case "macdonald":
			Application.OpenURL ("https://www.instagram.com/McDonalds/");
			break;
		case "apple":
			Application.OpenURL ("https://www.instagram.com/apple/");
			break;
        default:
            Application.OpenURL("https://www.instagram.com/");
            break;
        }
    }

	public void onSnapchat()
	{
		switch (Global.SLogo[0].logo_name.ToLower()) {
		case "nike":
			Application.OpenURL ("https://www.snapchat.com/add/nike");
			break;
		case "macdonald":
			Application.OpenURL ("https://www.snapchat.com/add/mcdonalds.at");
			break;
        default:
            Application.OpenURL("https://www.snapchat.com/");
            break;
        }
    }

	public void onTwitter()
	{
		switch (Global.SLogo[0].logo_name.ToLower()) {
		case "pepsi":
			Application.OpenURL ("https://twitter.com/pepsicanada");
			break;
		case "nike":
			Application.OpenURL ("https://twitter.com/nike");
			break;
		case "mercede":
			Application.OpenURL ("https://twitter.com/mercedesbenz");
			break;
		case "macdonald":
			Application.OpenURL ("https://twitter.com/McDonalds");
			break;
		case "apple":
			Application.OpenURL ("https://twitter.com/Apple");
			break;
        default:
            Application.OpenURL("https://twitter.com/");
            break;
        }
    }

	public void onYoutube()
	{
		switch (Global.SLogo[0].logo_name.ToLower()) {
		case "pepsi":
			Application.OpenURL ("https://www.youtube.com/user/Pepsi");
			break;
		case "nike":
			Application.OpenURL ("https://www.youtube.com/user/nike");
			break;
		case "mercede":
			Application.OpenURL ("https://www.youtube.com/user/MercedesBenzTV");
			break;
		case "macdonald":
			Application.OpenURL ("https://www.youtube.com/c/McDonalds");
			break;
		case "apple":
			Application.OpenURL ("https://www.youtube.com/user/Apple");
			break;
        default:
            Application.OpenURL("https://www.youtube.com/");
            break;
        }
    }

	public void onSpotify()
	{
		switch (Global.SLogo[0].logo_name.ToLower()) {
		case "macdonald":
			Application.OpenURL ("https://open.spotify.com/user/mcdonalds");
			break;
        default:
            Application.OpenURL("https://open.spotify.com/");
            break;
        }
    }

	public void onLinkedin()
	{
		switch (Global.SLogo[0].logo_name.ToLower()) {
		case "mercede":
			Application.OpenURL ("https://www.linkedin.com/company/daimler");
			break;
        default:
            Application.OpenURL("https://www.linkedin.com/");
            break;
        }
    }

	public void oniTunes(){
		switch (Global.SLogo[0].logo_name.ToLower()) {
		case "apple":
			Application.OpenURL ("https://www.apple.com/itunes/");
			break;
        default:
            Application.OpenURL("https://www.apple.com/");
            break;
        }
    }

	public void onLogo(){
		Application.OpenURL ("https://www.upwork.com/");
	}

	public void onSale(){
		Application.OpenURL ("https://www.groupon.com/");
	}

	public void onPhone()
	{
		string telUrl = "tel:111-111-111";
		Application.OpenURL(telUrl);
	}

	public void email()
	{
		string email = "me@example.com";
		string subject = MyEscapeURL("My Subject");
		string body = MyEscapeURL("My Body\r\nFull of non-escaped chars");
		Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);
	}

	string MyEscapeURL (string url)
	{
		return WWW.EscapeURL(url).Replace("+","%20");
	}

    public void GotoLogin()
    {
        SceneManager.LoadScene("Login");
    }

    public void GotoSignup()
    {
        Global.curScene = "Login";
        StartCoroutine(load_signup());
    }

    IEnumerator load_signup()
    {
        AsyncOperation AO = SceneManager.LoadSceneAsync("Register", LoadSceneMode.Additive);
        AO.allowSceneActivation = false;
        while (AO.progress < 0.9f)
        {
            yield return null;
        }

        //Fade the loading screen out here

        AO.allowSceneActivation = true;
    }

    public void DelHis()
    {
        string logo = this.transform.parent.Find("Text").GetComponent<Text>().text;
        //uid, logo
        StartCoroutine(delete_history(logo));
    }

    IEnumerator delete_history(string logo)
    {
        Debug.Log("uid=" + Global.uid);
        WWWForm form = new WWWForm();
        form.AddField("logo", logo);
        form.AddField("uid", Global.uid);
        string requestURL = Global.DOMAIN + Global.delHistory_api;
        WWW www = new WWW(requestURL, form);
        yield return www;
        if (www.error == null)
        {
            JsonData result = JsonMapper.ToObject(www.text);
            if (result.Count > 0)
            {
                if (result["success"].ToString() == "1")
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
                else
                {
                }
            }
        }
    }

    public void hideMsgBox()
    {
        this.gameObject.SetActive(false);
    }

    public void gotoBack()
    {
        SceneManager.LoadScene(Global.curScene);
    }
}

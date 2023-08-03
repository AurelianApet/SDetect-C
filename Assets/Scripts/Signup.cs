using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using LitJson;

public class Signup : MonoBehaviour {
    public InputField firstname;
    public InputField lastname;
    public InputField email;
    public InputField pwd;
	// Use this for initialization
	void Start () {
        Screen.SetResolution(Screen.width, (int)(Screen.width / Global.ratio), true);
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void register()
    {
        string fname = firstname.text;
        string lname = lastname.text;
        string email_text = email.text;
        string password = pwd.text;
        if(fname == "")
        {
            this.transform.parent.Find("MsgBox").gameObject.SetActive(true);
            this.transform.parent.Find("MsgBox/Back/Msg").GetComponent<Text>().text = "Please input first name.";
        }else if(lname == "")
        {
            this.transform.parent.Find("MsgBox").gameObject.SetActive(true);
            this.transform.parent.Find("MsgBox/Back/Msg").GetComponent<Text>().text = "Please input last name.";
        }
        else if(email_text == "")
        {
            this.transform.parent.Find("MsgBox").gameObject.SetActive(true);
            this.transform.parent.Find("MsgBox/Back/Msg").GetComponent<Text>().text = "Please input email address.";
        }
        else if(password == "")
        {
            this.transform.parent.Find("MsgBox").gameObject.SetActive(true);
            this.transform.parent.Find("MsgBox/Back/Msg").GetComponent<Text>().text = "Please input password.";
        }
        else
        {
            StartCoroutine(signup(fname, lname, email_text, password));
        }
    }

    IEnumerator signup(string fname, string lname, string email, string pwd)
    {
        WWWForm form = new WWWForm();
        form.AddField("fname", fname);
        form.AddField("lname", lname);
        form.AddField("email", email);
        form.AddField("pwd", pwd);
        form.AddField("type", 1);
        string requestURL = Global.DOMAIN + Global.signup_api;
        WWW www = new WWW(requestURL, form);
        yield return process_signup(www);
    }

    IEnumerator process_signup(WWW www)
    {
        yield return www;
        if (www.error == null)
        {
            JsonData result = JsonMapper.ToObject(www.text);
            if(result.Count > 0)
            {
                if (result["success"].ToString() == "1")
                {
                    Global.curScene = "Register";
                    SceneManager.LoadScene("Login");
                }
                else if(result["success"].ToString() == "2")
                {
                    this.transform.parent.Find("MsgBox").gameObject.SetActive(true);
                    this.transform.parent.Find("MsgBox/Back/Msg").GetComponent<Text>().text = "Sorry.. email already exists.";
                    //already exist
                    firstname.text = "";
                    lastname.text = "";
                    email.text = "";
                    pwd.text = "";
                }
                else
                {
                    this.transform.parent.Find("MsgBox").gameObject.SetActive(true);
                    this.transform.parent.Find("MsgBox/Back/Msg").GetComponent<Text>().text = "Signup failed. Please try again.";
                    //signup failed
                    firstname.text = "";
                    lastname.text = "";
                    email.text = "";
                    pwd.text = "";
                }
            }
        }
    }
}

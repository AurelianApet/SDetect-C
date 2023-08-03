using System.Collections;
using UnityEngine;
using LitJson;
using UnityEngine.UI;
using System.IO;
using System;
using SimpleJSON;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Text;

public class Detect : MonoBehaviour
{
    public GameObject buttons;
    public Text logo;
    public GameObject loadingEffect;
    public GameObject detectBtn;
    GameObject box;

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    delegate string ProcessPostData(string data);

    [DllImport("LOGODETECTOR", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr fnLogoDetector(StringBuilder imgBase64, StringBuilder googleAPIKey,
            [MarshalAs(UnmanagedType.FunctionPtr)] ProcessPostData pPostData);

    private void Awake()
    {
        Global.ratio = 1080.0f / 1920.0f;
        Screen.SetResolution(Screen.width, (int)(Screen.width / Global.ratio), true);
        buttons.transform.Find("Box").GetComponent<Animator>().enabled = false;
        Global.isProcess = false;
        Global.imageContent = "";
        Global.is_reset = true;
        box = buttons.transform.Find("Box").gameObject;
    }
    // Use this for initialization

    void Start()
    {
        StartCoroutine(is_detect());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator is_detect()
    {
        yield return new WaitForSeconds(1.0f);
        while (true)
        {
            while (!Global.isProcess || !Global.is_reset)
            {
                yield return new WaitForSeconds(0.1f);
            }
            if (Global.imageContent != "")
            {
                StringBuilder key = new StringBuilder(Global.google_key);
                StringBuilder img64 = new StringBuilder(Global.imageContent);
                //following is a callback function created which will be used by C++
                ProcessPostData calllback =
                (value) =>
                {
                    Debug.Log("detect_result" + value);
                    //the output returned by dll is arrived here
                   //the result is in the "value" fire the focus event here 
                   //result will come asynchronously here after a very shrot time when google will send it back so 
                   //all processing for the result must be done here and also the loop should not create so many call back functions
                   //it can create heap leaks. I hope you understand now
                    return "";
                };
   
                //extract the return message from the dll normal is "success" others are error messages
                
                IntPtr retcode = fnLogoDetector(img64, key, calllback);
                string retmsg = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(retcode);
                Debug.Log("detect_status" + retmsg);

               //Success only tells that the google had  now issue its asynchroous so the result will come late and above.
                if(retmsg == "Success")
                {
                    Debug.Log("---Found---");
                    Global.is_reset = false;
                }
                else
                {
                    Global.isProcess = false;
                }
            }
        }
    }

    IEnumerator WaitForRequest(WWW data)
    {
        yield return data; // Wait until the download is done
        loadingEffect.SetActive(false);
        if (data.error == null)
        {
            JSONNode jsonNode = SimpleJSON.JSON.Parse(data.text);
            if (jsonNode["responses"].ToString().Trim() == "[{}]")
            {
                Debug.Log("responses null");
                yield break;
            }
            Debug.Log("response=" + data.text);
            Global.SLogo = new List<sLogo>();
            //GameObject obj = Instantiate(detectBtn, new Vector3(0, 0, 0), Quaternion.identity);
            //obj.transform.parent = UIParent;
            GameObject obj = buttons.transform.Find("FocusBtn").gameObject;
            if (jsonNode["responses"][0]["logoAnnotations"][0]["description"] != null)
            {
                Debug.Log("---Found---");
                Global.is_reset = false;
                obj.SetActive(true);
                Global.SLogo.Add(new sLogo(jsonNode["responses"][0]["logoAnnotations"][0]["description"], "", 0.0f, 0.0f));
                StartCoroutine(stickerEffect(obj, box));
                OnFound();
            } else if (chkLogo(jsonNode))
            {
                Debug.Log("---Found---");
                Global.is_reset = false;
                obj.SetActive(true);
                StartCoroutine(stickerEffect(obj, box));
                OnFound();
            }
            else
            {
                Global.isProcess = false;
            }
        }
        else
        {
            OnLost();
            Global.isProcess = false;
        }
    }

    private bool chkLogo(JSONNode jsonNode)
    {
        bool detected = false;
        string can = jsonNode["responses"][0]["webDetection"][0]["bestGuessLabels"][0]["label"].ToString().Replace(" ", "");
        can = Regex.Replace(can, "[^\\w\\._]", "");
        if (can != "null" || can != "")
            return detected;
        for (int i = 0; i < Global.slogos.Length; i++)
        {
            if (Global.slogos[i].IndexOf(can) > -1)
            {
                detected = true;
                Debug.Log("logo detect!");
                Global.SLogo.Add(new sLogo(Global.plogos[i], "", 0.0f, 0.0f)); break;
            }
        }

        if (!detected)
        {
            for (int i = 0; i < jsonNode["responses"][0]["webDetection"][0]["webEntities"].Count; i++)
            {
                string desc = jsonNode["responses"][0]["webDetection"][0]["webEntities"][i]["description"];
                string score = jsonNode["responses"][0]["webDetection"][0]["webEntities"][i]["score"];
                if(float.Parse(score) > 0.5f)
                {
                    for (int j = 0; j < Global.slogos.Length; j++)
                    {
                        if (Global.slogos[j].IndexOf(desc) > -1)
                        {
                            detected = true;
                            Debug.Log("rest logo detect!");
                            Global.SLogo.Add(new sLogo(Global.plogos[i], "", 0.0f, 0.0f)); break;
                        }
                    }
                }
            }
        }
        return detected;
    }

    protected void OnFound()
    {
        switch (Global.SLogo[0].logo_name.ToLower())
        {
            case "pepsi":
                {
                    box.transform.Find("snapchat").gameObject.SetActive(false);
                    box.transform.Find("itunes").gameObject.SetActive(false);
                    box.transform.Find("spotify").gameObject.SetActive(false);
                    box.transform.Find("linkedin").gameObject.SetActive(false);
                    break;
                }
            case "nike":
                {
                    box.transform.Find("snapchat").gameObject.SetActive(true);
                    box.transform.Find("itunes").gameObject.SetActive(false);
                    box.transform.Find("spotify").gameObject.SetActive(false);
                    box.transform.Find("linkedin").gameObject.SetActive(false);
                    break;
                }
            case "mercede":
                {
                    box.transform.Find("snapchat").gameObject.SetActive(false);
                    box.transform.Find("itunes").gameObject.SetActive(false);
                    box.transform.Find("spotify").gameObject.SetActive(false);
                    box.transform.Find("linkedin").gameObject.SetActive(true);
                    break;
                }
            case "macdonald":
                {
                    box.transform.Find("snapchat").gameObject.SetActive(true);
                    box.transform.Find("itunes").gameObject.SetActive(false);
                    box.transform.Find("spotify").gameObject.SetActive(true);
                    box.transform.Find("linkedin").gameObject.SetActive(false);
                    break;
                }
            case "apple":
                {
                    box.transform.Find("snapchat").gameObject.SetActive(false);
                    box.transform.Find("itunes").gameObject.SetActive(true);
                    box.transform.Find("spotify").gameObject.SetActive(false);
                    box.transform.Find("linkedin").gameObject.SetActive(false);
                    break;
                }
            default:
                {
                    box.transform.Find("snapchat").gameObject.SetActive(false);
                    box.transform.Find("itunes").gameObject.SetActive(false);
                    box.transform.Find("spotify").gameObject.SetActive(false);
                    box.transform.Find("linkedin").gameObject.SetActive(false);
                    break;
                }
        }
        logo.text = Global.SLogo[0].logo_name;
    }

    protected void OnLost()
    {
        //buttons.SetActive (false);
    }

    IEnumerator stickerEffect(GameObject obj, GameObject box)
    {
        float delta = 0.0f;
        while (delta < 1.0f)
        {
            delta += 0.1f;
            if (obj != null && obj.transform != null) obj.transform.localScale += new Vector3(0.03f, 0.03f, 0.03f);
            yield return new WaitForSeconds(0.01f);
        }
        while (delta > 0.0f)
        {
            delta -= 0.1f;
            if (obj != null && obj.transform != null) obj.transform.localScale -= new Vector3(0.03f, 0.03f, 0.03f);
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(0.5f);
        obj.SetActive(false);
        buttons.transform.Find("Box").GetComponent<Animator>().enabled = true;
    }
}

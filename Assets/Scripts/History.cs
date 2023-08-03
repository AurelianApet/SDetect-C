using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class History : MonoBehaviour {
    public Text username;
    public GameObject record;
    public Transform scrollview_parent;
	// Use this for initialization
	void Start () {
        Screen.SetResolution(Screen.width, (int)(Screen.width / Global.ratio), true);
        Global.curScene = "Main";
        username.text = Global.username;
        LoadHistory();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void LoadHistory()
    {
        if (Global.history != null)
        {
            scrollview_parent.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(27, 100 + Global.history.Length * 110);
            for (int i = 0; i < Global.history.Length; i++)
            {
                GameObject new_record = (GameObject)Instantiate(record, new Vector3(532, 1515 - i * 110, 0), Quaternion.identity);
                new_record.transform.parent = scrollview_parent;
                new_record.transform.Find("Label/Text").GetComponent<Text>().text = Global.history[i];
            }
        }
    }
}

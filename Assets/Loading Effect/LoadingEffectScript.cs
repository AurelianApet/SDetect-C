using UnityEngine;
using System.Collections;

public class LoadingEffectScript : MonoBehaviour {
    public bool loading = false;
    public Texture loadingTexture;
    public float size = 150f;
    public float rotAngle = 100f;
    public float rotSpeed = 200f;
    
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (loading)
        {
            rotAngle += rotSpeed * Time.deltaTime;
        }
    }

    void OnGUI()
    {
        if (loading)
        {
            Vector2 pivot = new Vector2(Screen.width / 2, Screen.height / 2);
            GUIUtility.RotateAroundPivot(rotAngle % 360, pivot);
            GUI.DrawTexture(new Rect((Screen.width - size) / 2, (Screen.height - size) / 2, size, size), loadingTexture);
        }
    }
}

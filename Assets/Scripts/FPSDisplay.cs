using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
    public Color m_TextColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
	float deltaTime = 0.0f;
	
    void Start()
    {        
    }

    public void Update()
	{
		deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
	}
	
	void OnGUI()
	{
		int w = Screen.width, h = Screen.height;
		
		GUIStyle style = new GUIStyle();
		
		Rect rect = new Rect(0, 0, w, h * 2 / 50);
		style.alignment = TextAnchor.UpperLeft;
		style.fontSize = h * 2 / 50;
		style.normal.textColor = m_TextColor;
		float msec = deltaTime * 1000.0f;
		float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        GUI.Label(rect, text, style);        
    }
}
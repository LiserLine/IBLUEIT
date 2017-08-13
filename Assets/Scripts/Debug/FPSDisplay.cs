/*
 * http://wiki.unity3d.com/index.php?title=FramesPerSecond
*/

using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
    private float deltaTime = 0.0f;
    private GUIStyle style = new GUIStyle();
    private Rect rect;

    private void Start()
    {
        int w = Screen.width, h = Screen.height;
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 40;
        style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);
        rect = new Rect(0, 0, w, h * 2 / 100);
    }

    private void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
    }

    private void OnGUI()
    {
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        GUI.Label(rect, text, style);
    }
}
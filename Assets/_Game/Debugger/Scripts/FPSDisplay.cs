/*
 * http://wiki.unity3d.com/index.php?title=FramesPerSecond
*/

using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
    private float _deltaTime;
    private readonly GUIStyle _style = new GUIStyle();
    private Rect _rect;

    private void Awake()
    {
        int w = Screen.width, h = Screen.height;
        _style.alignment = TextAnchor.UpperLeft;
        _style.fontSize = h * 2 / 40;
        _style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);
        _rect = new Rect(0, 0, w, h * 2 / 100);
    }

    private void Update()
    {
        _deltaTime += (Time.deltaTime - _deltaTime) * 0.1f;
    }

    private void OnGUI()
    {
        var msec = _deltaTime * 1000.0f;
        var fps = 1.0f / _deltaTime;
        var text = $"{msec:0.0} ms ({fps:0.} fps)";
        GUI.Label(_rect, text, _style);
    }
}
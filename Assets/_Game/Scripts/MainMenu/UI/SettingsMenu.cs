using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    //ToDo - Load settings from File
    //private void OnEnable()
    //{
    //}

    public void SetVolume(float value) => AudioListener.volume = value;
}
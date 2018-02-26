using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    public void SetVolume(float value) => AudioListener.volume = value;
}
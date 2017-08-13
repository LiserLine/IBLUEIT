using UnityEngine;

public class LocalizationSceneButtons : MonoBehaviour
{
    public string LocalizationFileName;

    public void LoadLocalizationFile()
    {
        if (string.IsNullOrEmpty(LocalizationFileName))
        {
            Debug.LogError("Localization file name not set to " + this.gameObject.name);
            return;
        }

        LocalizationManager.Instance.LoadLocalizationData(LocalizationFileName);
    }
}

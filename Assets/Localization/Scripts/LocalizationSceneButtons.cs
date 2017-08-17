using UnityEngine;

public class LocalizationSceneButtons : MonoBehaviour
{
    public string LocalizationFileName;

    public void LoadLocalizationFile()
    {
        if (string.IsNullOrEmpty(LocalizationFileName))
        {
            Debug.LogErrorFormat("Localization file name not set to {0}", this.gameObject.name);
            return;
        }

        GameUtilities.WriteAllText(GameConstants.LocalizationPath, LocalizationFileName);

        LocalizationManager.Instance.LoadLocalizationData(LocalizationFileName);
    }
}

using UnityEngine;

public class LocalizationSceneButtons : MonoBehaviour
{
    [SerializeField]
    private string localizationFileName;

    public void LoadLocalizationFile()
    {
        if (string.IsNullOrEmpty(localizationFileName))
        {
            Debug.LogErrorFormat("Localization file name not set to {0}", this.gameObject.name);
            return;
        }

        Utils.WriteAllText(@"savedata/localization/localization.dat", localizationFileName);

        LocalizationManager.Instance.LoadLocalizationData(localizationFileName);
    }
}

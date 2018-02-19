using UnityEngine;
using UnityEngine.UI;

public class LocalizedText : MonoBehaviour
{
    public string Key;

    private void OnEnable()
    {
        if (LocalizationManager.Instance != null)
            this.GetComponent<Text>().text = LocalizationManager.Instance.GetLocalizedValue(Key);
    }
}
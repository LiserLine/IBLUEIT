using UnityEngine;
using UnityEngine.UI;

public class LocalizedText : MonoBehaviour
{
    public string Key;
    private Text text;

    private void Start()
    {
        text = this.GetComponent<Text>();
        text.text = LocalizationManager.Instance.GetLocalizedValue(Key);
    }
}

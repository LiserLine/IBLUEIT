using UnityEngine;
using UnityEngine.UI;

public class LocalizedText : MonoBehaviour
{
    public string Key;
    private Text _text;

    private void Start()
    {
        _text = this.GetComponent<Text>();
        _text.text = LocalizationManager.Instance.GetLocalizedValue(Key);
    }
}

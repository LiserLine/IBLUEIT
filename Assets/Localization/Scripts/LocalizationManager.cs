// Source: https://www.youtube.com/watch?v=5Kt9jbnqzKA&list=PLX2vGYjWbI0TWkV9aEYq93bOX2kwseqUT

using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance;

    private Dictionary<string, string> _localizedText;

    public bool IsReady { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    public void LoadLocalizationData(string filename)
    {
        _localizedText = new Dictionary<string, string>();

        var filepath = Application.streamingAssetsPath + Path.AltDirectorySeparatorChar + filename;

#if UNITY_ANDROID
        var www = new WWW(filepath);

        if (!www.isDone)
        {
            Debug.Log("Waiting www...");
        }

        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.LogErrorFormat("WWW Failed to load localized text on {0}", filepath);
            return;
        }

        var jsonData = www.text;
#else
        if (!File.Exists(filepath))
        {
            Debug.LogErrorFormat("Failed to load localized text on {0}", filepath);
            return;
        }

        var jsonData = GameUtilities.ReadAllText(filepath);
#endif

        var loadedData = JsonUtility.FromJson<LocalizationData>(jsonData);

        foreach (var localizationItem in loadedData.Items)
        {
            _localizedText.Add(localizationItem.Key, localizationItem.Value);
        }

        Debug.LogFormat("LocalizationManager loaded {0} items.", _localizedText.Count);

        IsReady = true;
    }

    public string GetLocalizedValue(string key)
    {
        return !_localizedText.ContainsKey(key) ? key : _localizedText[key];
    }
}

// Source: https://www.youtube.com/watch?v=5Kt9jbnqzKA&list=PLX2vGYjWbI0TWkV9aEYq93bOX2kwseqUT

using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance;

    private Dictionary<string, string> _localizedText;
    private bool _isReady = false;
    private string _selectedLocalization;

    //This must be changed between projects!!!
    private int _localizationScreenId = 2;

    private readonly string PATH_SELECTED_LOCALIZATION =
        Path.Combine(GameConstants.SaveDataPath, "localization.dat");

    #region Properties

    public bool IsReady { get { return _isReady; } }
    public string Localization
    {
        get
        {
            return _selectedLocalization;
        }
    }
    public string FilePath { get { return PATH_SELECTED_LOCALIZATION; } }

    #endregion

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

        Debug.Log("LocalizationManager awaking...");

        DontDestroyOnLoad(gameObject);
        LoadLocalizationData();
    }

    public void LoadLocalizationData()
    {
        LoadLocalizationData(Localization);
    }

    public void LoadLocalizationData(string filename)
    {
        if (File.Exists(PATH_SELECTED_LOCALIZATION))
        {
            _selectedLocalization = filename = LoadSelectedLocalization();
        }
        else
        {
            if (string.IsNullOrEmpty(filename))
            {
                SceneManager.LoadScene(_localizationScreenId, LoadSceneMode.Additive);
                return;
            }
            else
            {
                _selectedLocalization = filename;
                CreateSelectedLocalization();
            }
        }

        _localizedText = new Dictionary<string, string>();

        string filepath = Path.Combine(Application.streamingAssetsPath, filename);
        if (!File.Exists(filepath))
        {
            Debug.LogWarning("Could not load localized text on " + filepath);
        }

        string jsonData = GameUtilities.ReadAllText(filepath);
        LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(jsonData);
        for (int i = 0; i < loadedData.Items.Length; i++)
        {
            _localizedText.Add(loadedData.Items[i].Key, loadedData.Items[i].Value);
        }

        Debug.Log("LocalizationManager loaded " + _localizedText.Count + " items.");

        try { SceneManager.UnloadSceneAsync(_localizationScreenId); } catch { }

        _isReady = true;
        Debug.Log("LocalizationManager is ready.");
    }

    private string LoadSelectedLocalization()
    {
        _selectedLocalization = GameUtilities.ReadAllText(PATH_SELECTED_LOCALIZATION);
        return _selectedLocalization;
    }

    public void CreateSelectedLocalization()
    {
        GameUtilities.WriteAllText(PATH_SELECTED_LOCALIZATION, _selectedLocalization);
    }

    public string GetLocalizedValue(string key)
    {
        if (!_localizedText.ContainsKey(key)) return key;
        return _localizedText[key];
    }

    public string GetLocalization()
    {
        return _selectedLocalization;
    }

    public void ResetManager()
    {
        _isReady = false;
        ResetLocalization();
    }

    public void ResetLocalization()
    {
        _selectedLocalization = string.Empty;
    }
}

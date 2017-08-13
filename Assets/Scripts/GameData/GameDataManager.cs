using System.IO;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance;

    private GameData _gameData;
    private bool _isReady = false;

    private readonly string _filePath = Path.Combine(GameConstants.SaveDataPath, "game.dat");

    #region Properties

    public bool IsReady { get { return _isReady; } }
    public string FilePath { get { return _filePath; } }
    public bool IsDebug { get { return _gameData.DebugMode; } }

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

        Debug.Log("GameManager awaking...");
        DontDestroyOnLoad(gameObject);

        LoadGameData();

        _isReady = true;
        Debug.Log("GameData is ready.");
    }

    private void OnDestroy()
    {
        //SaveGameData();
    }

    public void LoadGameData()
    {
        if (!File.Exists(FilePath))
        {
            NewGameData();
        }

        _gameData = (GameData)JsonUtility.FromJson(GameUtilities.ReadAllText(FilePath), typeof(GameData));
    }

    private void NewGameData()
    {
        GameData gm = new GameData()
        {
            DebugMode = true,
        };

        SaveGameData(gm);
    }

    private void SaveGameData(GameData gameData)
    {
        string jsonData = JsonUtility.ToJson(gameData, true);
        GameUtilities.WriteAllText(FilePath, jsonData);
    }

    public void SaveGameData()
    {
        SaveGameData(_gameData);
    }
}

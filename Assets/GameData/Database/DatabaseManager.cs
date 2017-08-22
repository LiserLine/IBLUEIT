using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager Instance { get; private set; }

    public AccountDb Accounts { get; private set; }

    public void Awake()
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

        Debug.Log("Loading Databases...");

        Accounts = new AccountDb();
        Debug.Log($"{Accounts.AccountList?.Count} accounts found.");
    }
}

using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Player Player { get; set; }

    private void Start()
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

    private void Update()
    {

    }
}

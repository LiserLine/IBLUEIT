using UnityEngine;

public class DebugToggle : MonoBehaviour
{
	private void Start()
	{
        this.gameObject.SetActive(GameDataManager.Instance.IsDebug);
    }
}

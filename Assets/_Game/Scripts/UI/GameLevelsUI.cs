using UnityEngine;
using UnityEngine.UI;

public class GameLevelsUI : MonoBehaviour
{
    [SerializeField]
    private Text insHeight, expHeight, expSize;

    private void FixedUpdate()
    {
        insHeight.text = Spawner.Instance.InspiratoryHeightLevel.ToString();
        expHeight.text = Spawner.Instance.ExpiratoryHeightLevel.ToString();
        expSize.text = Spawner.Instance.ExpiratorySizeLevel.ToString();
    }
}

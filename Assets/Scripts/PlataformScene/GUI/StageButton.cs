using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class StageButton : MonoBehaviour
{
    public int StageId;

    void Start()
    {
        if (GameManager.Instance.Player.OpenLevel >= StageId)
        {
            GetComponent<Button>().interactable = true;
        }
    }
}

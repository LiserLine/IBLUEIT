using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class StageButton : MonoBehaviour
{
    public int StageId;

    private void FixedUpdate()
    {
        if (GameManager.Instance.Player.StagesOpened >= StageId)
        {
            GetComponent<Button>().interactable = true;
        }
    }
}

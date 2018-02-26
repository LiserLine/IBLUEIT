using UnityEngine;
using UnityEngine.UI;

namespace Ibit.MainMenu.UI
{
    public class PitacoStatus : MonoBehaviour
    {
        [SerializeField]
        private SerialController serialController;

        [SerializeField]
        private Sprite offline, online;

        private void Awake()
        {
            if (serialController == null)
                serialController = FindObjectOfType<SerialController>();

            if (serialController == null)
                Debug.LogWarning("Serial Controller instance not found!");
        }

        private void FixedUpdate() => GetComponent<Image>().sprite = serialController.IsConnected ? online : offline;
    }
}
using UnityEngine;

public class PlataformOnTheFly : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F2))
            SerialController.Instance.Recalibrate();
    }
}

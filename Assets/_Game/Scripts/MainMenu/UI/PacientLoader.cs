using UnityEngine;
using UnityEngine.UI;

public class PacientLoader : MonoBehaviour
{
    public Pacient pacient;

    private void OnEnable() => this.GetComponent<Button>().onClick.AddListener(OnPacientSelected);

    private void OnPacientSelected()
    {
        Pacient.Loaded = pacient;
        GameObject.Find("Canvas").transform.Find("Player Menu").gameObject.SetActive(true);
        GameObject.Find("Canvas").transform.Find("Load Menu").gameObject.SetActive(false);
        Debug.Log($"{pacient.Name} loaded.");
    }
}
using UnityEngine;
using UnityEngine.UI;

public partial class CanvasManager : MonoBehaviour
{
    private const string Credits = "I Blue It - 0.4" +
        "\n\n[Masters Candidate - Renato Hartmann Grimes]" +
        "\nEletrical Engineering Department" +
        "\n\n[PhD. Marcelo da Silva Hounsell]" +
        "\nComputer Science Department" +
        "\n\n[Santa Catarina State University]" +
        "\nCenter of Technological Sciences";

    public void ShowCredits() => SysMessage.Info(Credits);

    public void ShowPlayerInfo()
    {
        SysMessage.Info($"Jogador: {Pacient.Loaded.Name}\n" +
                        $"Condição: {Pacient.Loaded.Condition}\n" +
                        $"Partidas Jogadas: {Pacient.Loaded.PlaySessionsDone}\n" +
                        $"Pico Exp.: {Pacient.Loaded.Capacities.RawExpPeakFlow} Pa\n" +
                        $"Pico Ins.: {Pacient.Loaded.Capacities.RawInsPeakFlow} Pa\n" +
                        $"Tempo Exp.: {Pacient.Loaded.Capacities.RawExpFlowDuration / 1000f:F1} s\n" +
                        $"Tempo Ins.: {Pacient.Loaded.Capacities.RawInsFlowDuration / 1000f:F1} s\n" +
                        $"Freq. Resp. Média: {Pacient.Loaded.Capacities.RawRespCycleDuration / 1000f:F1} s");
    }

    private void AddClickSfxToButtons()
    {
        foreach (var component in GameObject.Find("Canvas").GetComponentsInChildren(typeof(Button), true))
        {
            var btn = (Button)component;
            btn.onClick.AddListener(PlayClick);
        }
    }

    private void Awake()
    {
        if (Pacient.Loaded == null)
            return;

        GameObject.Find("Canvas").transform.Find("Start Panel").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("Player Menu").gameObject.SetActive(true);
    }

    private void PlayClick() => SoundManager.Instance.PlaySound("BtnClickUI");

    private void Start() => AddClickSfxToButtons();
}
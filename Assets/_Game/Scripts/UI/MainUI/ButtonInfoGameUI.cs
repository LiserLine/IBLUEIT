using UnityEngine;

public class ButtonInfoGameUI : MonoBehaviour
{
    public void ShowGameInfo()
    {
        const string credits = "I Blue It - v0.2 (Dev)" +
                               "\n\n[Masters Candidate - Renato Hartmann Grimes]" +
                               "\nEletrical Engineering Department" +
                               "\n\n[PhD. Marcelo da Silva Hounsell]" +
                               "\nComputer Science Department" +
                               "\n\n[Santa Catarina State University]" +
                               "\nCenter of Technological Sciences";

        SysMessage.Info(credits);
    }
}

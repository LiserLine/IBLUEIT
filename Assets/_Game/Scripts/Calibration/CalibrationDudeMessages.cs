using UnityEngine;

namespace Assets._Game.Scripts.Calibration
{
    public partial class CalibrationManager
    {
        private void DudeTalk(string msg)
        {
            balloonText.text = msg;
            dude.GetComponent<Animator>().SetBool("Talking", true);
        }

        private void DudeClearMessage()
        {
            balloonText.text = "";
            dude.GetComponent<Animator>().SetBool("Talking", false);
        }

        private void DudeCongratulate()
        {
            SoundManager.Instance.PlaySound("Success");
            DudeTalk("Muito bem! Pressione (►) para continuar.");
        }

        //private void DudeAskAgain()
        //{
        //    DudeTalk("Mais uma vez! Pressione (►) para continuar.");
        //    SetStep(currentStep - 2);
        //}

        private void DudeWarnUnknownFlow()
        {
            SoundManager.Instance.PlaySound("Failure");
            DudeTalk("Não consegui medir sua respiração. Vamos tentar novamente? Pressione (►) para continuar.");
        }

        private void DudeWarnPitacoDisconnected() => DudeTalk("O PITACO não está conectado. Conecte-o ao computador! Pressione (►) para continuar.");
    }
}
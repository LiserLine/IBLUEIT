using UnityEngine;

namespace _Game.Scripts.Calibration
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
            DudeTalk("Muito bem!");
            SetStep(currentExerciseCount == 3 ? currentStep + 2 : currentStep + 1);
        }

        private void DudeAskAgain()
        {
            DudeTalk("Mais uma vez!");
            SetStep(currentStep - 2);
        }

        private void DudeWarnUnknownFlow()
        {
            SoundManager.Instance.PlaySound("Failure");
            DudeTalk("Não consegui medir sua respiração. Vamos tentar novamente?");
            SetStep(currentStep);
        }

        private void DudeWarnPitacoDisconnected()
        {
            enterButton.SetActive(true);
            DudeTalk("O PITACO não está conectado. Conecte-o ao computador!");
            SetStep(99);
        }
    }
}
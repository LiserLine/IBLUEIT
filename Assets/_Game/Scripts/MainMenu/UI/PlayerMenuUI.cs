using Ibit.Core.Data;
using Ibit.Core.Util;
using UnityEngine;

namespace Ibit.MainMenu.UI
{
    public class PlayerMenuUI : MonoBehaviour
    {
        private void OnEnable()
        {
            if (!Pacient.Loaded.IsCalibrationDone)
            {
                SysMessage.Info("Para começar a jogar, você precisa \n calibrar todas 5 ações no menu de calibração!");
            }
        }
    }
}

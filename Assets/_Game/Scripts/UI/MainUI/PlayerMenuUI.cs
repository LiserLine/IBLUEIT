public class PlayerMenuUI : BasicUI<PlayerMenuUI>
{
    public void ShowPlayerInfo()
    {
        SysMessage.Info($"Jogador: {Pacient.Loaded.Name}\n" +
                        $"Condição: {Pacient.Loaded.Disfunction}\n" +
                        $"Partidas Jogadas: {Pacient.Loaded.PlaySessionsDone}\n" +
                        $"Pico Exp.: {Pacient.Loaded.RespiratoryData.ExpiratoryPeakFlow}Pa\n" +
                        $"Pico Ins.: {Pacient.Loaded.RespiratoryData.InspiratoryPeakFlow} Pa\n" +
                        $"Tempo Exp.: {Pacient.Loaded.RespiratoryData.ExpiratoryFlowTime / 1000f:F1}s\n" +
                        $"Tempo Ins.: {Pacient.Loaded.RespiratoryData.InspiratoryFlowTime / 1000f:F1}s\n" +
                        $"Freq. Resp. Média: {Pacient.Loaded.RespiratoryData.RespiratoryFrequency / 1000f:F1}s");
    }
}

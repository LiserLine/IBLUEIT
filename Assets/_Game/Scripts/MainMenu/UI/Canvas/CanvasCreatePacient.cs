﻿using Ibit.Core.Data;
using Ibit.Core.Database;
using Ibit.Core.Util;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Ibit.MainMenu.UI.Canvas
{
    public partial class CanvasManager
    {
        public void CreatePacient()
        {
            var bDay = GameObject.Find("LabelBDay").GetComponent<Text>().text;
            var bMonth = GameObject.Find("LabelBMonth").GetComponent<Text>().text;
            var bYear = GameObject.Find("LabelBYear").GetComponent<Text>().text;

            DateTime birthday;
            try
            {
                birthday = new DateTime(int.Parse(bYear), int.Parse(bMonth), int.Parse(bDay));
            }
            catch (ArgumentOutOfRangeException)
            {
                SysMessage.Warning("Data invalida!");
                return;
            }

            var playerName = GameObject.Find("InputFieldName").GetComponent<InputField>().text;

            if (playerName.Length == 0)
            {
                SysMessage.Warning("Nome de jogador indefinido!");
                return;
            }

            var normal = GameObject.Find("ToggleNormal").GetComponent<Toggle>().isOn;
            var obstructive = GameObject.Find("ToggleObstructive").GetComponent<Toggle>().isOn;
            var restrictive = GameObject.Find("ToggleRestrictive").GetComponent<Toggle>().isOn;

            if (normal == obstructive == restrictive == false)
            {
                SysMessage.Warning("Condição Indefinida!");
                return;
            }

            var disfunction = restrictive ? ConditionType.Restrictive
                : (obstructive ? ConditionType.Obstructive : ConditionType.Normal);

            var observations = GameObject.Find("Observations").GetComponent<InputField>().text;

            var ethnicity = GameObject.Find("EthnicityLabel").GetComponent<Text>().text;

            float weight;
            try
            {
                weight = Parsers.Float(GameObject.Find("WeightText").GetComponent<Text>().text);

                if (weight < 10)
                    throw new Exception();
            }
            catch (Exception)
            {
                SysMessage.Warning("Peso inválido");
                return;
            }

            float height;
            try
            {
                height = Parsers.Float(GameObject.Find("HeightText").GetComponent<Text>().text);

                if (height < 70f || height > 250f)
                    throw new Exception();
            }
            catch (Exception)
            {
                SysMessage.Warning("Altura inválida");
                return;
            }

            float threshold;
            try
            {
                threshold = Parsers.Float(GameObject.Find("ThresholdText").GetComponent<Text>().text);

                if (threshold < 0)
                    throw new Exception();
            }
            catch (Exception)
            {
                SysMessage.Warning("Threshold inválido");
                return;
            }

            var plr = new Pacient
            {
                Name = playerName,
                Birthday = birthday,
                Condition = disfunction,
                Id = PacientDb.Instance.PacientList.Count > 0 ? PacientDb.Instance.PacientList.Max(x => x.Id) + 1 : 1,
                Observations = observations,
                Capacities = new Capacities(),
                CalibrationDone = false,
                UnlockedLevels = 1,
                AccumulatedScore = 0,
                Ethnicity = ethnicity,
                Height = height,
                HowToPlayDone = false,
                PitacoThreshold = threshold,
                PlaySessionsDone = 0,
                Weight = weight
            };

            var tmpPlr = PacientDb.Instance.GetPacient(playerName);

            if (plr.Name.Equals(tmpPlr?.Name)
                && plr.Birthday.Equals(tmpPlr?.Birthday)
                && plr.Condition.Equals(tmpPlr?.Condition))
            {
                SysMessage.Warning("Jogador existente!");
                return;
            }

            PacientDb.Instance.CreatePacient(plr);
            Pacient.Loaded = plr;

            GameObject.Find("Canvas").transform.Find("New Menu").gameObject.SetActive(false);
            GameObject.Find("Canvas").transform.Find("Player Menu").gameObject.SetActive(true);

            SysMessage.Info("Jogador criado com sucesso!");
        }
    }
}
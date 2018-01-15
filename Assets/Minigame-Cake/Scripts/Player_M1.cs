using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_M1 : MonoBehaviour {
	

	public Stat flow;

	public Candles candle;

	public Stars score;

	public ScoreMenu scoreMenu;

	public SerialController serialController;
	public float picoExpiratorio = 0;
	public bool stopedFlow = false;
    public float sensorValue;

	
	//Cria Player para testes.
	void Start () {
		#if UNITY_EDITOR
		if (GameManager.Instance.Player == null)
		{
			GameManager.Instance.Player = new Player
			{
				Name = "NetRunner",
				Id = 9999,
				RespiratoryInfo = new RespiratoryInfo
				{
					InspiratoryPeakFlow = 300f,
					ExpiratoryPeakFlow = 500f,
					InspiratoryFlowTime = 1f,
					ExpiratoryFlowTime = 3f,
					RespirationFrequency = 0.1666f                    
				},
				CalibrationDone = true,
				SessionsDone = 1,
			};
		}

		if (GameManager.Instance.Stage == null)
		{
			GameManager.Instance.Stage = new Stage
			{
				Id = 777,
			};
		}
		#endif


	}



	void OnEnable()
	{
		serialController.OnSerialMessageReceived += OnMessageReceived;
	}

	void OnDisable()
	{
		serialController.OnSerialMessageReceived -= OnMessageReceived;
	}

	// Update is called once per frame
	void OnMessageReceived (string msg) {

		if (!SerialGetOffset.IsUsingOffset) 
			return;

		if (msg.Length < 1) 
			return;

		sensorValue = GameUtilities.ParseFloat(msg) - SerialGetOffset.Offset;

		//var playerPeak = GameManager.Instance.Player.RespiratoryInfo.ExpiratoryPeakFlow;

		//Debug.Log ($"Sensor: {sensorValue} [25%: {0.25f * playerPeak} | 50%: {0.5f * playerPeak} | 75%: {0.75f * playerPeak}]");


		if (sensorValue > 0 && picoExpiratorio < sensorValue)
			picoExpiratorio = sensorValue;

    }

	public void EnablePlay(){
		
	}

		
		
		

		




}

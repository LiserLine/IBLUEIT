using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// This script is set to the player
/// </summary>
public class PositionOnSerial : MonoBehaviour
{
	private Player _player;
	private Transform _transform;
	private SerialListener _serialMessager;
	private bool _isUsingOffset;
	private float _offset;
	private float _cameraOffset;

	private const float RelativeLimit = 0.3f;

	public ControlBehaviour Behaviour;

	private void Start()
	{
		_transform = GetComponent<Transform>();
		_serialMessager = GetComponent<SerialListener>();
		_isUsingOffset = false;
		_cameraOffset = Camera.main.orthographicSize - Camera.main.transform.position.y - 1;
		_player = GameManager.Instance?.Player;

		StartCoroutine(GetOffset());
	}

	private IEnumerator GetOffset()
	{
		while (!_serialMessager.IsConnected)
			yield return new WaitForSeconds(3f);
		
		var temp = 0f;
		for (var i = 0; i < 5000; i++)
		{
			var message = _serialMessager.MessageReceived;
			temp += string.IsNullOrEmpty(message) ? 0f : ParseSerialMessage(message);
		}

		_offset = temp / 5000f;
		Debug.Log($"Offset set to {_offset}");

		_isUsingOffset = true;
	}

	private void Update()
	{
		ChangeBehaviourHotkey();
		PositionOnSerialMessage();
	}

	private void PositionOnSerialMessage()
	{
		if (!_isUsingOffset) return;

		var message = _serialMessager.MessageReceived;
		if (message.Length < 1) return;

		var sensorValue = ParseSerialMessage(message) - _offset;

#if UNITY_EDITOR
		var ExpiratoryPeakFlow = 300f * 0.75f * GameConstants.UserPowerMercy; //debug
		var nextPosition = _cameraOffset * sensorValue / ExpiratoryPeakFlow;
#else
        Debug.Log($"UserPowerMercy: {GameConstants.UserPowerMercy}")
        var nextPosition = (_cameraOffset * sensorValue / _player.ExpiratoryPeakFlow) * GameConstants.UserPowerMercy;
#endif

		Vector3 a = _transform.position;
		Vector3 b = Vector3.zero;
		switch (Behaviour)
		{
			case ControlBehaviour.Absolute:
				b = new Vector3(_transform.position.x, nextPosition, _transform.position.z);
				break;

			case ControlBehaviour.Relative:
				if (!(nextPosition >= -RelativeLimit && nextPosition <= RelativeLimit))
				{
					b = _transform.position + new Vector3(0f, nextPosition);
				}
				break;
		}

		_transform.position = Vector3.Lerp(a, b, Time.deltaTime * 5f);

		PitacoRecorder.Instance.Add(_transform.position.y);
	}

	private static float ParseSerialMessage(string msg) => float.Parse(msg.Replace('.', ','));

	private void OnCollisionEnter(Collision collision)
	{
		Destroy(collision.gameObject);
	}

	private void OnEnable()
	{
		PitacoRecorder.Instance.Start();
	}

	private void OnDisable()
	{
		PitacoRecorder.Instance.Stop();

#if UNITY_EDITOR
		PitacoRecorder.Instance.WriteData();
#else
        var stage = GameManager.Instance.Stage;
        PitacoRecorder.Instance.WriteData(_player, stage, GameConstants.GetSessionsPath(_player), true);
#endif

	}

	private void ChangeBehaviourHotkey()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			switch (Behaviour)
			{
				case ControlBehaviour.Absolute:
					Behaviour = ControlBehaviour.Relative;
					break;
				case ControlBehaviour.Relative:
					Behaviour = ControlBehaviour.Absolute;
					break;
			}

			Debug.LogFormat("Behaviour: {0}", Behaviour);
		}
	}
}

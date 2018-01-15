using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Candles : MonoBehaviour{

	public Sprite[] candlesOn = new Sprite[3];

	public Sprite candleOff;

    public Sprite candleOn;

	public void TurnOff(int index){
		this.gameObject.transform.GetChild(index).GetComponent<SpriteRenderer>().sprite = candleOff;
	}
    public void TurnOn()
    {
		this.gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = candleOn;
        this.gameObject.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = candleOn;
        this.gameObject.transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = candleOn;
    }
		


}
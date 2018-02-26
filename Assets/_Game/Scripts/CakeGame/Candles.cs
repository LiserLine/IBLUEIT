using UnityEngine;

namespace Ibit.CakeGame
{
    public class Candles : MonoBehaviour
    {
        public Sprite candleOff;
        public Sprite candleOn;
        public Sprite[] candlesOn = new Sprite[3];

        public void TurnOff(int index)
        {
            gameObject.transform.GetChild(index).GetComponent<SpriteRenderer>().sprite = candleOff;
        }

        public void TurnOn()
        {
            gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = candleOn;
            gameObject.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = candleOn;
            gameObject.transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = candleOn;
        }
    }
}
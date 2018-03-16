using UnityEngine;

namespace Ibit.CakeGame
{
    public class Candles : MonoBehaviour
    {
        public Sprite candleOff;
        public Sprite candleOn;
        public Sprite[] candlesOn = new Sprite[3];
        public Animator[] candlesAnim;

        public void TurnOff(int index)
        {
            candlesAnim[index].SetBool("TurnOff", true);
        }

        public void TurnOn()
        {
            candlesAnim[0].SetBool("TurnOff", false);
            candlesAnim[1].SetBool("TurnOff", false);
            candlesAnim[2].SetBool("TurnOff", false);
            //gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = candleOn;
            //gameObject.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = candleOn;
            //gameObject.transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = candleOn;
        }
    }
}
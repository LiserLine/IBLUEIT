using UnityEngine;
using UnityEngine.UI;

// ToDo - Slider to help player in calibration
public class SliderColorOnSerial : MonoBehaviour
{
	[SerializeField]
	float test = 0;

	private Slider _slider;

	void Start()
	{
		_slider = this.GetComponent<Slider>();
	}

	void Update()
	{
		var tmpColors = _slider.colors;

		if(_slider.value < 0.2 || _slider.value > 0.8)
		{
			tmpColors.normalColor = Color.red;
		}
		else
		{
			tmpColors.normalColor = Color.green;
		}

		_slider.colors = tmpColors;
	}
}
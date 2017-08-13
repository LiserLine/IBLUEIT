using UnityEngine;

public class KeepChangingColor : MonoBehaviour
{
    public float Frequency = 0.5f; //time in seconds

    private Material _material;
    private float _delta = 0.0f;

    private void Start()
    {
        _material = this.GetComponent<Material>();
    }

    private void Update()
    {
        _delta += Time.deltaTime;
        if (_delta >= Frequency)
        {
            _material.color = new Color(Random.value, Random.value, Random.value, 1.0f); ;
            _delta = 0;
        }
    }
}

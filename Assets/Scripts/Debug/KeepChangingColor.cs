using UnityEngine;

public class KeepChangingColor : MonoBehaviour
{
    private Material _material;
    private float _delta;

    private void Awake()
    {
        _material = GetComponent<Material>();
    }

    private void Update()
    {
        _delta += Time.deltaTime;

        if (_delta <= 0.5f) return;

        _material.color = new Color(Random.value, Random.value, Random.value, 1.0f); ;
        _delta = 0;
    }
}

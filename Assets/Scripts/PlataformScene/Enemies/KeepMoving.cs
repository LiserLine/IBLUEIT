using UnityEngine;

public class KeepMoving : MonoBehaviour
{
    private Transform _transform;
    private float _delta = 0f;

    [Header("Settings")]
    public float Speed = 1f;
    public float SelfDestructTimer = 10f;

    private void Start()
    {
        _transform = this.GetComponent<Transform>();
    }

    private void Update()
    {
        _transform.Translate(new Vector3(-(Speed * Time.deltaTime) * 7.5f, 0f, 0f));

        _delta += Time.deltaTime;
        if(_delta > this.SelfDestructTimer)
        {
            Destroy(this.gameObject);
        }
    }
}

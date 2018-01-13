using UnityEngine;

public class MoveObject : MonoBehaviour
{
    public float speed { get; set; } = 1;

    [SerializeField]
    private int destroyTimer = 200;

    private void OnEnable()
    {
        Destroy(this.gameObject, destroyTimer);
    }

    private void Update()
    {
        this.transform.Translate(new Vector3(-Time.deltaTime * speed, 0f));
    }
}

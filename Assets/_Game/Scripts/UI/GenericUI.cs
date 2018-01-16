using UnityEngine;

public abstract class GenericUI<T> : MonoBehaviour where T : MonoBehaviour
{
    public virtual void Show() => this.transform.localScale = Vector3.one;
    public virtual void Hide() => this.transform.localScale = Vector3.zero;
}

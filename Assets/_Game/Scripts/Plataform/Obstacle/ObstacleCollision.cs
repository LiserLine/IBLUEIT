using UnityEngine;

public partial class Obstacle
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Contains("Player"))
        {
            TakeDamage();
        }
    }

    private void TakeDamage()
    {
        heartPoint--;
    }
}
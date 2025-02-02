using UnityEngine;

public class DamagePlayer : MonoBehaviour
{
    private int damage = 25;

    private void OnTriggerEnter(Collider other)
    {
        PlayerStats playerStats = other.GetComponent<PlayerStats>();

        if (playerStats != null)
        {
            playerStats.TakeDamage(damage);
        }
    }
}

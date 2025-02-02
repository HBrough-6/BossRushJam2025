using UnityEngine;

public class DamageCollider : MonoBehaviour
{
    public BoxCollider damageCollider;
    PlayerStats stats;
    public int currentWeaponDamage = 30;

    private void Awake()
    {
        damageCollider = GetComponent<BoxCollider>();
        damageCollider.gameObject.SetActive(true);
        damageCollider.isTrigger = true;
        damageCollider.enabled = false;

        stats = FindObjectOfType<PlayerStats>();
    }

    public void EnableDamageCollider()
    {
        damageCollider.enabled = true;
    }

    public void DisableDamageCollider()
    {
        damageCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Enemy"))
        {
            EnemyStats enemyStats = other.GetComponent<EnemyStats>();

            if (enemyStats != null)
            {
                enemyStats.TakeDamage(Mathf.RoundToInt(currentWeaponDamage * stats.dmgModifier));
            }
        }
    }
}

using UnityEngine;

public class ItemPickup : Interactable
{
    public enum ItemStatBoost
    {
        Health,
        Stamina,
        Attack
    }

    public ItemStatBoost statIncrease;

    public override void Interact(PlayerManager playerManager)
    {
        base.Interact(playerManager);
        Debug.Log("interacted with item");
        PickUpItem(playerManager);
    }

    private void PickUpItem(PlayerManager playerManager)
    {
        PlayerStats stats = playerManager.GetComponent<PlayerStats>();
        PlayerLocomotion playerLocomotion = playerManager.GetComponent<PlayerLocomotion>();

        playerLocomotion.rigidbody.velocity = Vector3.zero; // stops the player from sliding while picking up the item

        switch (statIncrease)
        {
            case ItemStatBoost.Health:
                IncreaseHealth(stats);
                break;
            case ItemStatBoost.Stamina:
                IncreaseStamina(stats);
                break;
            case ItemStatBoost.Attack:
                IncreaseAttack(stats);
                break;
            default:
                break;
        }

        Debug.Log("Hello");
        Destroy(this.gameObject);
    }

    private void IncreaseHealth(PlayerStats stats)
    {
        stats.IncreaseHealthLevel(3);
    }

    private void IncreaseStamina(PlayerStats stats)
    {
        stats.IncreaseStaminaLevel(3);
    }

    private void IncreaseAttack(PlayerStats stats)
    {
        stats.BuffDamage();
    }
}

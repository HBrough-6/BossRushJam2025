using UnityEngine;

public class PlayerAttacker : MonoBehaviour
{
    AnimatorHandler animatorHandler;
    InputHandler inputHandler;
    WeaponSlotManager weaponSlotManager;
    public string lastAttack;

    private void Awake()
    {
        animatorHandler = GetComponentInChildren<AnimatorHandler>();
        inputHandler = GetComponent<InputHandler>();
        weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
    }

    public void HandleWeaponCombo(WeaponItem weapon)
    {
        if (inputHandler.comboFlag)
        {
            animatorHandler.anim.SetBool("canDoCombo", false);
            if (lastAttack == weapon.OH_Light_Attack_1)
            {
                animatorHandler.PlayTargetAnimation(weapon.OH_Light_Attack_2, true);
                lastAttack = weapon.OH_Light_Attack_2;
            }
            else if (lastAttack == weapon.OH_Light_Attack_2)
            {
                animatorHandler.PlayTargetAnimation(weapon.OH_Light_Attack_3, true);
                lastAttack = weapon.OH_Light_Attack_3;
            }
        }
    }

    public void HandleLightAttack(WeaponItem weapon)
    {
        weaponSlotManager.attackingWeapon = weapon;
        animatorHandler.PlayTargetAnimation(weapon.OH_Light_Attack_1, true);
        lastAttack = weapon.OH_Light_Attack_1;
    }

    public void HandleHeavyAttack(WeaponItem weapon)
    {
        weaponSlotManager.attackingWeapon = weapon;
        animatorHandler.PlayTargetAnimation(weapon.OH_Heavy_Attack_1, true);
        lastAttack = weapon.OH_Heavy_Attack_1;
    }
}

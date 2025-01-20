using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    WeaponSlotManager WeaponSlotManager;

    public WeaponItem rightWeapon;
    public WeaponItem leftWeapon;

    private void Awake()
    {
        WeaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
    }

    private void Start()
    {
        WeaponSlotManager.loadWeaponOnSlot(rightWeapon, false);
        WeaponSlotManager.loadWeaponOnSlot(leftWeapon, true);
    }
}

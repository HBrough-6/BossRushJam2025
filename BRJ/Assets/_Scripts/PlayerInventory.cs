using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    WeaponSlotManager WeaponSlotManager;

    public WeaponItem rightWeapon;
    public WeaponItem leftWeapon;

    public WeaponItem unarmedWeapon;

    public WeaponItem[] weaponsInRightHandSlot = new WeaponItem[1];
    public WeaponItem[] weaponsInLeftHandSlot = new WeaponItem[1];

    public int currentRightWeaponIndex = 0;
    public int currentLeftWeaponIndex = -1;

    private void Awake()
    {
        WeaponSlotManager = GetComponentInChildren<WeaponSlotManager>();

    }

    private void Start()
    {
        rightWeapon = weaponsInRightHandSlot[0];
        leftWeapon = unarmedWeapon;

        WeaponSlotManager.loadWeaponOnSlot(rightWeapon, false);
        WeaponSlotManager.loadWeaponOnSlot(leftWeapon, true);
    }

    public void ChangeRightWeapon()
    {
        currentRightWeaponIndex = currentRightWeaponIndex + 1;

        if (currentRightWeaponIndex == 0 && weaponsInRightHandSlot[0] != null)
        {
            rightWeapon = weaponsInRightHandSlot[currentRightWeaponIndex];
            WeaponSlotManager.loadWeaponOnSlot(weaponsInRightHandSlot[currentRightWeaponIndex], false);
        }
        else if (currentRightWeaponIndex == 0 && weaponsInRightHandSlot[0] == null)
        {
            currentRightWeaponIndex += 1;
        }

        else if (currentRightWeaponIndex == 1 && weaponsInRightHandSlot[1] != null)
        {
            rightWeapon = weaponsInRightHandSlot[currentRightWeaponIndex];
            WeaponSlotManager.loadWeaponOnSlot(weaponsInRightHandSlot[currentRightWeaponIndex], false);
        }
        else
        {
            currentRightWeaponIndex += 1;
        }

        if (currentRightWeaponIndex > weaponsInRightHandSlot.Length - 1)
        {
            currentRightWeaponIndex = -1;
            rightWeapon = unarmedWeapon;
            WeaponSlotManager.loadWeaponOnSlot(unarmedWeapon, false);
        }
    }

    public void ChangeLeftWeapon()
    {
        currentLeftWeaponIndex = currentLeftWeaponIndex + 1;

        if (currentLeftWeaponIndex == 0 && weaponsInLeftHandSlot[0] != null)
        {
            leftWeapon = weaponsInLeftHandSlot[currentLeftWeaponIndex];
            WeaponSlotManager.loadWeaponOnSlot(weaponsInLeftHandSlot[currentLeftWeaponIndex], true);
        }
        else if (currentLeftWeaponIndex == 0 && weaponsInLeftHandSlot[0] == null)
        {
            currentLeftWeaponIndex += 1;
        }

        else if (currentLeftWeaponIndex == 1 && weaponsInLeftHandSlot[1] != null)
        {
            leftWeapon = weaponsInLeftHandSlot[currentLeftWeaponIndex];
            WeaponSlotManager.loadWeaponOnSlot(weaponsInLeftHandSlot[currentLeftWeaponIndex], false);
        }
        else
        {
            currentLeftWeaponIndex += 1;
        }

        if (currentLeftWeaponIndex > weaponsInLeftHandSlot.Length - 1)
        {
            currentLeftWeaponIndex = -1;
            leftWeapon = unarmedWeapon;
            WeaponSlotManager.loadWeaponOnSlot(unarmedWeapon, true);
        }
    }
}

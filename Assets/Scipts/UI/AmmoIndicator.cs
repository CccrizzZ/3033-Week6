using UnityEngine;
using UnityEngine.UI;
using Weapons;

public class AmmoIndicator : MonoBehaviour
{

    public Text WeaponName;
    public Text curAmmo;
    public Text totalAmmo;

    private WeaponComponent WeaponComponent;

    private void OnWeaponEquipped(WeaponComponent weaponComponent)
    {
        WeaponComponent = weaponComponent;
    }


    private void OnEnable()
    {
        PlayerEvent.OnWeaponEquipped += OnWeaponEquipped;
    }

    private void OnDisable()
    {
        PlayerEvent.OnWeaponEquipped -= OnWeaponEquipped;
    }


    private void Update()
    {
        WeaponName.text = WeaponComponent.WeaponInformation.WeaponName;
        curAmmo.text = WeaponComponent.WeaponInformation.BulletsInClip.ToString();
        totalAmmo.text = WeaponComponent.WeaponInformation.BulletsAvailable.ToString();
    }


}

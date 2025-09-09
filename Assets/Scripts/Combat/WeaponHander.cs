using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHander : MonoBehaviour
{
    [field: SerializeField] public GameObject Weapon { get; private set; }

    public void EnableWeapon()
    {
        Weapon.SetActive(true);
    }

    public void DisableWeapon()
    {
        Weapon.SetActive(false);
    }
}

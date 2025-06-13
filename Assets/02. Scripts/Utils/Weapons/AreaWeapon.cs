using System.Collections.Generic;
using UnityEngine;

public class AreaWeapon : WeaponBase
{
    public float tickInterval = 1f;
    float timer;

    public override void OnAttach(CombatController newOwner)
    {
        owner = newOwner;
    }

    public override void OnDetach()
    {
        Destroy(gameObject);
    }
}

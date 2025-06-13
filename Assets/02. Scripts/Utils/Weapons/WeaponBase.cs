using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    public CombatController owner;
    public abstract void OnAttach(CombatController owner);
    public abstract void OnDetach();
}

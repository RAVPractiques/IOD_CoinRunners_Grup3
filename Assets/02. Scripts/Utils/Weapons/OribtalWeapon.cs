using UnityEngine;

public class OrbitalWeapon : WeaponBase
{
    [SerializeField]
    private float orbitRadius = 2f;

    [SerializeField]
    private float orbitSpeed = 180f;

    [HideInInspector]
    public int index;

    [HideInInspector]
    public int total;

    public override void OnAttach(CombatController newOwner)
    {
        owner = newOwner;
    }

    public override void OnDetach()
    {
        Destroy(gameObject);
    }

    void Update()
    {
        if (owner == null)
            return;
        float angle = index * Mathf.PI * 2f / total + Time.time * orbitSpeed * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(angle), 1, Mathf.Sin(angle)) * orbitRadius;
        transform.position = owner.transform.position + offset;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (
            other.GetComponentInChildren<CombatController>() is CombatController combatController
            && combatController != owner
        )
        {
            int damage = owner.Damage;
            int knockbackPower = owner.Knockback;
            Vector3 direction = (other.transform.position - owner.transform.position).normalized;
            combatController.TakeDamage(damage, knockbackPower, direction);
        }
    }
}

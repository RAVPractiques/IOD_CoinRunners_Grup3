using System.Collections.Generic;
using UnityEngine;

public class AreaWeapon : WeaponBase
{
    [SerializeField]
    private float radiusPerCoin = 0.5f;

    [SerializeField]
    private float tickInterval = 1f;

    private SphereCollider areaCollider;
    private Dictionary<Collider, float> nextTick = new Dictionary<Collider, float>();

    private void Awake()
    {
        areaCollider = GetComponent<SphereCollider>();
        if (areaCollider == null)
            Debug.LogError("AreaWeapon necesita un SphereCollider en el objeto.");
    }

    private void Update()
    {
        if (owner != null)
        {
            // Coloca el arma de área en los pies del jugador (ajusta el offset Y si es necesario)
            Vector3 pos = owner.transform.position;
            pos.y = owner.transform.position.y - 1; // O usa un offset si quieres que esté más bajo
            transform.position = pos;
        }
    }

    public override void OnAttach(CombatController newOwner)
    {
        owner = newOwner;
        UpdateRadius();

        var coinObtainer = owner.GetComponentInParent<CoinObtainer>();
        if (coinObtainer != null)
            coinObtainer.OnCoinObtained += OnCoinObtained;
    }

    public override void OnDetach()
    {
        var coinObtainer = owner.GetComponentInParent<CoinObtainer>();
        if (coinObtainer != null)
            coinObtainer.OnCoinObtained -= OnCoinObtained;
    }

    private void OnCoinObtained(int newCoins)
    {
        UpdateRadius();
    }

    private void UpdateRadius()
    {
        transform.localScale = transform.localScale + Vector3.one * radiusPerCoin;
    }

    private void OnTriggerStay(Collider other)
    {
        if (
            other.GetComponentInChildren<CombatController>() is CombatController combatController
            && combatController != owner
        )
        {
            if (!nextTick.ContainsKey(other))
                nextTick[other] = Time.time;

            if (Time.time >= nextTick[other])
            {
                combatController.TakeDamage(
                    owner.Damage,
                    owner.Knockback,
                    (other.transform.position - owner.transform.position).normalized
                );
                nextTick[other] = Time.time + tickInterval;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (nextTick.ContainsKey(other))
            nextTick.Remove(other);
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    [SerializeField]
    private CharacterStatsData statsData;

    [Header("Stats Multipliers")]
    [Range(1, 20)]
    public int healthMultiplier = 1;

    [Range(1, 20)]
    public int damageMultiplier = 1;

    [Range(1, 20)]
    public int knockbackMultiplier = 1;

    [Range(1, 20)]
    public int moveSpeedMultiplier = 1;

    private List<WeaponBase> weapons = new List<WeaponBase>();

    private int currentHealth;
    private Rigidbody rb;
    private CoinObtainer coinObtainer;

    public int Damage => statsData.damage * damageMultiplier;
    public int Knockback => statsData.knockback * knockbackMultiplier;
    public int MoveSpeed => statsData.moveSpeed * moveSpeedMultiplier;
    public int MaxHealth => statsData.maxHealth * healthMultiplier;

    void Awake()
    {
        currentHealth = MaxHealth;
        rb = GetComponentInParent<Rigidbody>();
    }

    private void OnEnable()
    {
        coinObtainer = GetComponentInParent<CoinObtainer>();
        if (coinObtainer != null)
        {
            coinObtainer.OnCoinObtained += OnCoinObtained;
        }
    }

    private void OnCoinObtained(int coins)
    {
        AddWeaponOfMyType();
    }

    void Start()
    {
        AddWeaponOfMyType();
    }

    void AddWeaponOfMyType()
    {
        if (statsData.weaponPrefab == null)
            return;

        var newWeapon = Instantiate(statsData.weaponPrefab);
        var weaponBase = newWeapon.GetComponent<WeaponBase>();
        if (weaponBase is OrbitalWeapon orbital)
        {
            orbital.index = weapons.Count;
            orbital.total = weapons.Count + 1;
            foreach (var w in weapons)
                if (w is OrbitalWeapon ow)
                    ow.total = weapons.Count + 1;
        }
        AttachWeapon(weaponBase);
    }

    public void AttachWeapon(WeaponBase weapon)
    {
        weapon.OnAttach(this);
        weapons.Add(weapon);
    }

    public void DetachWeapon(WeaponBase weapon)
    {
        weapon.OnDetach();
        weapons.Remove(weapon);
    }

    public void TakeDamage(int dmg, int knockbackPower, Vector3 direction)
    {
        currentHealth -= dmg;
        Debug.Log($"Taking damage: {dmg}, Current Health: {currentHealth}");
        Vector3 knockbackDirection = direction * knockbackPower;
        rb.AddForce(knockbackDirection, ForceMode.Impulse);
        if (currentHealth <= 0)
            OnDeath();
    }

    public void OnDeath()
    {
        foreach (var w in weapons)
            w.OnDetach();
        weapons.Clear();
        Destroy(transform.parent.gameObject, 1f);
    }
}

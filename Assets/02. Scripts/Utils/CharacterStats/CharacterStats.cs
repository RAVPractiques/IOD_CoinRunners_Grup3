using UnityEngine;

[CreateAssetMenu(menuName = "Character/Stats")]
public class CharacterStatsData : ScriptableObject
{
    [Range(1, 5)]
    public int maxHealth = 3;

    [Range(1, 5)]
    public int damage = 2;

    [Range(1, 5)]
    public int knockback = 2;

    [Range(1, 5)]
    public int moveSpeed = 3;
    public GameObject weaponPrefab;
}

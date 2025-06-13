using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class StormController : MonoBehaviour
{
    [SerializeField]
    private float innerRadius = 10f;

    [SerializeField]
    private float outerRadius = 20f;

    [SerializeField]
    private float shrinkSpeed = 2f;

    [SerializeField]
    private float minInnerRadius = 2f;

    [SerializeField]
    private float damagePerSecond = 10f;

    [SerializeField]
    private float waitTime = 10f;

    private bool shrinking = false;

    private SphereCollider stormCollider;
    private ParticleSystem stormParticles;
    private Dictionary<Collider, float> playerNextDamage = new Dictionary<Collider, float>();

    void Start()
    {
        stormCollider = GetComponent<SphereCollider>();
        if (stormCollider)
            stormCollider.radius = outerRadius;
        stormParticles = GetComponentInChildren<ParticleSystem>();
        stormParticles?.Play();
        StartCoroutine(ShrinkStormCoroutine());
    }

    IEnumerator ShrinkStormCoroutine()
    {
        yield return new WaitForSeconds(waitTime);
        shrinking = true;
    }

    void Update()
    {
        if (shrinking)
        {
            if (innerRadius > minInnerRadius)
                innerRadius -= shrinkSpeed * Time.deltaTime;
        }

        if (stormParticles)
        {
            ShapeModule shape = stormParticles.shape;
            shape.radius = innerRadius;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.GetComponentInChildren<CombatController>() is CombatController combatController)
        {
            float distance = Vector3.Distance(transform.position, other.transform.position);
            if (distance > innerRadius)
            {
                if (!playerNextDamage.ContainsKey(other))
                    playerNextDamage[other] = Time.time;

                if (Time.time >= playerNextDamage[other])
                {
                    if (combatController != null)
                        combatController.TakeDamage(
                            Mathf.FloorToInt(damagePerSecond),
                            0,
                            Vector3.zero
                        );

                    playerNextDamage[other] = Time.time + 1f;
                }
            }
        }
    }
}

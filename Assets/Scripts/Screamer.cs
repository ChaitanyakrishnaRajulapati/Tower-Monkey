using UnityEngine;
using System.Collections.Generic;

public class ScreamerPulse : MonoBehaviour
{
    public float range = 6f;

    [Range(0f, 1f)]
    public float slowMultiplier = 0.6f;

    public float slowDuration = 1f;
    public float cooldown = 2f;

    float timer = 0f;
    bool isSlowing = false;

    HashSet<EnemyNavMovement> slowedEnemies = new HashSet<EnemyNavMovement>();

    void Update()
    {
        timer += Time.deltaTime;

        if (isSlowing)
        {
            ApplySlow();

            if (timer >= slowDuration)
            {
                timer = 0f;
                isSlowing = false;

                foreach (var e in slowedEnemies)
                    if (e != null) e.RemoveSlow();

                slowedEnemies.Clear();
            }
        }
        else
        {
            if (timer >= cooldown)
            {
                timer = 0f;
                isSlowing = true;
            }
        }
    }

    void ApplySlow()
    {
        foreach (var e in EnemyNavMovement.All)
        {
            if (e == null) continue;

            float d = Vector3.Distance(transform.position, e.transform.position);

            if (d <= range && !slowedEnemies.Contains(e))
            {
                e.ApplySlow(slowMultiplier);
                slowedEnemies.Add(e);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}


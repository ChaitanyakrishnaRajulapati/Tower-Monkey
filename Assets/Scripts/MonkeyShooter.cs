using UnityEngine;

public class MonkeyShooter : MonoBehaviour
{
    [Header("Targeting")]
    public float range = 7f;
    public string enemyTag = "Enemy";
    public Transform rotator;          // rotate only this (optional)
    public float turnSpeed = 10f;

    [Header("Fire Point")]
    public Transform firePoint;        // where projectile/laser starts

    [Header("Projectile Mode (default)")]
    public GameObject bulletPrefab;
    public float fireRate = 2f;        // shots per second

    [Header("Laser Mode (optional)")]
    public bool useLaser = false;
    public int laserDamagePerSecond = 20;
    public LineRenderer lineRenderer;  // optional (only if useLaser=true)

    Transform target;
    EnemyHealth targetHealth;
    float fireCountdown = 0f;

    void Start()
    {
        InvokeRepeating(nameof(UpdateTarget), 0f, 0.2f);
    }

    void Update()
    {
        if (target == null)
        {
            if (lineRenderer) lineRenderer.enabled = false;
            return;
        }

        // Rotate (Y only)
        if (rotator)
        {
            Vector3 dir = target.position - rotator.position;
            dir.y = 0f;
            Quaternion lookRot = Quaternion.LookRotation(dir);
            rotator.rotation = Quaternion.Lerp(rotator.rotation, lookRot, turnSpeed * Time.deltaTime);
        }

        if (useLaser)
        {
            LaserAttack();
        }
        else
        {
            BulletAttack();
        }
    }

    void UpdateTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);

        float shortest = Mathf.Infinity;
        GameObject nearest = null;

        foreach (GameObject e in enemies)
        {
            float d = Vector3.Distance(transform.position, e.transform.position);
            if (d < shortest)
            {
                shortest = d;
                nearest = e;
            }
        }

        if (nearest != null && shortest <= range)
        {
            target = nearest.transform;
            targetHealth = nearest.GetComponent<EnemyHealth>();
        }
        else
        {
            target = null;
            targetHealth = null;
        }
    }

    void BulletAttack()
    {
        if (bulletPrefab == null || firePoint == null) return;

        if (fireCountdown <= 0f)
        {
            Shoot();
            fireCountdown = 1f / fireRate;
        }

        fireCountdown -= Time.deltaTime;
    }

    void Shoot()
    {
        GameObject b = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Projectile p = b.GetComponent<Projectile>();
        if (p != null)
            p.SetTarget(target);
    }

    void LaserAttack()
    {
        if (targetHealth != null)
            targetHealth.TakeDamage(Mathf.RoundToInt(laserDamagePerSecond * Time.deltaTime));

        if (lineRenderer != null && firePoint != null)
        {
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, firePoint.position);
            lineRenderer.SetPosition(1, target.position);
        }
    }

    // Optional: show range circle in scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}

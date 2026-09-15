using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 20f;
    public int damage = 10;
    public float hitDistance = 0.3f;

    Transform target;

    public void SetTarget(Transform t)
    {
        target = t;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 dir = target.position - transform.position;
        float distThisFrame = speed * Time.deltaTime;

        if (dir.magnitude <= hitDistance)
        {
            HitTarget();
            return;
        }

        transform.Translate(dir.normalized * distThisFrame, Space.World);
        transform.LookAt(target);
    }

    void HitTarget()
    {
        EnemyHealth eh = target.GetComponent<EnemyHealth>();
        if (eh != null)
            eh.TakeDamage(damage);

        Destroy(gameObject);
    }
}


using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class EnemyNavMovement : MonoBehaviour
{
    public static List<EnemyNavMovement> All = new List<EnemyNavMovement>();

    public Transform goal;

    private NavMeshAgent agent;

    float originalSpeed;
    bool speedSaved = false;

    public float reachDist = 0.8f;

    void OnEnable()
    {
        if (!All.Contains(this))
            All.Add(this);
    }

    void OnDisable()
    {
        All.Remove(this);
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (agent != null)
            originalSpeed = agent.speed;

        if (goal != null && agent != null)
            agent.SetDestination(goal.position);
    }

    void Update()
    {
        if (agent == null || goal == null) return;

agent.SetDestination(goal.position);

float dist = Vector3.Distance(transform.position, goal.position);
float reach = Mathf.Max(reachDist, agent.stoppingDistance + 0.2f);

if (!agent.pathPending && (agent.remainingDistance <= reach || dist <= reach))
{
    if (PlayerHealth.Instance != null)
        PlayerHealth.Instance.TakeDamage(1);

    EnemyReward r = GetComponent<EnemyReward>();
    if (r != null) r.DestroyNoReward();
    else Destroy(gameObject);
}

        
    }

    public void ApplySlow(float mult)
    {
        if (agent == null) return;

        if (!speedSaved)
        {
            originalSpeed = agent.speed;
            speedSaved = true;
        }

        agent.speed = originalSpeed * mult;
    }

    public void RemoveSlow()
    {
        if (agent == null) return;

        if (speedSaved)
            agent.speed = originalSpeed;
    }
}









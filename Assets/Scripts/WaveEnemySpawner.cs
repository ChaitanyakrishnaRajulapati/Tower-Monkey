using UnityEngine;

public class WaveEnemyTracker : MonoBehaviour
{
    private WaveSpawner spawner;

    public void Init(WaveSpawner s)
    {
        spawner = s;
    }

    void OnDestroy()
    {
        if (spawner != null)
            spawner.NotifyEnemyDied();
    }
}


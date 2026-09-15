using UnityEngine;

public class EnemyReward : MonoBehaviour
{
    public int coinReward = 5;

    private bool shouldReward = false;

    // Call this ONLY when enemy dies (HP <= 0)
    public void RewardAndDestroy()
    {
        shouldReward = true;
        Destroy(gameObject);
    }

    // Call this when enemy reaches end (escape)
    public void DestroyNoReward()
    {
        shouldReward = false;
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (!shouldReward) return;
        if (CoinManager.Instance != null)
            CoinManager.Instance.Add(coinReward);
    }
}

using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int hp = 100;

    public void TakeDamage(int dmg)
    {
        hp -= dmg;
        if (hp <= 0)
        {
            var reward = GetComponent<EnemyReward>();
            if (reward != null) reward.RewardAndDestroy();
            else Destroy(gameObject);
        }
    }
}




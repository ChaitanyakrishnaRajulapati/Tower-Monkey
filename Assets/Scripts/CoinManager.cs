using UnityEngine;
using System;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;

    [Header("Starting Coins")]
    [SerializeField] private int coins = 100;

    public int Coins => coins;                 // read-only outside
    public event Action<int> OnCoinsChanged;   // UI listens to this

    void Awake()
    {
        // ✅ Prevent duplicates
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        // ✅ Push initial value to UI
        OnCoinsChanged?.Invoke(coins);
    }

    public bool Spend(int amount)
    {
        if (amount <= 0) return true;
        if (coins < amount) return false;

        coins -= amount;
        OnCoinsChanged?.Invoke(coins);
        return true;
    }

    public void Add(int amount)
    {
        if (amount <= 0) return;

        coins += amount;
        OnCoinsChanged?.Invoke(coins);
    }
}



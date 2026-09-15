using UnityEngine;
using TMPro;

public class CoinsUI : MonoBehaviour
{
    public TextMeshProUGUI coinsText;

    void Start()
    {
        if (coinsText == null)
            coinsText = GetComponent<TextMeshProUGUI>();

        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.OnCoinsChanged += UpdateCoins;
            UpdateCoins(CoinManager.Instance.Coins); // initial update
        }
    }

    void OnDestroy()
    {
        if (CoinManager.Instance != null)
            CoinManager.Instance.OnCoinsChanged -= UpdateCoins;
    }

    void UpdateCoins(int value)
    {
        coinsText.text = "Coins: " + value;
    }
}



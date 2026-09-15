using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance;

    public int maxHP = 5;
    public int hp;

    public TextMeshProUGUI hpText;
    public GameObject gameOverPanel;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        hp = maxHP;
        UpdateUI();

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    public void TakeDamage(int dmg)
    {
        hp -= dmg;

        if (hp < 0) hp = 0;

        UpdateUI();

        if (hp == 0)
        {
            GameOver();
        }
    }

    void UpdateUI()
    {
        if (hpText != null)
            hpText.text = "HP: " + hp;
    }

    void GameOver()
    {
        Time.timeScale = 0f;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }
}

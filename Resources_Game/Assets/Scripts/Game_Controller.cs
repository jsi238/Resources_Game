using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public int money = 0;
    public int moneyGenerationRate = 10;
    public int moneyCap = 200;
    private int moneyLevel = 1;
    public int maxMoneyLevel = 5;

    private int ultimateCooldownTime = 30;
    private float currentCooldownTime = 0f;
    private bool canUseUltimate = false;

    public int ultimateCost = 100;

    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI ultimateStatusText;
    public TextMeshProUGUI moneyLevelText;
    public Button levelUpButton;
    public Button ultimateButton;
    public TextMeshProUGUI gameOverText;
    public Button restartButton;

    public int levelUpCostMultiplier = 50;
    public GameObject[] walletLevelIndicators;

    public Character_Manager allyBase;
    public Character_Manager enemyBase;

    public Image ultimateVisualEffect;
    private Camera mainCamera;
    private bool gameOver = false;

    public float shakeMagnitude = 0.2f;
    public float shakeDuration = 0.5f;

    void Start()
    {
        gameOver = false;
        mainCamera = Camera.main;

        StartCoroutine(GenerateMoney());
        UpdateUI();

        levelUpButton.onClick.AddListener(LevelUpMoney);
        ultimateButton.onClick.AddListener(UseUltimatePower);

        restartButton.onClick.AddListener(RestartGame);
        restartButton.gameObject.SetActive(false);
        gameOverText.gameObject.SetActive(false);

        ActivateWalletLevelIndicator();
    }

    IEnumerator GenerateMoney()
    {
        while (!gameOver)
        {
            if (money < moneyCap)
            {
                money += moneyGenerationRate;
                if (money > moneyCap) money = moneyCap;
            }
            UpdateUI();
            yield return new WaitForSeconds(1f);
        }
    }

    public void LevelUpMoney()
    {
        int upgradeCost = GetLevelUpCost();
        if (money >= upgradeCost && moneyLevel < maxMoneyLevel)
        {
            moneyLevel++;
            moneyGenerationRate += 5;
            moneyCap += 50;
            money -= upgradeCost;

            ActivateWalletLevelIndicator();
            UpdateUI();
        }
    }

    private int GetLevelUpCost()
    {
        return moneyLevel * levelUpCostMultiplier;
    }

    public void UseUltimatePower()
    {
        if (canUseUltimate && money >= ultimateCost)
        {
            money -= ultimateCost;

            StartCoroutine(ActivateUltimateVisualEffect());
            StartCoroutine(CameraShake(shakeDuration, shakeMagnitude));

            DealDamageToEnemies(30); // change ult damage

            currentCooldownTime = 0f;
            canUseUltimate = false;
            UpdateUI();
        }
    }

    void DealDamageToEnemies(float damageAmount)
    {
        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in allEnemies)
        {
            if (enemy.name == "Enemy Base") continue;

            Character_Manager enemyManager = enemy.GetComponent<Character_Manager>();

            if (enemyManager != null)
            {
                enemyManager.DealDamage(enemy, damageAmount);
            }
        }
    }

    IEnumerator ActivateUltimateVisualEffect()
    {
        if (ultimateVisualEffect != null)
        {
            ultimateVisualEffect.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            ultimateVisualEffect.gameObject.SetActive(false);
        }
    }

    IEnumerator CameraShake(float duration, float magnitude)
    {
        Vector3 originalPos = mainCamera.transform.position;
        float elapsedTime = 0.0f;

        while (elapsedTime < duration)
        {
            float xShake = Random.Range(-1f, 1f) * magnitude;
            float yShake = Random.Range(-1f, 1f) * magnitude;

            mainCamera.transform.position = new Vector3(originalPos.x + xShake, originalPos.y + yShake, originalPos.z);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.position = originalPos;
    }

    void Update()
    {
        if (!gameOver)
        {
            CheckGameOver();
        }

        if (!canUseUltimate)
        {
            currentCooldownTime += Time.deltaTime;
            if (currentCooldownTime >= ultimateCooldownTime)
            {
                canUseUltimate = true;
                currentCooldownTime = ultimateCooldownTime;
            }
            UpdateUI();
        }
    }

    private void CheckGameOver()
    {
        if (allyBase.IsDead())
        {
            SetGameOver("Lose!");
        }
        else if (enemyBase.IsDead())
        {
            SetGameOver("Win!");
        }
    }

    private void SetGameOver(string resultMessage)
    {
        gameOver = true;
        gameOverText.text = resultMessage;
        gameOverText.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);
        DisableAllControls();
    }

    private void DisableAllControls()
    {
        levelUpButton.interactable = false;
        ultimateButton.interactable = false;
        StopAllCoroutines();
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void UpdateUI()
    {
        if (moneyText != null)
            moneyText.text = "Money: $" + money + " / $" + moneyCap;

        if (ultimateStatusText != null)
        {
            if (canUseUltimate)
            {
                if (money >= ultimateCost)
                {
                    ultimateStatusText.text = "Ready\nCost: $" + ultimateCost;
                }
                else
                {
                    ultimateStatusText.text = "Ready\nCost: $" + ultimateCost;
                }
            }
            else
            {
                ultimateStatusText.text = "Timer: " + Mathf.CeilToInt(ultimateCooldownTime - currentCooldownTime) + "s\nCost: $" + ultimateCost;
            }
        }

        if (moneyLevelText != null)
            moneyLevelText.text = "Level: " + moneyLevel + "/" + maxMoneyLevel + "\nCost: $" + GetLevelUpCost();

        levelUpButton.interactable = money >= GetLevelUpCost() && moneyLevel < maxMoneyLevel;
        ultimateButton.interactable = canUseUltimate && money >= ultimateCost;
    }

    private void ActivateWalletLevelIndicator()
    {
        if (walletLevelIndicators != null && moneyLevel - 1 < walletLevelIndicators.Length)
        {
            GameObject indicator = walletLevelIndicators[moneyLevel - 1];
            if (indicator != null)
            {
                indicator.SetActive(true);
            }
        }
    }

    public bool SpendMoney(int amount)
    {
        if (money >= amount)
        {
            money -= amount;
            UpdateUI();
            return true;
        }
        return false;
    }
}

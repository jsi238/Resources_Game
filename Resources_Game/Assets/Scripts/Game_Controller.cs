using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public int money = 0;
    public int moneyGenerationRate = 10;
    public int moneyCap = 200;
    private int moneyLevel = 1;
    public int maxMoneyLevel = 5;

    private int ultimateCooldownTime = 30;
    private float currentCooldownTime = 0f;
    private bool canUseUltimate = true;

    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI ultimateStatusText;
    public TextMeshProUGUI moneyLevelText;
    public Button levelUpButton;
    public Button ultimateButton;

    public int levelUpCostMultiplier = 50;
    public GameObject[] walletLevelIndicators;

    private bool gameOver = false;

    public Image ultimateVisualEffect;
    private Camera mainCamera;

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

        ActivateWalletLevelIndicator();
    }

    IEnumerator GenerateMoney()
    {
        while (true)
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
            moneyGenerationRate += 10;
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
        if (canUseUltimate)
        {
            StartCoroutine(ActivateUltimateVisualEffect());
            StartCoroutine(CameraShake(shakeDuration, shakeMagnitude));

            currentCooldownTime = 0f;
            canUseUltimate = false;
            UpdateUI();
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

    private void UpdateUI()
    {
        if (moneyText != null)
            moneyText.text = "Money: $" + money + " / $" + moneyCap;

        if (ultimateStatusText != null)
            ultimateStatusText.text = canUseUltimate ? "Ultimate Ready" : "Ultimate Cooldown: " + Mathf.CeilToInt(ultimateCooldownTime - currentCooldownTime) + "s";

        if (moneyLevelText != null)
            moneyLevelText.text = "Wallet Level: " + moneyLevel + "/" + maxMoneyLevel + "\nUpgrade Cost: $" + GetLevelUpCost();

        levelUpButton.interactable = money >= GetLevelUpCost() && moneyLevel < maxMoneyLevel;
        ultimateButton.interactable = canUseUltimate;
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

    public bool getGameOverStatus()
    {
        return gameOver;
    }

    public void setGameOverStatus(bool value)
    {
        gameOver = value;
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

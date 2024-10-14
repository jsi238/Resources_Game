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
    void Start()
    {
        gameOver = false;

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
            currentCooldownTime = 0f;
            canUseUltimate = false;
            UpdateUI();
        }
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
}

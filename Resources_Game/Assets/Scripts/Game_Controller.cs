using System.Collections;
using UnityEngine;
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

    public Text moneyText;
    public Text ultimateStatusText;

    void Start()
    {
        StartCoroutine(GenerateMoney());
        UpdateUI();
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
        if (moneyLevel < maxMoneyLevel)
        {
            moneyLevel++;
            moneyGenerationRate += 10;
            moneyCap += 50;
            UpdateUI();
        }
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
        moneyText.text = "Money: $" + money + " / $" + moneyCap;
        ultimateStatusText.text = canUseUltimate ? "Ultimate Ready" : "Ultimate Cooldown: " + Mathf.CeilToInt(ultimateCooldownTime - currentCooldownTime) + "s";
    }
}

using UnityEngine;
using UnityEngine.UI;

public class CooldownTimer : MonoBehaviour
{
    public int cooldownDuration = 5;
    public Text cooldownText;
    public Button cooldownButton;
    public int price;
    public Text priceText;

    private GameController gameController;
    private bool isOnCooldown = false;
    private float cooldownTimeLeft;

    void Start()
    {
        gameController = FindObjectOfType<GameController>();

        if (priceText != null)
        {
            priceText.text = "$" + price;
        }

        cooldownButton.onClick.AddListener(OnButtonClick);
        cooldownText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isOnCooldown)
        {
            cooldownTimeLeft -= Time.deltaTime;
            cooldownText.text = Mathf.Ceil(cooldownTimeLeft).ToString();

            if (cooldownTimeLeft <= 0)
            {
                EndCooldown();
            }
        }

        cooldownButton.interactable = gameController.money >= price && !isOnCooldown;
    }

    void OnButtonClick()
    {
        if (gameController.SpendMoney(price))
        {
            StartCooldown();
        }
    }

    void StartCooldown()
    {
        isOnCooldown = true;
        cooldownTimeLeft = cooldownDuration;
        cooldownButton.interactable = false;
        cooldownText.gameObject.SetActive(true);
    }

    void EndCooldown()
    {
        isOnCooldown = false;
        cooldownText.gameObject.SetActive(false);
        cooldownText.text = "";
        cooldownButton.interactable = true;
    }
}

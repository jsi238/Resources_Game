using UnityEngine;
using UnityEngine.UI;

public class Cooldown_Timer : MonoBehaviour
{
    public int cooldownDuration = 5;
    public Text cooldownText;
    public Button cooldownButton;

    private bool isOnCooldown = false;
    private float cooldownTimeLeft;

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
    }

    public void StartCooldown()
    {
        if (!isOnCooldown)
        {
            isOnCooldown = true;
            cooldownTimeLeft = cooldownDuration;
            cooldownButton.interactable = false;
            cooldownText.gameObject.SetActive(true);
        }
    }

    private void EndCooldown()
    {
        isOnCooldown = false;
        cooldownText.gameObject.SetActive(false);
        cooldownText.text = "";
        cooldownButton.interactable = true;
    }
}

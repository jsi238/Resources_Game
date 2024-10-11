using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Health_Manager : MonoBehaviour
{
    private GameObject parent;

    private RectTransform rt;

    private float health;
    private float maxHealth;

    // Start is called before the first frame update
    void Start()
    {
        maxHealth = this.gameObject.GetComponentInParent<Movement>().getMaxHealth();
        rt = this.GetComponent<RectTransform>();

        Debug.Log(rt.rect.width);

    }

    // Update is called once per frame
    void Update()
    {
        health = this.gameObject.GetComponentInParent<Movement>().getHealthPoints();

        rt.sizeDelta = new Vector2 (health/maxHealth, 0);

        Debug.Log(rt.rect.width);
    }
}

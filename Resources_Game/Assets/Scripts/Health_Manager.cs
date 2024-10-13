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
        maxHealth = this.gameObject.GetComponentInParent<Character_Manager>().getMaxHitPoints();
        rt = this.GetComponent<RectTransform>();

        //Debug.Log(rt.rect.width);

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void takeDamage()
    {
        health = this.gameObject.GetComponentInParent<Character_Manager>().getCurrentHitPoints();

        rt.sizeDelta = new Vector2(rt.rect.width * (health / maxHealth), rt.rect.height);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Movement : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float maxHealth;
    [SerializeField] private float healthPoints;
    [SerializeField] private float attackSpeed;
    [SerializeField] public float damage;

    private float contactTime = 0;
    private bool isTouching = false;

    private GameObject enemy;

    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(this.gameObject.name + " has " + healthPoints + " remaining.");

        if (!isTouching)
        {
            rb.velocity = Vector3.right * moveSpeed;
        }
        else
        {
            rb.velocity = Vector3.zero;
        }

        if (healthPoints <= 0)
        {
            this.gameObject.SetActive(false);
        }
    }

    public float getMaxHealth()
    {
        return maxHealth;
    }

    public float getHealthPoints()
    {
        {
            return healthPoints;
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        isTouching = true;
        enemy = collision.gameObject;
        Debug.Log("bonk");
    }
    public void OnCollisionStay2D(Collision2D collision)
    {
        if (enemy.tag != this.tag)
        {
            //Debug.Log(contactTime);
            if (contactTime >= enemy.GetComponent<Movement>().attackSpeed)
            {
                healthPoints -= enemy.GetComponent<Movement>().damage;
                contactTime = 0;
            }
            contactTime += Time.deltaTime;
        }
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        isTouching = false;
    }
}

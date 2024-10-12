using UnityEngine;

public class Character_Manager : MonoBehaviour
{
    // WIP: distance ally/enemy should stop from opposing team to handle ranged and melee
    public float stoppingDistance = 1.0f;
    public float moveSpeed = 2.0f;
    private Collider2D myCollider;

    void Start()
    {
        myCollider = GetComponent<Collider2D>();
        Collider2D[] nearbyObjects = FindObjectsOfType<Collider2D>();
        foreach (var collider in nearbyObjects)
        {
            if (collider.gameObject != this.gameObject && collider.gameObject.tag == gameObject.tag)
            {
                Physics2D.IgnoreCollision(collider, myCollider);
            }
        }
    }

    void Update()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector2.Distance(transform.position, enemy.transform.position);
            Character_Manager enemyManager = enemy.GetComponent<Character_Manager>();

            if (enemyManager != null)
            {
                float effectiveStoppingDistance = Mathf.Max(stoppingDistance, enemyManager.stoppingDistance);

                if (distanceToEnemy > effectiveStoppingDistance)
                {
                    MoveTowardsEnemy(enemy);
                }
                else
                {
                    StopMovement();
                }
            }
        }
    }

    void MoveTowardsEnemy(GameObject target)
    {
        Vector2 direction = (target.transform.position - transform.position).normalized;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * moveSpeed;
        }
    }

    void StopMovement()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == gameObject.tag)
        {
            Physics2D.IgnoreCollision(collision.collider, myCollider);
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == gameObject.tag)
        {
            Physics2D.IgnoreCollision(collision.collider, myCollider);
        }
    }
}

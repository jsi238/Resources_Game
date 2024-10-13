using UnityEngine;

public class Character_Manager : MonoBehaviour
{
    // WIP: distance ally/enemy should stop from opposing team to handle ranged and melee
    public float stoppingDistance = 1.0f;
    public float moveSpeed = 2.0f;
    public float attackSpeed = 1.0f;
    public float damage = 2f;
    public float hitPoints = 10f;
    public float maxHitPoints = 10f;

    private float distanceToEnemy;
    private float minDistanceToEnemy = Mathf.Infinity;
    private GameObject closestTarget;

    private Collider2D myCollider;

    private float contactTime = 0; //checks how much time objects have been in contact with one another

    private Health_Manager health_manager;

    private bool isAlly;
    void Start()
    {
        if (gameObject.tag == "Ally")
        {
            isAlly = true;
        }
        else
        {
            isAlly = false;
        }

        Debug.Log("Is this object an ally: " + isAlly);

        //health_manager = this.gameObject.GetComponentInChildren<Health_Manager>();
        /*
        myCollider = GetComponent<Collider2D>();
        Collider2D[] nearbyObjects = FindObjectsOfType<Collider2D>();
        foreach (var collider in nearbyObjects)
        {
            if (collider.gameObject != this.gameObject && collider.gameObject.tag == gameObject.tag)
            {
                Physics2D.IgnoreCollision(collider, myCollider);
            }
        }
        */
    }

    void Update()
    {
        if (isAlly)
        {
            HandleMovement("Enemy"); //allies head towards enemies
        }
        else
        {
            HandleMovement("Ally"); //enemies head toward allies
        }
    }

    void HandleMovement(string target)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(target);

        //Debug.Log(enemies.Length);

        foreach (GameObject enemy in enemies)
        {
            distanceToEnemy = Mathf.Abs(Vector2.Distance(transform.position, enemy.transform.position)); //finds distance from center of ally to center of enemy

            if (distanceToEnemy < minDistanceToEnemy)
            {
                minDistanceToEnemy = distanceToEnemy;
                closestTarget = enemy;
            }

            Character_Manager enemyManager = enemy.GetComponent<Character_Manager>();

            if (enemyManager != null)
            {
                float effectiveStoppingDistance = Mathf.Max(stoppingDistance, enemyManager.stoppingDistance);

                //Debug.Log("Distance to enemy is: " + distanceToEnemy);


            }
            else
            {
                distanceToEnemy = Mathf.Infinity;
            }
        }
        if (minDistanceToEnemy > stoppingDistance)
        {
            MoveTowardsEnemy(closestTarget);
        }
        else
        {
            StopMovement();
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

    public float getMaxHitPoints()
    {
        return maxHitPoints;
    }

    public float getCurrentHitPoints()
    {
        return hitPoints;
    }

    public float getDamage()
    {
        return damage;
    }
}

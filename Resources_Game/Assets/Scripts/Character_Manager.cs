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
    private Animator animator;

    [SerializeField] private Sprite destroyedBase;

    private float contactTime = 0; //checks how much time objects have been in contact with one another

    private Health_Manager health_manager;

    private GameObject[] enemies;

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

        animator = GetComponent<Animator>();

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
            HandleMovement("Enemy Center"); //allies head towards enemies
        }
        else
        {
            HandleMovement("Ally Center"); //enemies head toward allies
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

        if (closestTarget != null)
        {
            if (minDistanceToEnemy > stoppingDistance)
            {
                MoveTowardsEnemy(closestTarget);
            }
            else
            {
                contactTime += Time.deltaTime;
                StopMovement();
                AttackEnemy(closestTarget);
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

    void AttackEnemy(GameObject target)
    {
        animator.SetBool("isAttacking", true);
        if (contactTime >= attackSpeed)
        {
            DamageTarget(target);
            contactTime = 0;
        }
    }

    public void DealDamage(GameObject target, float damage)
    {
        target.GetComponentInParent<Character_Manager>().hitPoints -= damage;
    }

    public void DamageTarget(GameObject target)
    {
        target.GetComponentInParent<Character_Manager>().DealDamage(target, damage);
    }

    public void CharacterDeath()
    {
        if (hitPoints == 0)
        {
            Destroy(this.gameObject, 1f);
        }

        //when an object dies, reset all enemies to search for next closest target
        foreach (GameObject enemy in enemies)
        {
            Character_Manager enemyManager = enemy.GetComponentInParent<Character_Manager>();
            //Animator enemyAnimator = enemy.GetComponentInParent<Animator>();

            enemyManager.SetMinDistance(Mathf.Infinity);
            //enemyAnimator.SetBool("isAttacking", false);
        }
    }

    public void AdjustHealth()
    {
        Slider health = this.GetComponentInChildren<Slider>();

        if (health != null)
        {
            health.value = health.maxValue * hitPoints / maxHitPoints;
        }
    }

    //Setters and getter that might be useful later
    public void SetMinDistance(float newDistance)
    {
        minDistanceToEnemy = newDistance;
    }
    public float GetMaxHitPoints()
    {
        return maxHitPoints;
    }

    public float GetCurrentHitPoints()
    {
        return hitPoints;
    }

    public float GetDamage()
    {
        return damage;
    }
}

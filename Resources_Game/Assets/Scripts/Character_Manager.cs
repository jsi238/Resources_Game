using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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

    [SerializeField] private Sprite destroyedTower;
    private Animator animator;
    [SerializeField] private AnimationClip attackAnimation;
    [SerializeField] private AnimationClip deathAnimation;

    private float contactTime = 0; //checks how much time objects have been in contact with one another

    private Health_Manager health_manager;

    private GameObject[] enemies;

    private bool isAlly;
    void Start()
    {
        hitPoints = maxHitPoints;

        if (gameObject.tag == "Ally")
        {
            isAlly = true;
        }
        else
        {
            isAlly = false;
        }

        animator = GetComponent<Animator>();

        attackSpeed = attackAnimation.length;
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

        CharacterDeath();

        AdjustHealth();
    }

    void HandleMovement(string target)
    {
        enemies = GameObject.FindGameObjectsWithTag(target);

        if (isAlly)
        {
            Debug.Log("Number of enemies detected: " + enemies.Length);
        }

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

        animator.SetBool("isMoving", true);
        animator.SetBool("isAttacking", false);
    }

    void StopMovement()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }

        animator.SetBool("isMoving", false);
    }

    void AttackEnemy(GameObject target)
    {
        animator.SetBool("isAttacking", true);
        if (contactTime >= attackSpeed && target.GetComponentInParent<Character_Manager>().GetCurrentHitPoints() > 0)
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
        if (hitPoints <= 0)
        {
            if (this.gameObject.name == "Allied Base" || this.gameObject.name == "Enemy Base")
            {
                this.gameObject.GetComponent<SpriteRenderer>().sprite = destroyedTower;
                //GameObject.Find("Game Controller").GetComponent<GameController>().setGameOverStatus(true);
            }
            else
            {
                animator.SetBool("isDead", true);
                animator.SetBool("isAttacking", false);
                contactTime = 0;
                Destroy(this.gameObject, deathAnimation.length);
            }
            
        }

        //when an object dies, reset all enemies to search for next closest target
        foreach (GameObject enemy in enemies)
        {
            Character_Manager enemyManager = enemy.GetComponentInParent<Character_Manager>();

            enemyManager.SetMinDistance(Mathf.Infinity);
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

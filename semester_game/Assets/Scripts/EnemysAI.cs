using UnityEngine;
using UnityEngine.AI;

public class EnemysAI : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;

    Animator enemyAnimator;

    //Enemy Stats
    public float damage = 10f;
    public float range = 100f;
    public float fireRate = 15f;

    //Shooting Flash
    public ParticleSystem muzzleFlash;

    //Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private void Awake()
    {
        player = GameObject.Find("PlayerObj").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        enemyAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInAttackRange && playerInSightRange) AttackPlayer();
    }

    private void Patroling()
    {
        Debug.Log("Patroling");

        if (!walkPointSet)
        {
            Debug.Log("On waypoint, searching for another one");

            SearchWalkPoint();
        }

        if (walkPointSet)
        {
            Debug.Log("Going to waypoint");
            enemyAnimator.SetBool("isWalking", true);
            agent.SetDestination(walkPoint);
        }
            
        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f)
        {
            Debug.Log("On waypoint");
            enemyAnimator.SetBool("isWalking", false);
            walkPointSet = false;
        }
            
    }

    private void SearchWalkPoint()
    {
        Debug.Log("Search Walk Point");

        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
        {
            Debug.Log("Waypoint is on ground");
            walkPointSet = true;
        }
    }

    private void ChasePlayer()
    {
        Debug.Log("Chase Player");

        enemyAnimator.SetBool("isChasingPlayer", true);
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        Debug.Log("Attack Player");

        agent.SetDestination(transform.position);

        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            //Attack code:
            enemyAnimator.SetBool("isShooting", true);
            muzzleFlash.Play();

            RaycastHit hitInfo;
            if(Physics.Raycast(transform.position, player.transform.position, out hitInfo, range))
            {
                Debug.Log(hitInfo.transform.name);

                PlayerController target = hitInfo.transform.GetComponent<PlayerController>();

                if(target != null)
                {
                    target.TakeDamage(damage);
                }
            }
            //================

            alreadyAttacked = true;
            enemyAnimator.SetBool("isShooting", false);
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
            enemyAnimator.SetBool("isReloading", true);
        }
    }

    private void ResetAttack()
    {
        Debug.Log("Reset Attack");
        alreadyAttacked = false;
    }
}

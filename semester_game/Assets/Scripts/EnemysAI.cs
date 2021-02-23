using UnityEngine;
using UnityEngine.AI;

public class EnemysAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public GameObject enemyHead;
    public GameObject enemyGun;
    public GameObject enemyArm;

    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;

    public ParticleSystem muzzleFlashAI;

    public Animator animator;

    // Patrolling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public float damage = 50;

    // Chasing
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if(!playerInSightRange && !playerInAttackRange)
        {
            animator.SetBool("isWalking", true);
            Patrolling();
        }

        if (playerInSightRange && !playerInAttackRange)
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isChasingPlayer", true);
            ChasePlayer();
        }

        if (playerInSightRange && playerInAttackRange)
        {
            AttackPlayer();
        }
    }

    private void Patrolling()
    {

        if(!walkPointSet)
        {
            SearchWalkPoint();
        }

        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
        }
    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        
        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
        {
            walkPointSet = true;
        }
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);

        transform.LookAt(player, transform.up);

        if (!alreadyAttacked)
        {
            Shoot();

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void Shoot()
    {
        muzzleFlashAI.Play();

        RaycastHit hitInfo;
        if(Physics.Raycast(transform.position, transform.forward, out hitInfo, attackRange))
        {
            Debug.Log(hitInfo.transform.name);

            PlayerController target = hitInfo.transform.GetComponent<PlayerController>();

            if(target != null)
            {
                target.TakeDamage(damage);
            }
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}

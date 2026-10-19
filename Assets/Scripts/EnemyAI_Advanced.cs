// EnemyAI_Advanced.cs
// ذكاء اصطناعي مُحسن مع حالات: Patrol, Chase, Attack
// يحتاج إلى NavMeshAgent وطرق Patrol (waypoints)

using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI_Advanced : MonoBehaviour
{
    public enum State { Idle, Patrol, Chase, Attack }
    public State state = State.Patrol;

    [Header("References")]
    public Transform[] patrolPoints;
    int currentPatrolIndex = 0;

    public Transform target; //player
    NavMeshAgent agent;

    [Header("Stats")]
    public int maxHealth = 120;
    int currentHealth;
    public float detectRadius = 20f;
    public float attackRadius = 12f;
    public float lostSightTime = 4f;
    float lastSeenTime = -999f;

    [Header("Combat")]
    public int damage = 10;
    public float attackInterval = 1.2f;
    float lastAttack = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentHealth = maxHealth;
        if (target == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) target = p.transform;
        }
    }

    void Update()
    {
        switch (state)
        {
            case State.Patrol: UpdatePatrol(); break;
            case State.Chase: UpdateChase(); break;
            case State.Attack: UpdateAttack(); break;
            case State.Idle: break;
        }

        EvaluateState();
    }

    void UpdatePatrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;
        agent.isStopped = false;
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
    }

    void UpdateChase()
    {
        if (target == null) return;
        agent.isStopped = false;
        agent.SetDestination(target.position);
    }

    void UpdateAttack()
    {
        if (target == null) return;
        agent.isStopped = true;
        transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));
        if (Time.time - lastAttack >= attackInterval)
        {
            lastAttack = Time.time;
            TryAttack();
        }
    }

    void TryAttack()
    {
        RaycastHit hit;
        Vector3 dir = (target.position + Vector3.up * 1.0f) - (transform.position + Vector3.up * 1.0f);
        if (Physics.Raycast(transform.position + Vector3.up * 1.0f, dir.normalized, out hit, attackRadius))
        {
            if (hit.collider.CompareTag("Player"))
            {
                var pc = hit.collider.GetComponent<PlayerController_Advanced>();
                if (pc != null) pc.TakeDamage(damage);
            }
        }
    }

    void EvaluateState()
    {
        if (target == null) return;
        float dist = Vector3.Distance(transform.position, target.position);
        bool canSee = CanSeeTarget();

        if (canSee)
        {
            lastSeenTime = Time.time;
            if (dist <= attackRadius) state = State.Attack;
            else state = State.Chase;
        }
        else
        {
            if (Time.time - lastSeenTime < lostSightTime) state = State.Chase;
            else state = State.Patrol;
        }
    }

    bool CanSeeTarget()
    {
        Vector3 dir = (target.position + Vector3.up*1.0f) - (transform.position + Vector3.up*1.0f);
        if (Vector3.Distance(transform.position, target.position) > detectRadius) return false;
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up*1.0f, dir.normalized, out hit, detectRadius))
        {
            return hit.collider.CompareTag("Player");
        }
        return false;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0) Die();
        else
        {
            // تتجه نحو اللاعب عند تلقي الضرر
            state = State.Chase;
        }
    }

    void Die()
    {
        // تاثيرات موت بسيطة
        agent.isStopped = true;
        Destroy(gameObject, 3f);
    }
}

using UnityEngine;
using UnityEngine.AI;

// EnemyAI.cs
// ذكاء اصطناعي بسيط: دورية (اختياري)، اكتشاف اللاعب، مطاردة وإطلاق/إلحاق ضرر
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    public int maxHealth = 100;
    int currentHealth;

    NavMeshAgent agent;
    Transform player;
    public float detectRadius = 15f;
    public float shootRadius = 10f;
    public int damage = 10;
    public float attackInterval = 1f;
    float lastAttack = 0f;

    public HUDController hud; // لاستخدام للاختبار أو الإحصاءات

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentHealth = maxHealth;
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p) player = p.transform;
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist <= detectRadius)
        {
            agent.SetDestination(player.position);

            if (dist <= shootRadius && Time.time - lastAttack >= attackInterval)
            {
                lastAttack = Time.time;
                TryAttackPlayer();
            }
        }
    }

    void TryAttackPlayer()
    {
        // هجوم بسيط: raycast للتحقق من خط الرؤية
        RaycastHit hit;
        Vector3 dir = (player.position + Vector3.up*1.0f) - (transform.position + Vector3.up*1.0f);
        if (Physics.Raycast(transform.position + Vector3.up*1.0f, dir.normalized, out hit, shootRadius))
        {
            if (hit.collider.CompareTag("Player"))
            {
                var pc = hit.collider.GetComponent<PlayerController>();
                if (pc != null) pc.TakeDamage(damage);
            }
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        // موت بسيط: تعطيل الوكيل وإظهار رسالة
        agent.isStopped = true;
        // يمكنك إضافة تأثيرات تفجير/انهيار هنا
        Destroy(gameObject, 2f);
    }
}

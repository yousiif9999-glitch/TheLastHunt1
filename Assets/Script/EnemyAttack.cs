using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public int damage = 10;
    public float attackDistance = 2f;
    public float attackCooldown = 1f; // يضرب كل ثانية

    private float nextAttackTime = 0f;
    private Transform player;

    private bool isActive = false; // هل الهجوم شغال؟

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        // تشغيل/إيقاف بالزر T
        if (Input.GetKeyDown(KeyCode.T))
        {
            isActive = !isActive;
            Debug.Log("Enemy Attack: " + (isActive ? "ON" : "OFF"));
        }

        if (!isActive) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= attackDistance && Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + attackCooldown;
            Attack();
        }
    }

    void Attack()
    {
        PlayerHealth ph = player.GetComponent<PlayerHealth>();

        if (ph != null)
        {
            ph.TakeDamage(damage);
        }
    }
}
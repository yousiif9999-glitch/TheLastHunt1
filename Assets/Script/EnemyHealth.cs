using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    [Header("Respawn")]
    public bool randomRespawn = true;
    public float respawnTime = 3f;
    public Transform[] spawnPoints;

    [Header("Drop")]
    public GameObject ammoDropPrefab;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (ammoDropPrefab != null)
        {
            Instantiate(ammoDropPrefab, transform.position + new Vector3(0, 1.5f, 0), Quaternion.identity);
        }

        gameObject.SetActive(false);
        Invoke(nameof(Respawn), respawnTime);
    }

    void Respawn()
    {
        currentHealth = maxHealth;

        if (randomRespawn && spawnPoints.Length > 0)
        {
            Transform randomPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            transform.position = randomPoint.position;
            transform.rotation = randomPoint.rotation;
        }

        gameObject.SetActive(true);
    }
}
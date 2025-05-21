using UnityEngine;

public class Block : MonoBehaviour
{
    public int maxHealth = 3; // ������������ ���������� ������ �����
    private int currentHealth; // ������� ���������� ������
    [SerializeField] private Transform bonus1;
    [SerializeField] private Transform bonus2;
    [SerializeField] private Transform bonus3;
    [SerializeField] private Transform bonus4;
    [SerializeField] private Transform bonus5;

    void Start()
    {
        // ������������� ������� �������� ������ �������������
        maxHealth = Random.Range(1, 4);
        currentHealth = maxHealth;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // ���������, ��� ������������ ��������� � �����
        if (collision.gameObject.CompareTag("Ball"))
        {
            TakeDamage();
        }
    }

    void TakeDamage()
    {
        // ��������� �������� �� 1
        currentHealth--;

        // ���� �������� �������� ����, ���������� ����
        if (currentHealth <= 0)
        {
            TryToSpawnBonuses();
            ScoreManager.AddScore(10);
            Destroy(gameObject);
        }
        else
        {
            // ��������� �������� ���� (��������, ����� ����)
            UpdateBlockAppearance();
        }
    }

    void TryToSpawnBonuses()
    {
        float spawnchance = 1f;
        Vector3 spawnpos = transform.position;
        Quaternion spawnrot = transform.rotation;
        float receivedChance = Random.Range(0f, 1f);
        int randInt = Random.Range(1, 6);
        if (receivedChance <= spawnchance)
        {
            if (randInt == 1)
            {
                Instantiate(bonus1, spawnpos, spawnrot);
            }
            if (randInt == 2)
            {
                Instantiate(bonus2, spawnpos, spawnrot);
            }
            if (randInt == 3)
            {
                Instantiate(bonus3, spawnpos, spawnrot);
            }
            if (randInt == 4)
            {
                Instantiate(bonus4, spawnpos, spawnrot);
            }
            if (randInt == 5)
            {
                Instantiate(bonus5, spawnpos, spawnrot);
            }
        }
    }

    void UpdateBlockAppearance()
    {
        //// �������� ���� ����� � ����������� �� ����������� ��������
        //SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        //if (renderer != null)
        //{
        //    switch (currentHealth)
        //    {
        //        case 2:
        //            renderer.color = Color.yellow; // ������ ���� ��� 2 ������
        //            break;
        //        case 1:
        //            renderer.color = Color.red; // ������� ���� ��� 1 �����
        //            break;
        //    }
        //}
    }
}
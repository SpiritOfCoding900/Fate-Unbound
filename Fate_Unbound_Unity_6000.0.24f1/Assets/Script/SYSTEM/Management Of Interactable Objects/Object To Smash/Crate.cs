using System.Collections.Generic;
using UnityEngine;

public class Crate : MonoBehaviour, IDamagable
{
    public float HP = 1f;

    [Header("Loot Table Of Items: ")]
    public List<LootDrop> lootTable;

    public void TakeDamage(float damage)
    {
        AudioManager.Instance.SFXSound(SoundID.Confirm);
        HP -= damage;
    }

    void Update()
    {
        EnemyDead();
    }

    public void EnemyDead()
    {
        if (HP <= 0)
        {
            AudioManager.Instance.SFXSound(SoundID.Cancel);
            // Drop Loot
            foreach (var loot in lootTable)
            {
                float roll = Random.Range(0f, 100f);
                if (roll <= loot.dropChance && loot.lootPrefab != null)
                {
                    Instantiate(loot.lootPrefab, transform.position, Quaternion.identity);
                    break; // Drop only one item; remove this if multiple drops allowed
                }
            }

            // Death
            GetComponent<Collider2D>().enabled = false;
            this.enabled = false;
            Destroy(gameObject, 1.5f);
        }
    }
}
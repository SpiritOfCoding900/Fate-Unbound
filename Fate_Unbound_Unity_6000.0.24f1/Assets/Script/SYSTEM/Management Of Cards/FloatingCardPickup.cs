using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FloatingCardPickup : MonoBehaviour
{
    public CardData cardData;

    [SerializeField] private string playerTag = "Player";

    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        if (cardData == null) return;

        // Find the inventory/equipment on the player
        var equipment = other.GetComponent<CardEquipment>();
        if (equipment == null) return;

        bool equipped = equipment.TryEquip(cardData);
        if (equipped)
        {
            Destroy(gameObject); // only destroy if it successfully equipped
        }
        // else: do nothing (inventory full)
    }
}
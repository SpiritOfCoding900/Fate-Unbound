using UnityEngine;

public class CardEquipment : MonoBehaviour
{
    [SerializeField] private int maxSlots = 5;

    [Header("UI")]
    [SerializeField] private CardSlotsUI slotsUI;

    [SerializeField] private Player player;
    private CardData[] equipped;

    private void Awake()
    {
        if (player == null) player = GetComponent<Player>();
        equipped = new CardData[maxSlots];
        if (slotsUI != null) slotsUI.Refresh(equipped);

        RecalculateAndApplyStats(); // initial
    }

    void Update()
    {
        if (!Input.GetKey(KeyCode.B)) return;

        if (Input.GetKeyDown(KeyCode.Alpha1)) UnequipSlot(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) UnequipSlot(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3)) UnequipSlot(2);
        else if (Input.GetKeyDown(KeyCode.Alpha4)) UnequipSlot(3);
        else if (Input.GetKeyDown(KeyCode.Alpha5)) UnequipSlot(4);
    }

    public bool TryEquip(CardData card)
    {
        if (card == null) return false;

        // Find first empty slot (left to right)
        for (int i = 0; i < equipped.Length; i++)
        {
            if (equipped[i] == null)
            {
                equipped[i] = card;

                // Update UI
                if (slotsUI != null)
                    slotsUI.Refresh(equipped);

                // Apply stats immediately after equipping
                RecalculateAndApplyStats();

                return true;
            }
        }

        // No empty slot -> cannot equip
        return false;
    }

    public void UnequipSlot(int index)
    {
        if (index < 0 || index >= equipped.Length) return;
        if (equipped[index] == null) return;

        equipped[index] = null;

        CompactLeft(); // Keeps slots packed left

        if (slotsUI != null)
            slotsUI.Refresh(equipped);

        RecalculateAndApplyStats();
    }

    private void CompactLeft()
    {
        int write = 0;
        for (int read = 0; read < equipped.Length; read++)
        {
            if (equipped[read] != null)
            {
                equipped[write] = equipped[read];
                if (write != read) equipped[read] = null;
                write++;
            }
        }
    }

    private void RecalculateAndApplyStats()
    {
        int totalHP = 0;
        int totalATK = 0;
        int totalDEF = 0;

        for (int i = 0; i < equipped.Length; i++)
        {
            var c = equipped[i];
            if (c == null) continue;

            totalHP += c.hpBonus;
            totalATK += c.atkBonus;
            totalDEF += c.defBonus;
        }

        if (player != null)
            player.ApplyCardBonuses(totalHP, totalATK, totalDEF);
    }

    public bool IsFull()
    {
        for (int i = 0; i < equipped.Length; i++)
            if (equipped[i] == null) return false;
        return true;
    }
}
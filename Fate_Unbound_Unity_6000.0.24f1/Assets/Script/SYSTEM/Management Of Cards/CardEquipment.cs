using UnityEngine;

public class CardEquipment : MonoBehaviour
{
    [SerializeField] private int maxSlots = 5;

    [Header("UI")]
    [SerializeField] private CardSlotsUI slotsUI;

    private CardData[] equipped;

    private void Awake()
    {
        equipped = new CardData[maxSlots];
        if (slotsUI != null)
            slotsUI.Refresh(equipped);
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

                // Later: Apply stats/passives here
                // ApplyCard(card);

                return true;
            }
        }

        // No empty slot -> cannot equip
        return false;
    }

    public bool IsFull()
    {
        for (int i = 0; i < equipped.Length; i++)
            if (equipped[i] == null) return false;
        return true;
    }
}
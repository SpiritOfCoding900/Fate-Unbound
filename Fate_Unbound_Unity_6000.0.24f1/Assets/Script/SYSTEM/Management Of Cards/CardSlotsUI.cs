using UnityEngine;
using UnityEngine.UI;

public class CardSlotsUI : MonoBehaviour
{
    [SerializeField] private Image[] slotImages; // size 5, left to right
    [SerializeField] private Sprite emptySprite; // optional

    public void Refresh(CardData[] equipped)
    {
        for (int i = 0; i < slotImages.Length; i++)
        {
            var img = slotImages[i];
            if (img == null) continue;

            CardData data = (equipped != null && i < equipped.Length) ? equipped[i] : null;

            if (data != null && data.icon != null)
            {
                img.sprite = data.icon;
                img.enabled = true;
            }
            else
            {
                if (emptySprite != null)
                {
                    img.sprite = emptySprite;
                    img.enabled = true;
                }
                else
                {
                    // If you want truly empty: hide the image
                    img.enabled = false;
                }
            }
        }
    }
}
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Card Data")]
public class CardData : ScriptableObject
{
    public string cardName;
    public Sprite icon;

    [Header("Stats")]
    public int atkBonus;
    public int defBonus;
    public int hpBonus;

    // Later:
    // public PassiveEffect passive;

    [Header("Primary Description")]
    [TextArea(3, 5)]
    public string PriDesc;

    [Header("Secondary Description")]
    [TextArea(3, 5)]
    public string SecDesc;
}
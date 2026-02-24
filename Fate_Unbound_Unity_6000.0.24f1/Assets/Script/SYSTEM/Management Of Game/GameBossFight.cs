using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameBossFight : MonoBehaviour
{
    public GameUIID openUIForWinning = GameUIID.YouWin;
    public GameUIID openUIForLosing = GameUIID.YouLose;
    public string sceneAfterWinning = "putSceneName";
    public Player pm;               // Reference to the player.
    public GameObject bossEnemy;    // Assign the boss enemy in the inspector.

    private bool hasEnded = false;

    void Start()
    {
        pm = FindObjectOfType<Player>(); // You can replace this with singleton or drag via inspector
    }

    void Update()
    {
        if (hasEnded) return;

        // Check if player has died
        if (pm.CurrentHP <= 0 && pm != null)
        {
            hasEnded = true;
            UIManager.Instance.OpenReplace(openUIForLosing);
            Destroy(pm.gameObject);
        }

        // Check if boss has been defeated
        if (bossEnemy == null)
        {
            hasEnded = true;
            UIManager.Instance.OpenReplace(openUIForWinning);
        }
    }
}
using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameHeadCount : SimpleSingleton<GameHeadCount>
{
    public TMP_Text headCountText; // Assign this in the Inspector
    public TMP_Text headCountRequirementText; // Assign this in the Inspector
    public GameUIID openUIForWinning = GameUIID.YouWin;
    public GameUIID openUIForLosing = GameUIID.YouLose;
    public string sceneAfterWinning = "putSceneName";
    public int killsSoFar = 0; // Amount Of Kills Needed
    public int killsRequired = 10; // Amount Of Kills Needed
    private bool requirementFulfilled = false;

    public Player pm;
    //public List<Enemy> Enemies = new List<Enemy>();

    private bool hasEnded = false;



    protected virtual void Start()
    {
        pm = FindObjectOfType<Player>();
        UpdateKillText(); // Initialize display
    }

    void Update()
    {
        if (hasEnded) return;

        // Check for player defeat
        if (pm.HP <= 0)
        {
            UIManager.Instance.OpenReplace(openUIForLosing);
            Destroy(pm.gameObject, 0.75f);
            hasEnded = true;
        }

        // Win condition has not yet met
        if (killsSoFar < killsRequired) return;

        // Win condition check
        if (killsSoFar >= killsRequired)
        {
            Debug.Log("Kill Requirements Met");
            UIManager.Instance.OpenReplace(openUIForWinning);
            hasEnded = true;
        }
    }

    private void UpdateKillText()
    {
        if (headCountText != null)
            headCountText.text = "Kills: " + killsSoFar;

        if (headCountRequirementText != null)
            headCountRequirementText.text = "Goal: " + killsRequired;
    }
}

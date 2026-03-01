using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLevelUpStats : MonoBehaviour
{
    public static PlayerLevelUpStats Instance;

    // Gold
    public int Gold = 0;


    private void Start()
    {
        Gold = 0;
    }

    private void Awake()
    {
        Instance = this; // Inserting this into the Static Pigeon hole.
    }

    private void OnDestroy()
    {
        Instance = null;
    }
}

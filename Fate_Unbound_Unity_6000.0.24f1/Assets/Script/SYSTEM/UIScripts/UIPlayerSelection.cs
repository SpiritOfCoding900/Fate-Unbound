using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

[System.Serializable]
public class CharacterCardUI
{
    public string NameOfClass = "Put Class Name Here";
    public TMP_Text nameText;
    public TMP_Text hpText;
    public TMP_Text mpText;
    public TMP_Text atkText;
    public TMP_Text defText;
    public TMP_Text dodgeRateText;
    public TMP_Text speedText;
    public TMP_Text descriptionText;
}

[System.Serializable]
public class WeaponSet
{
    public List<Weapon> weapons;
}


public class UIPlayerSelection : MonoBehaviour
{
    [Header("Find GameManager")]
    public GameManager GM;

    [Header("Character Select")]
    public List<CharacterCardUI> characterCards; // Assign 3 elements in the inspector

    [Header("Go To Scene")]
    public string putSceneName = "Tutorial Stage 1 - Move Roll Attack";

    ///test
    //public Animator anim;

    private void Awake()
    {
        GM = FindFirstObjectByType<GameManager>(); // Unity 2022+
        // GM = FindObjectOfType<GameManager>();   // older Unity

        if (GM == null)
            Debug.LogError("No GameManager found in the scene!", this);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PauseGame();
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        UIManager.Instance.CloseAll();
    }

    public void PauseGame()
    {
        //Time.timeScale = 0f;

        var classList = CharacterLoader.Instance.myClassList;

        for (int i = 0; i < characterCards.Count; i++)
        {
            if (i < classList.classes.Count)
            {
                var data = classList.classes[i];
                var ui = characterCards[i];

                ui.NameOfClass = data.className;
                ui.nameText.text = data.className;
                ui.hpText.text = data.MaxHP.ToString();
                ui.mpText.text = data.MaxMP.ToString();
                ui.atkText.text = data.ATK.ToString();
                ui.defText.text = data.DEF.ToString();
                ui.dodgeRateText.text = data.dodgeRate.ToString();
                ui.speedText.text = data.moveSpeed.ToString();
                ui.descriptionText.text = data.description;
            }
        }
    }

    public void ChosenBerserkerClass() => ChooseClass(0);
    public void ChosenRangerClass() => ChooseClass(1);
    public void ChosenPaladinClass() => ChooseClass(2);

    private void ChooseClass(int index)
    {
        var characterData = CharacterLoader.Instance.myClassList.classes[index];

        // Store the chosen class into GameManager (safe even if player not spawned yet)
        GameManager.PlayerClassStats stats = new GameManager.PlayerClassStats
        {
            className = characterData.className,
            MaxHP = characterData.MaxHP,
            MaxMP = characterData.MaxMP,
            ATK = characterData.ATK,
            DEF = characterData.DEF,
            dodgeRate = characterData.dodgeRate,
            moveSpeed = characterData.moveSpeed,
            description = characterData.description,
            ID = characterData.ID
        };

        GM.SetChosenClass(stats);

        Time.timeScale = 1f;
        UIManager.Instance.CloseAll();
        AudioManager.Instance.SFXSound(SoundID.Confirm);

        if (!string.IsNullOrEmpty(putSceneName))
            SceneManager.LoadScene(putSceneName);
    }

}

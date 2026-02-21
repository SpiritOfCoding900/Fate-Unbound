using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIWinScreen : MonoBehaviour
{
    public Button btnPlay;
    public string putSceneName;

    void Start()
    {
        // Try to use singleton if available
        ISceneTargetProvider provider = null;

        if (GameTimer.Instance != null)
            provider = GameTimer.Instance;

        if (provider != null)
        {
            putSceneName = provider.GetSceneName();
            Debug.Log("Next scene: " + putSceneName);
        }
        else
        {
            Debug.LogWarning("No provider found for scene name.");
        }

        btnPlay.onClick.AddListener(OnPlayClick);
    }

    private void OnPlayClick()
    {
        UIManager.Instance.CloseAll();
        AudioManager.Instance.SFXSound(SoundID.Confirm);

        if (putSceneName != null)
        {
            SceneManager.LoadScene(putSceneName);
        }
    }
}

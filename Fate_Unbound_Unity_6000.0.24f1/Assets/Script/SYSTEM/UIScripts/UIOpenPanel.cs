using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIOpenPanel : MonoBehaviour
{
    public Button btnPlay;
    public GameUIID SelectUI;

    // Start is called before the first frame update
    void Start()
    {
        btnPlay.onClick.AddListener(OnPlayClick);
    }

    private void OnPlayClick()
    {
        // UIManager.Instance.CloseAll();
        AudioManager.Instance.SFXSound(SoundID.Confirm);
        UIManager.Instance.Open(SelectUI);
    }
}

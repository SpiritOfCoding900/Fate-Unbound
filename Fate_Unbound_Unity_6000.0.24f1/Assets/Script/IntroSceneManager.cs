using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class IntroSceneManager : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        videoPlayer.loopPointReached += VideoPlayer_loopPointReached;
    }

    // Update is called once per frame
    private void VideoPlayer_loopPointReached(VideoPlayer source)
    {
        SceneManager.LoadScene("MainScene");
    }
}

using UnityEngine;

public class GameStart : MonoBehaviour
{
    public Transform spawnPoint;
    public bool OpenPlayerSelect = true;

    private void Awake()
    {
        GameManager.Instance.spawnPlayerOnce(spawnPoint.position);

        if (OpenPlayerSelect && !UIManager.Instance.IsUIOpen(GameUIID.TutorialScreen))
            UIManager.Instance.Open(GameUIID.Settings);
    }
}

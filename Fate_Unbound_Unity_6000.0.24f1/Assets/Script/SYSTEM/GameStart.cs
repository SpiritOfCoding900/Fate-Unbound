using UnityEngine;

public class GameStart : MonoBehaviour
{
    public bool OpenPlayerSelect = true;

    private void Awake()
    {
        GameManager.Instance.spawnPlayerOnce(new Vector3(0, 0, 0));

        if (OpenPlayerSelect && !UIManager.Instance.IsUIOpen(GameUIID.TutorialScreen))
            UIManager.Instance.Open(GameUIID.PlayerSelection);
    }
}

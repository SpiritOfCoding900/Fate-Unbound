using UnityEngine;

public class PlayerRollSensor : MonoBehaviour
{
    private Player player;

    private void Awake()
    {
        player = GetComponentInParent<Player>();
        if (player == null)
            Debug.LogError("PlayerRollSensor: No Player found in parent.");
    }

    private void OnTriggerEnter2D(Collider2D other) => player?.RegisterRollable(other);
    private void OnTriggerExit2D(Collider2D other) => player?.UnregisterRollable(other);
}
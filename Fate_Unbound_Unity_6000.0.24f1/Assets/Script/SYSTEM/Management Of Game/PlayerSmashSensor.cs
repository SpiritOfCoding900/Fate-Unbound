using UnityEngine;

public class PlayerSmashSensor : MonoBehaviour
{
    private Player player;

    private void Awake()
    {
        player = GetComponentInParent<Player>();
        if (player == null)
            Debug.LogError("PlayerSmashSensor: No Player found in parent.");
    }

    private void OnTriggerEnter2D(Collider2D other) => player?.RegisterSmashable(other);
    private void OnTriggerExit2D(Collider2D other) => player?.UnregisterSmashable(other);
}
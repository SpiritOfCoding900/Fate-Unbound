using UnityEngine;

public class AttackRangeTrigger : MonoBehaviour
{
    [SerializeField] private Player player;

    private void Awake()
    {
        if (player == null)
            player = GetComponentInParent<Player>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        player.RegisterDamageable(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        player.UnregisterDamageable(other);
    }
}
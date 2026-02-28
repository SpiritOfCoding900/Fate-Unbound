using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class DoorSceneLoader : MonoBehaviour, IInteractable
{
    [Header("Scene to load (must be in Build Settings)")]
    [SerializeField] private string sceneName;

    [Header("Who can trigger this door?")]
    [SerializeField] private string playerTag = "Player";

    [Header("Optional")]
    [SerializeField] private bool preventDoubleTrigger = true;

    private bool hasTriggered;

    private void Reset()
    {
        // Ensure the door collider is set to Trigger automatically
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (preventDoubleTrigger && hasTriggered) return;
        if (!other.CompareTag(playerTag)) return;

        Interact();
    }

    public void Interact()
    {
        if (preventDoubleTrigger) hasTriggered = true;

        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogError($"{name}: No sceneName set on DoorSceneLoader.");
            hasTriggered = false;
            return;
        }

        // Safety check: is this scene actually in Build Settings?
        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError($"{name}: Scene '{sceneName}' cannot be loaded. Add it to Build Settings or check spelling.");
            hasTriggered = false;
            return;
        }

        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
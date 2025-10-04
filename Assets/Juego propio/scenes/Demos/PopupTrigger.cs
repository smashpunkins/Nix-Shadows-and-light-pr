using UnityEngine;

public class PopupTrigger : MonoBehaviour
{
    [Header("Popup Settings")]
    public GameObject popupPrefab;      // Assign your PopupText prefab
    [TextArea(2, 5)]                    // Nice multi-line text box in Inspector
    public string message = "Tutorial text here";
    public Transform spawnPoint;        // Optional (where text spawns)

    [Header("Trigger Settings")]
    public bool triggerOnce = true;     // If true, popup only shows once

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (triggerOnce && hasTriggered) return;

        hasTriggered = true;

        // Decide spawn position
        Vector3 position = spawnPoint != null ? spawnPoint.position : transform.position;

        // Spawn popup
        GameObject popup = Instantiate(popupPrefab, position, Quaternion.identity);

        // Try TextMeshPro first
        TMPro.TextMeshProUGUI tmpText = popup.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (tmpText != null)
        {
            tmpText.text = message;
            return;
        }

        // Fallback to default UI Text
        UnityEngine.UI.Text uiText = popup.GetComponentInChildren<UnityEngine.UI.Text>();
        if (uiText != null)
        {
            uiText.text = message;
        }
    }
}

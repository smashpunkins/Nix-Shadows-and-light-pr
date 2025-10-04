using UnityEngine;

public class PopupText : MonoBehaviour
{
    [Header("Popup Behavior")]
    public float lifetime = 2f; // seconds before disappearing
    public Vector3 floatSpeed = new Vector3(0, 1f, 0); // upward movement per second

    void Start()
    {
        // Automatically destroy this popup after lifetime
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Make the text float upward
        transform.position += floatSpeed * Time.deltaTime;
    }
}

using UnityEngine;

public class Quitter : MonoBehaviour
{
    public void QuitGame()
    {
        Debug.Log("Quit Game called");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // so it works in Play Mode
#endif
    }
}
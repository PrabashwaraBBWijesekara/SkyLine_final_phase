using UnityEngine;
using UnityEngine.SceneManagement; // Import this to access SceneManager

public class GotoGarden : MonoBehaviour
{
    // Name of the scene to load
    private string sceneToLoad = "Garden"; // Replace "NextScene" with your scene name

    // Alternatively, you can use OnTriggerEnter2D if you are using trigger colliders
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the trigger object has a specific tag, e.g., "Building"
        if (collision.gameObject.CompareTag("Entrance"))
        {
            // Load the specified scene
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}

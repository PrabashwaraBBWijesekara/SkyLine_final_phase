using UnityEngine;
using UnityEngine.SceneManagement; // Import this to access SceneManager

public class GotoCastle : MonoBehaviour
{
    // Name of the scene to load
    private string sceneToLoad = "Castle"; // Replace "NextScene" with your scene name

    // Alternatively, you can use OnTriggerEnter2D if you are using trigger colliders
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the trigger object has a specific tag, e.g., "Building"
        if (collision.gameObject.CompareTag("Castle"))
        {
            // Load the specified scene
            if (ItemCollector.coins>1000) {
                SceneManager.LoadScene(sceneToLoad);
            }
            //SceneManager.LoadScene(sceneToLoad);
        }
    }
}

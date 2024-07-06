
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GemGenerator : MonoBehaviour
{
    public GameObject gemPrefab;
    [SerializeField] private float generationTime = 2f; // Time in seconds to generate a gem
    [SerializeField] private int gemsPerGeneration = 1;
    [SerializeField] private float radius = 1f;
    [SerializeField] private Slider progressBar;

    private float timer = 0f;
    private List<GameObject> generatedGems = new List<GameObject>();

    void Start()
    {
        if (progressBar == null)
        {
            // If no progressBar is set, we simply don't set it up.
        }
        else
        {
            progressBar.maxValue = generationTime;
            progressBar.value = 0;
        }
    }

    void Update()
    {

        if (gemPrefab == null)
        {
            return; // Exit the update if not generating or if prefab is null or destroyed.
        }

        timer += Time.deltaTime;

        if (progressBar != null)
        {
            progressBar.value = timer;
        }

        if (timer >= generationTime && AllGemsCollected())
        {
            GenerateGems();
            timer = 0f; // Reset timer

            if (progressBar != null)
            {
                progressBar.value = 0;
            }
        }
    }
    private bool AllGemsCollected()
    {
        // Remove any null entries (coins that have been collected/destroyed)
        generatedGems.RemoveAll(gem => gem == null);
        return generatedGems.Count == 0;
    }

    private void GenerateGems()
    {

        if (gemPrefab == null)
        {
            return; // Skip gem generation if the prefab is null or destroyed.
        }

        for (int i = 0; i < gemsPerGeneration; i++)
        {
            Vector3 randomPosition = transform.position + Random.insideUnitSphere * radius;
            GameObject gem = Instantiate(gemPrefab, randomPosition, Quaternion.identity);
            generatedGems.Add(gem);

        }
    }
}

using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    public GameObject[] coinPrefabs; // Array to hold all coin prefabs
    public int numberOfCoins = 10; // Number of coins to spawn

    private void Start()
    {
        SpawnCoins();
    }

    public void SpawnCoins()
    {
        Debug.Log("code runs");
        // Loop through the number of coins to spawn
        for (int i = 0; i < numberOfCoins; i++)
        {
            // Get a random position within the specified range
            Vector3 randomPosition = new Vector3(Random.Range(-10f, 10f), 0, Random.Range(-10f, 10f));

            Vector3 offset = new Vector3(0.1f * i, 0, 0.1f * i);

            Vector3 final = randomPosition + offset;

            // Get a random coin prefab from the array
            GameObject randomCoinPrefab = coinPrefabs[Random.Range(0, coinPrefabs.Length)];

            // Instantiate the random coin prefab at the random position
            Instantiate(randomCoinPrefab, final, Quaternion.identity);
        }
    }
}

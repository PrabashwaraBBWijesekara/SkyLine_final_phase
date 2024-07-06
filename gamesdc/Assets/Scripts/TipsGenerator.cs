using UnityEngine;
using UnityEngine.UI;
using EasyUI.popupmessages;

public class TipsGenerator : MonoBehaviour
{
    public GameObject coinPrefab;
    [SerializeField] private int coinsPerGeneration = 1; // Number of gems generated each time
    [SerializeField] private float radius = 0.5f; // Radius of the circle

    // List of energy-saving tips
    private string[] tips = new string[]
    {
       "Save energy by turning off lights when you leave a room.",
        "Unplug devices when not in use to reduce phantom energy consumption.",
        "Use energy-efficient LED bulbs instead of incandescent bulbs.",
        "Use natural light during the day to reduce the need for artificial lighting.",
        "Use a smart power strip to automatically turn off devices.",
        "Turn off your computer and monitor when not in use.",
        "Use renewable energy sources like solar panels.",
        "Upgrade to energy-efficient appliances.",
        "Avoid using the washing machine during peak hours.",
        "Air-dry clothes instead of using a dryer."

    };

    [SerializeField] private Slider cooldownSlider;
    public float cooldownTime = 10f;
    private float lastTipTime;

    private void Start()
    {
        lastTipTime = 0; // Initialize to allow immediate tip display
        if (cooldownSlider != null)
        {
            cooldownSlider.maxValue = cooldownTime;
            cooldownSlider.value = cooldownTime;
            Debug.Log("Slider initialized: maxValue=" + cooldownSlider.maxValue + ", value=" + cooldownSlider.value);
        }
        else
        {
            Debug.LogError("Cooldown slider is not assigned.");
        }
    }

    private void Update()
    {
        if (cooldownSlider != null)
        {
            float timeSinceLastTip = Time.time - lastTipTime;
            float timeupdate = (timeSinceLastTip >= cooldownTime) ? cooldownTime : timeSinceLastTip;

            cooldownSlider.value = timeSinceLastTip;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            float timeSinceLastTip = Time.time - lastTipTime;
            if (timeSinceLastTip >= cooldownTime)
            {
                lastTipTime = Time.time; // Update the last tip time
                int randomIndex = Random.Range(0, tips.Length);
                string randomTip = tips[randomIndex];
                // Show the popup with the random tip
                PopupMessageUI.Instance
                    .SetTitle("Energy-Saving Tip")
                    .SetMessage(randomTip)
                    .SetReward(true)
                    .Show();

                // Reset the slider value
                if (cooldownSlider != null)
                {
                    cooldownSlider.value = 0f;
                }
                GenerateCoins();
            }
            else
            {
                Debug.Log("Cooldown active. Time remaining: " + (cooldownTime - timeSinceLastTip));
            }
        }
    }
    private void GenerateCoins()
    {
        for (int i = 0; i < coinsPerGeneration; i++)
        {
            Vector3 randomPosition = transform.position + Random.insideUnitSphere * radius;
            //randomPosition.y = transform.position.y; // Ensure the coins are generated at the same Y level

            // Instantiate the coin prefab at the calculated position
            GameObject coin = Instantiate(coinPrefab, randomPosition, Quaternion.identity);
        }


    }
}

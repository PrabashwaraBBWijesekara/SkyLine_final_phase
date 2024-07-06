/*
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class EnvironmentController : MonoBehaviour
{
    // Reference to the GameObject tree_40
    private GameObject[] trees = new GameObject[80]; // Array to hold 80 tree GameObjects
    private SpriteRenderer[] treeRenderers = new SpriteRenderer[80]; // Array to hold SpriteRenderers for efficiency

    private string apiKey = "NjVkNDIyMjNmMjc3NmU3OTI5MWJmZGIzOjY1ZDQyMjIzZjI3NzZlNzkyOTFiZmRhOQ";
    private string loginEndpoint = "http://20.15.114.131:8080/api/login";
    private string profileEndpoint = "http://20.15.114.131:8080/api/power-consumption/current/view";
    public static string password = "NjVkNDIyMjNmMjc3NmU3OTI5MWJmZGIzOjY1ZDQyMjIzZjI3NzZlNzkyOTFiZmRhOQ";
    private float timeSinceLastRequest = 0f;
    private float requestInterval = 10f; // Time in seconds between requests

    public static string jwtToken = "nulljwtToken";  // To pass jwt token to forward scene
    public string jwt = "";
    public float power_changes_value;
    public List<float> power_consumption_values_array = new List<float>(); // Initialize the list to store power consumption values


    public IEnumerator PostRequest()
    {
        string json = "{\"apiKey\":\"" + apiKey + "\"}";

        // Use Unity's built-in JSON utility to convert JSON string to bytes
        byte[] data = System.Text.Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest request = UnityWebRequest.PostWwwForm(loginEndpoint, "POST")) // request handling
        {
            request.SetRequestHeader("Content-Type", "application/json");
            request.uploadHandler = new UploadHandlerRaw(data);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string response = request.downloadHandler.text;
                jwt = response;
                Debug.Log("You are successfully authenticated. Your JWT token is: " + jwt);



                try
                {
                    // Parse the JSON string
                    LoginResponse jsonResponse = JsonUtility.FromJson<LoginResponse>(response);

                    // Extract the JWT token value
                    jwtToken = jsonResponse.token;

                    Debug.Log(jwtToken);

                }
                catch (Exception ex)
                {
                    Debug.Log("Error parsing JSON: " + ex.Message);
                }
            }
            else
            {
                Debug.LogError("Request failed: " + request.error);
                SceneManager.LoadScene("ErrorPage");
            }
        }
    }

    public IEnumerator getCurrentPowerConsumption()
    {
        using (UnityWebRequest requestnew = UnityWebRequest.Get(profileEndpoint))
        {
            // Properly formatted authorization header
            requestnew.SetRequestHeader("Authorization", "Bearer " + jwtToken);

            // Send the request
            yield return requestnew.SendWebRequest();

            // Check if the request was successful
            if (requestnew.result == UnityWebRequest.Result.Success)
            {
                // Successfully received response
                string jsonResponse = requestnew.downloadHandler.text;

                try
                {
                    // Parse the JSON using JsonUtility
                    PowerConsumptionResponse response = JsonUtility.FromJson<PowerConsumptionResponse>(jsonResponse);

                    // Log the current power consumption
                    Debug.Log("Current Power Consumption is: " + response.currentConsumption);

                    // Add the current consumption value to the array
                    if (power_consumption_values_array.Count == 2)
                    {
                        // If the list already has two elements, remove the first (oldest)
                        power_consumption_values_array.RemoveAt(0);
                    }
                    power_consumption_values_array.Add(response.currentConsumption);

                    // Calculate the change in power consumption if two values are present
                    if (power_consumption_values_array.Count == 2)
                    {
                        power_changes_value = power_consumption_values_array[1] - power_consumption_values_array[0];
                        Debug.Log("Change in Power Consumption: " + power_changes_value);
                    }
                    else
                    {
                        Debug.Log("Waiting for next power consumption value.");
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError("JSON Parse Error: " + ex.Message);
                }
            }
            else if (requestnew.result == UnityWebRequest.Result.ConnectionError || requestnew.result == UnityWebRequest.Result.ProtocolError)
            {
                // Log error
                Debug.LogError("Error: " + requestnew.error);
            }
            else
            {
                // Handle other types of errors
                Debug.LogError("Request failed: " + requestnew.error);
            }
        }
    }

    [System.Serializable]
    public class PowerConsumptionResponse
    {
        public float currentConsumption;  // Adjust the type if necessary
    }


    // Example of a class structure for your JSON response


    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < trees.Length; i++)
        {
            trees[i] = GameObject.Find("tree_" + (i + 1));
            if (trees[i] != null)
            {
                treeRenderers[i] = trees[i].GetComponent<SpriteRenderer>();
                if (treeRenderers[i] == null)
                {
                    Debug.LogError("Missing SpriteRenderer on " + trees[i].name);
                }
            }
            else
            {
                Debug.LogError("Tree GameObject not found: tree_" + (i + 1));
            }
        }
        StartCoroutine(PostRequest());
        DateTime now_time = DateTime.Now;

        // Log the current time to the console
        Debug.Log("Current Time: " + now_time.ToShortTimeString());
        DateTime now_date = DateTime.Now;

        // Log the current date to the console
        Debug.Log("Current Date: " + now_date.ToShortDateString());
    }

    void Update()
    {
        timeSinceLastRequest += Time.deltaTime;

        if (timeSinceLastRequest >= requestInterval)
        {
            timeSinceLastRequest = 0f;
            StartCoroutine(getCurrentPowerConsumption());
        }

        if (power_changes_value < 0.1)
        {
            foreach (var treeRenderer in treeRenderers)
            {
                if (treeRenderer != null)
                {
                    treeRenderer.color = Color.green;
                    treeRenderer.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
                }
            }
        }
        else if (power_changes_value < 0.5)
        {
            foreach (var treeRenderer in treeRenderers)
            {
                if (treeRenderer != null)
                {
                    treeRenderer.color = Color.red;
                    treeRenderer.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
                }
            }
        }
        else
        {
            foreach (var treeRenderer in treeRenderers)
            {
                if (treeRenderer != null)
                {
                    treeRenderer.color = Color.black;
                    treeRenderer.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                }
            }
        }


    }

    // Class to represent login response
    [Serializable]
    public class LoginResponse
    {
        public string token;
    }
    
}*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class EnvironmentController : MonoBehaviour
{
    // Reference to the GameObject tree_40
    private GameObject[] trees = new GameObject[80]; // Array to hold 80 tree GameObjects
    private SpriteRenderer[] treeRenderers = new SpriteRenderer[80]; // Array to hold SpriteRenderers for efficiency

    private string apiKey = "NjVkNDIyMjNmMjc3NmU3OTI5MWJmZGIzOjY1ZDQyMjIzZjI3NzZlNzkyOTFiZmRhOQ";
    private string loginEndpoint = "http://20.15.114.131:8080/api/login";
    private string profileEndpoint = "http://20.15.114.131:8080/api/power-consumption/current/view";
    public static string password = "NjVkNDIyMjNmMjc3NmU3OTI5MWJmZGIzOjY1ZDQyMjIzZjI3NzZlNzkyOTFiZmRhOQ";
    private float timeSinceLastRequest = 0f;
    private float requestInterval = 10f; // Time in seconds between requests

    public static string jwtToken = "nulljwtToken";  // To pass jwt token to forward scene
    public string jwt = "";
    public float power_changes_value;
    public List<float> power_consumption_values_array = new List<float>(); // Initialize the list to store power consumption values

    private HeartBar heart;

    public IEnumerator PostRequest()
    {
        string json = "{\"apiKey\":\"" + apiKey + "\"}";

        // Use Unity's built-in JSON utility to convert JSON string to bytes
        byte[] data = System.Text.Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest request = UnityWebRequest.PostWwwForm(loginEndpoint, "POST")) // request handling
        {
            request.SetRequestHeader("Content-Type", "application/json");
            request.uploadHandler = new UploadHandlerRaw(data);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string response = request.downloadHandler.text;
                jwt = response;
                Debug.Log("You are successfully authenticated. Your JWT token is: " + jwt);

                try
                {
                    // Parse the JSON string
                    LoginResponse jsonResponse = JsonUtility.FromJson<LoginResponse>(response);

                    // Extract the JWT token value
                    jwtToken = jsonResponse.token;

                    Debug.Log(jwtToken);

                }
                catch (Exception ex)
                {
                    Debug.Log("Error parsing JSON: " + ex.Message);
                }
            }
            else
            {
                Debug.LogError("Request failed: " + request.error);
                SceneManager.LoadScene("ErrorPage");
            }
        }
    }

    public IEnumerator getCurrentPowerConsumption()
    {
        using (UnityWebRequest requestnew = UnityWebRequest.Get(profileEndpoint))
        {
            // Properly formatted authorization header
            requestnew.SetRequestHeader("Authorization", "Bearer " + jwtToken);

            // Send the request
            yield return requestnew.SendWebRequest();

            // Check if the request was successful
            if (requestnew.result == UnityWebRequest.Result.Success)
            {
                // Successfully received response
                string jsonResponse = requestnew.downloadHandler.text;

                try
                {
                    // Parse the JSON using JsonUtility
                    PowerConsumptionResponse response = JsonUtility.FromJson<PowerConsumptionResponse>(jsonResponse);

                    // Log the current power consumption
                    Debug.Log("Current Power Consumption is: " + response.currentConsumption);

                    // Add the current consumption value to the array
                    if (power_consumption_values_array.Count == 2)
                    {
                        // If the list already has two elements, remove the first (oldest)
                        power_consumption_values_array.RemoveAt(0);
                    }
                    power_consumption_values_array.Add(response.currentConsumption);

                    // Calculate the change in power consumption if two values are present
                    if (power_consumption_values_array.Count == 2)
                    {
                        power_changes_value = power_consumption_values_array[1] - power_consumption_values_array[0];
                        Debug.Log("Change in Power Consumption: " + power_changes_value);

                        // Call the ApplyDamage method in Player
                        if (heart != null)
                        {
                            heart.ApplyDamage(power_changes_value);
                        }
                    }
                    else
                    {
                        Debug.Log("Waiting for next power consumption value.");
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError("JSON Parse Error: " + ex.Message);
                }
            }
            else if (requestnew.result == UnityWebRequest.Result.ConnectionError || requestnew.result == UnityWebRequest.Result.ProtocolError)
            {
                // Log error
                Debug.LogError("Error: " + requestnew.error);
            }
            else
            {
                // Handle other types of errors
                Debug.LogError("Request failed: " + requestnew.error);
            }
        }
    }

    [System.Serializable]
    public class PowerConsumptionResponse
    {
        public float currentConsumption;  // Adjust the type if necessary
    }

    // Example of a class structure for your JSON response

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < trees.Length; i++)
        {
            trees[i] = GameObject.Find("tree_" + (i + 1));
            if (trees[i] != null)
            {
                treeRenderers[i] = trees[i].GetComponent<SpriteRenderer>();
                if (treeRenderers[i] == null)
                {
                    Debug.LogError("Missing SpriteRenderer on " + trees[i].name);
                }
            }
            else
            {
                Debug.LogError("Tree GameObject not found: tree_" + (i + 1));
            }
        }
        StartCoroutine(PostRequest());
        DateTime now_time = DateTime.Now;

        // Log the current time to the console
        Debug.Log("Current Time: " + now_time.ToShortTimeString());
        DateTime now_date = DateTime.Now;

        // Log the current date to the console
        Debug.Log("Current Date: " + now_date.ToShortDateString());

        // Find the Player instance
        heart = FindObjectOfType<HeartBar>();
        if (heart == null)
        {
            Debug.LogError("Player not found!");
        }
    }

    void Update()
    {
        timeSinceLastRequest += Time.deltaTime;

        if (timeSinceLastRequest >= requestInterval)
        {
            timeSinceLastRequest = 0f;
            StartCoroutine(getCurrentPowerConsumption());
        }

        if (power_changes_value < 0.1)
        {
            foreach (var treeRenderer in treeRenderers)
            {
                if (treeRenderer != null)
                {
                    treeRenderer.color = Color.green;
                    treeRenderer.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
                }
            }
        }
        else if (power_changes_value < 0.5)
        {
            foreach (var treeRenderer in treeRenderers)
            {
                if (treeRenderer != null)
                {
                    treeRenderer.color = Color.red;
                    treeRenderer.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
                }
            }
        }
        else
        {
            foreach (var treeRenderer in treeRenderers)
            {
                if (treeRenderer != null)
                {
                    treeRenderer.color = Color.black;
                    treeRenderer.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                }
            }
        }
    }

    // Class to represent login response
    [Serializable]
    public class LoginResponse
    {
        public string token;
    }
}

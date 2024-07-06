using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Linq;
using EasyUI.popupmessages;
using System.Diagnostics;
using Debug = UnityEngine.Debug;


public class EnergySavingsAlgorithm : MonoBehaviour
{
    // API Key should be stored securely and not hard-coded
    private string apiKey = "NjVkNDIyMjNmMjc3NmU3OTI5MWJmZGIzOjY1ZDQyMjIzZjI3NzZlNzkyOTFiZmRhOQ";
    private readonly string loginEndpoint = "http://20.15.114.131:8080/api/login";
    private readonly string profileEndpoint = "http://20.15.114.131:8080/api/power-consumption/current/view";
    private readonly string dailyConsumptionEndpoint = "http://20.15.114.131:8080/api/power-consumption/month/daily/view";
    private readonly string dailyConsumptionCurrentMonthEndpoint = "http://20.15.114.131:8080/api/power-consumption/current-month/daily/view";
    public Image blackImage;
    private float alpha;

    private float timeSinceLastRequest = 0f;
    private float requestInterval = 10f;
    public static string jwtToken;  // JWT Token to be used for API requests
    public List<float> powerConsumptionValuesArray = new List<float>(); // List to store power consumption values

    public int year1 = 2024;   // Example year for API requests
    public string month1 = "MAY"; // Example month for API requests
    public int day1 = 2;

    public string date1 = "12/4/2024";

    public Dictionary<string, float> dailyConsumptionMonthDict = new Dictionary<string, float>();
    public Dictionary<string, float> dailyConsumptionCurrentMonthDict = new Dictionary<string, float>();

    public delegate void OnDailyConsumptionReady();
    public static event OnDailyConsumptionReady onDailyConsumptionReady;

    public delegate void OnCurrentMonthConsumptionReady();
    public static event OnCurrentMonthConsumptionReady onCurrentMonthConsumptionReady;

    private bool isDailyConsumptionReady = false;
    private bool isCurrentMonthConsumptionReady = false;

    public static string key_value = "0";

    public float finalScore = 0;
    public static string condition;

    public GameObject coinprefab;
    public GameObject coinspawner;
    [SerializeField] private float radius = 20f;
    

    //public string Month = null;
    //public int Year = 0;
    //public int Day = 0;

    void Start()
    {

        onDailyConsumptionReady += HandleDailyConsumptionReady;
        onCurrentMonthConsumptionReady += HandleCurrentMonthConsumptionReady;

        InitializeTrees();
        StartCoroutine(PostRequest());  // Start the login process to retrieve JWT
        Debug.Log("Current Time: " + DateTime.Now.ToShortTimeString());
        Debug.Log("Current Date: " + DateTime.Now.ToShortDateString());

        //day1 = 1;

        date1 = DateTime.Now.ToShortDateString();
        //date1 = "4/30/2024";

        string[] dateparts = date1.Split("/");
        day1 = int.Parse(dateparts[1]);


        Debug.Log("day: " + day1);

        // Fetch daily power consumption
        //GenerateCoins();

    }


    void HandleDailyConsumptionReady()
    {
        isDailyConsumptionReady = true;
        CheckDataReadyAndProcess();
    }

    void HandleCurrentMonthConsumptionReady()
    {
        isCurrentMonthConsumptionReady = true;
        CheckDataReadyAndProcess();
    }

    // check the availability of both data set once after each data come
    void CheckDataReadyAndProcess()
    {
        if (isDailyConsumptionReady && isCurrentMonthConsumptionReady)
        {
            // Both sets of data are ready
            Debug.Log("Both daily and current month consumption data are ready.");

            // Now you can safely access and process both dictionaries:
            // dailyConsumptionMonthDict and dailyConsumptionCurrentMonthDict

            Algorithm();
            

        }
    }
    IEnumerator ChangeFade()
    {
        while (true)
        {
            alpha = UnityEngine.Random.Range(0f, 1f); // Generate a random alpha value between 0 and 1
            blackImage.color = new Color(0, 0, 0, alpha); // Set alpha for the color
            Debug.Log("Hello World");
            yield return new WaitForSeconds(1f); // Wait for 1 second before changing alpha again
        }
    }
    // energy saving algorithm is here
    void Algorithm()
    {
        // Implement your logic to process both sets of data here
        Debug.Log("Processing combined data...");

        float parameter1 = 0;
        float parameter2 = 0;
        float weekSumCurrentYear = 0;
        float previousWeekSum = 0;
        float yesterdayValue = 0;

        if (day1 == 1)
        {
            List<float> PreviousMonthLastTwoDays = new List<float>();
            var keys = new List<string>(dailyConsumptionMonthDict.Keys);
            int count = keys.Count;

            Debug.Log("condition 1 identified");

            PreviousMonthLastTwoDays.Add(dailyConsumptionMonthDict[keys[count - 2]]);
            PreviousMonthLastTwoDays.Add(dailyConsumptionMonthDict[keys[count - 1]]);

            yesterdayValue = PreviousMonthLastTwoDays[1];

            //param1
            parameter1 = PreviousMonthLastTwoDays[1] - PreviousMonthLastTwoDays[0];

            Debug.Log("yesterdayValue: " + yesterdayValue);
            Debug.Log("dayBeforeYesterdayValue: " + PreviousMonthLastTwoDays[1]);
        }

        else if (day1 == 2)
        {
            float previousMonthDay = 0;
            float currentMonthDay = 0;

            var keys = new List<string>(dailyConsumptionMonthDict.Keys);
            int count = keys.Count;

            Debug.Log("condition 2 identified");


            previousMonthDay = dailyConsumptionMonthDict[keys[count - 1]];
            currentMonthDay = dailyConsumptionCurrentMonthDict["1"];

            yesterdayValue = currentMonthDay;

            //param1
            parameter1 = currentMonthDay - previousMonthDay;

            Debug.Log("yesterdayValue: " + yesterdayValue);
            Debug.Log("dayBeforeYesterdayValue: " + previousMonthDay);

        }

        else
        {
            // yesterdayValue is defined outside.
            float dayBeforeYesterdayValue = 0;

            int yesterday = day1 - 1;
            int dayBeforeYesterday = day1 - 2;

            Debug.Log("condition 3 identified");

            yesterdayValue = dailyConsumptionCurrentMonthDict[yesterday.ToString()];
            dayBeforeYesterdayValue = dailyConsumptionCurrentMonthDict[dayBeforeYesterday.ToString()];

            Debug.Log("yesterdayValue: " + yesterdayValue);
            Debug.Log("dayBeforeYesterdayValue: " + dayBeforeYesterdayValue);

            //param1
            parameter1 = dayBeforeYesterdayValue - yesterdayValue;

        };

        if (day1 < 8)
        {
            List<float> PreviousMonthLastTwoDays = new List<float>();
            var keys = new List<string>(dailyConsumptionMonthDict.Keys);
            int count = keys.Count;

            for (int i = 0; i < day1; i++)
            {
                int day = day1 - i;
                weekSumCurrentYear += dailyConsumptionCurrentMonthDict[day.ToString()];
            };

            for (int j = 0; j < (7 - day1); j++)
            {
                weekSumCurrentYear += dailyConsumptionMonthDict[keys[count - j - 1]];
            };

            //Debug.Log("weekSumCurrentYear: " + weekSumCurrentYear);

        }

        else
        {
            for (int i = 0; i < 7; i++)
            {
                int day = day1 - i - 1;
                weekSumCurrentYear += dailyConsumptionCurrentMonthDict[day.ToString()];
            };

        };

        if (day1 < 14)
        {

            var keys = new List<string>(dailyConsumptionMonthDict.Keys);
            int count = keys.Count;

            for (int i = 0; i < (day1 - 7); i++)
            {
                int day = day1 - 7 - i;
                previousWeekSum += dailyConsumptionCurrentMonthDict[day.ToString()];
            };

            for (int j = 0; j < (14 - day1); j++)
            {
                previousWeekSum += dailyConsumptionMonthDict[keys[count - j - 8]];
            };


        }

        else
        {
            for (int i = 0; i < 7; i++)
            {
                int day = day1 - i - 8;
                previousWeekSum += dailyConsumptionCurrentMonthDict[day.ToString()];
            };

        };
        float weekAverage = (weekSumCurrentYear / 7);
        float previousWeekAverage = (previousWeekSum / 7);

        Debug.Log("weekSumCurrentYear: " + weekSumCurrentYear);
        Debug.Log("previousWeekSum: " + previousWeekSum);

        parameter2 = previousWeekAverage - weekAverage;
        Debug.Log("parameter1: " + parameter1);
        Debug.Log("parameter2: " + parameter2);

        finalScore = parameter1;


        if (parameter1 >= 0 && parameter2 >= 0)
        {
            
            Debug.Log("Very good");
            condition = "Excellent";
            finalScore = (parameter1 / 10) * 50 + parameter2 * 50;

        }
        else if (parameter1 >= 0 && parameter2 < 0)
        {
            Debug.Log("good");
            condition = "Good";
            finalScore = (parameter1 / 10) * 50;
            //StartCoroutine(ChangeFade());
        }
        else if (parameter1 < 0 && parameter2 >= 0)
        {
            Debug.Log("good");
            condition = "Good";
            finalScore = (parameter2 / 5) * 50;
            //StartCoroutine(ChangeFade());
        }
        else
        {
            Debug.Log("Energy Waste");
            condition = "Worse";
            finalScore = 0;
            StartCoroutine(ChangeFade());
            Debug.Log("Condition string is checkein in darken script");
        };
        Debug.Log("finalScore: " + finalScore);

        GenerateCoins();

    }
    // Placeholder for future tree-related initialization
    private void InitializeTrees()
    {
        // TODO: Implement any necessary initial setup for trees here
    }
    [Serializable]
    public class LoginResponse
    {
        public string token;
    }

    void Update()
    {
        timeSinceLastRequest += Time.deltaTime;

        if (timeSinceLastRequest >= requestInterval)
        {
            timeSinceLastRequest = 0f;
            StartCoroutine(GetCurrentPowerConsumption()); // Periodically fetch current power consumption
        }

    }

    // Handle Daily power consumption data 
    void HandleDataReady()
    {
        // Now handle or log the data
        string dictionaryString = string.Join(", ", dailyConsumptionMonthDict.Select(kv => $"Key: {kv.Key}, Value: {kv.Value}"));
        Debug.Log(dictionaryString);
    }

    // Method to handle user login and JWT retrieval
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
                string jwt = response;
                //jwtToken = jwt.Trim();
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
        StartCoroutine(GetDailyPowerConsumption(date1));
        StartCoroutine(getdailyConsumptionCurrentMonthEndpoint());
    }


    // Method to fetch daily power consumption data in a previous month
    public IEnumerator GetDailyPowerConsumption(string Date) // INPUT Date DD/MM/YYYY
    {

        string[] dateparts = Date.Split("/");

        int month = int.Parse(dateparts[0]);
        List<string> months = new List<string>
        {
            "january","february","march","april","may","june","july","august","september","october","november","december"
        };
        day1 = int.Parse(dateparts[1]);
        int Year = 0;
        string Month = null;
        if (month != 1)
        {
            Month = months[month - 2].ToUpper();
            Year = int.Parse(dateparts[2]);
        }
        else
        {
            Month = months[1].ToUpper();
            Year = int.Parse(dateparts[2]) - 1;

        };

        Year = int.Parse(dateparts[2]);


        string url = $"{dailyConsumptionEndpoint}?year={Year}&month={Month}";
        Debug.Log("Complete URL for request: " + url);
        Debug.Log("JWT Token at the time of request: " + jwtToken);

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("Authorization", "Bearer " + jwtToken);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                Debug.Log("Daily power consumption data: " + jsonResponse);
                dailyConsumptionMonthDict = ExtractDailyConsumption(jsonResponse);
                bool i = dailyConsumptionMonthDict.Count == 0;
                Debug.Log("i: " + i);
                onDailyConsumptionReady?.Invoke();
            }
            else
            {
                Debug.LogError("Daily consumption request failed: " + request.error);
                Debug.LogError("Response code: " + request.responseCode);
            }
        }
    }


    // Method to fetch current power consumption data (needs implementation)
    private IEnumerator GetCurrentPowerConsumption()
    {
        string url = profileEndpoint;
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("Authorization", "Bearer " + jwtToken);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                Debug.Log("Current power consumption data: " + jsonResponse);
            }
            else
            {
                Debug.LogError("Current consumption request failed: " + request.error);
            }
        }
    }

    // Method to fetch daily power consumption data in a current month
    public IEnumerator getdailyConsumptionCurrentMonthEndpoint()
    {
        using (UnityWebRequest requestnew2 = UnityWebRequest.Get(dailyConsumptionCurrentMonthEndpoint))
        {
            // Properly formatted authorization header
            requestnew2.SetRequestHeader("Authorization", "Bearer " + jwtToken);

            // Send the request
            yield return requestnew2.SendWebRequest();

            // Check if the request was successful
            if (requestnew2.result == UnityWebRequest.Result.Success)
            {
                string jsonResponseNew = requestnew2.downloadHandler.text;
                Debug.Log("Current daily power consumption data: " + jsonResponseNew);
                dailyConsumptionCurrentMonthDict = ExtractDailyConsumption(jsonResponseNew);
                onCurrentMonthConsumptionReady?.Invoke();
            }
            else
            {
                UnityEngine.Debug.LogError("Current daily consumption request failed: " + requestnew2.error);
            }

        }
    }
    [Serializable]
    public class DailyConsumptionResponse
    {
        public DailyPowerConsumptionView dailyPowerConsumptionView;
    }

    [Serializable]
    public class DailyPowerConsumptionView
    {
        public int year;
        public int month;
        public Dictionary<string, float> dailyUnits;
    }
    private Dictionary<string, float> ExtractDailyConsumption(string jsonResponse)
    {
        JObject parsedJson = JObject.Parse(jsonResponse);

        // Attempt to access the nested dictionary safely
        var dailyUnits = parsedJson["dailyPowerConsumptionView"]?["dailyUnits"]?.ToObject<Dictionary<string, float>>();
        Debug.Log(dailyUnits);
        if (dailyUnits == null)
        {
            Debug.LogError("Failed to extract daily consumption data: One or more properties are null.");
            return new Dictionary<string, float>(); // Return an empty dictionary to avoid further errors
        }

        // Log the extracted data
        //foreach (var day in dailyUnits)
        //{
        //    Debug.Log("day " + day.key + ": " + day.value + " units");
        //}

        return dailyUnits;
    }
    private void GenerateCoins()
    {
        finalScore = (int)finalScore;

        if (condition == "Excellent")
        {
            PopupMessageUI.Instance
                    .SetTitle("Excellent")
                    .SetMessage("You are gifted " + finalScore + " coins")
                    .SetReward(true)
                    .Show();
        }
        else if (condition == "Good")
        {
            PopupMessageUI.Instance
                    .SetTitle("Good")
                    .SetMessage("Acceptable consumption!!\n "+ "You are gifted " + finalScore + " coins")
                    .SetReward(true)
                    .Show();
        }
        else if (condition == "Worse")
        {
            PopupMessageUI.Instance
                    .SetTitle("Bad")
                    .SetMessage("You have wasted energy!!\n "+"You are gifted " + finalScore + " coins")
                    .SetReward(false)
                    .Show();
        }


        if (coinprefab == null)
        {
            return; // Skip gem generation if the prefab is null or destroyed.
        }

        for (int i = 0; i < finalScore; i++)
        {
            Vector3 randomPosition = coinspawner.transform.position + UnityEngine.Random.insideUnitSphere * radius;
            Instantiate(coinprefab, randomPosition, Quaternion.identity);

        }
    }


}


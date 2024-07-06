using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System;
using Random = UnityEngine.Random;

public class LeaderBoard : MonoBehaviour
{
    private Transform entryBody;
    private Transform entryContainer;
    private Transform entryTemplate;
    private Transform playerRank;
    private List<Player> playerList;
    private readonly string profileEndpoint = "http://20.15.114.131:8080/api/user/profile/list";
    private readonly string profileViewEndpoint = "http://20.15.114.131:8080/api/user/profile/view";

    private string authenticatedUsername;
    private string jwt_newone;
    private string id_value_string;
    public static string playerLoginJWTToken;
    public static string playerIDvalue;
    public static int initial_gems_value;
    public static int initial_coins_value;
    public static int condition_check_value;

    public static int coins_count = 50;
    public static int gems_count = 9;
    public GameObject loadingScreen;

    private async void Start()
    {
        
        string jwtToken = postmethod.jwtToken;  // Get the JWT token from postmethod
        StartCoroutine(SendLoginRequest());
       
        if (!string.IsNullOrEmpty(jwtToken) && jwtToken != "nulljwtToken")
        {
            ShowLoadingScreen(true);
            await FetchPlayerProfile(jwtToken);
            await FetchPlayerNamesFromAPI(jwtToken);
            ShowLoadingScreen(false);
        }
        else
        {
            Debug.LogError("JWT token is invalid. Authentication failed.");
        }
    }
    IEnumerator SendLoginRequest()
    {
        string username = postmethod.username_var;
        string password = postmethod.password;
        Debug.Log("The username is "+ "and password is"+password);

        var data = new
        {
            username = username,
            password = password
        };

        string jsonData = JsonConvert.SerializeObject(data);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest request = UnityWebRequest.PostWwwForm("http://localhost:8081/api/v1/player/log", jsonData))
        {
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
            }
            else
            {
                string response = request.downloadHandler.text;
                JObject jsonResponse = JObject.Parse(response);
                jwt_newone = (string)jsonResponse["token"];
                playerLoginJWTToken = jwt_newone;
                int id_value = (int)jsonResponse["id"];
                id_value_string = id_value.ToString();
                playerIDvalue = id_value_string;
                Debug.Log("The Spring Backend token is" + jwt_newone + "and Player ID is " + id_value);
                //updatePlayerStatus();
                string url = "http://localhost:8081/api/v1/player/answer/" + id_value_string;
                StartCoroutine(SendPlayerStatusGettingRequest(url, jwt_newone));
            }
        }
    }
    public IEnumerator SendPlayerStatusGettingRequest(string url, string jwt_newone)
    {
        using (UnityWebRequest requestForCheckingQADone = UnityWebRequest.Get(url))
        {
            requestForCheckingQADone.SetRequestHeader("Authorization", "Bearer " + jwt_newone);

            yield return requestForCheckingQADone.SendWebRequest();

            if (requestForCheckingQADone.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(requestForCheckingQADone.error);
                yield break;
            }

            string jsonResponse = requestForCheckingQADone.downloadHandler.text;
            Debug.Log("Response: " + jsonResponse);

            try
            {
                JObject responseJson = JObject.Parse(jsonResponse);
                Debug.Log("This is all json object parse" + responseJson);

                // Get the "content" object
                JObject content = (JObject)responseJson["content"];

                // Access "totalCoins" from "content"
                int totalCoins = (int)content["totalCoins"];

                // Access "playerStatus" from "content"
                JObject playerStatus = (JObject)content["playerStatus"];

                // Access specific properties within "playerStatus" (if needed)
                int playerStatusCoins = (int)playerStatus["coins"];
                int playerStatusGems = (int)playerStatus["gems"];

                // Log the playerStatus object (optional)
                Debug.Log("Response: player status" + playerStatus);
                // Correcting the path to access resources within playerStatus
                if (playerStatus == null)
                {// I want to player Status was null check valued assigned as 1

                    condition_check_value = 1;
                   
                    //gemsCount.text = totalCoins.ToString();
                    initial_coins_value = 100;
                    //coinsCount.text = "100";
                   
                    JObject playerData = new JObject(
                        new JProperty("coins", 0),
                        new JProperty("gems", 0),
                        new JProperty("resources", new JArray())  // Ensure the array is initialized correctly.
                    );

                    Debug.Log("Test null or not player ");


                    int playerId;
                    if (!int.TryParse(CheckinPlayDirectly.playerIDvalue, out playerId))
                    {
                        Debug.LogError("Invalid player ID");
                    }
                    else
                    {
                        // StartCoroutine(PlayerStatusSave("http://localhost:8081/api/playerstatus/savestatus", CheckinPlayDirectly.playerLoginJWTToken, playerId, playerData));
                    }

                }
                else
                {
                    condition_check_value = 0;
                }
                if (responseJson["content"]?["playerStatus"]?["resources"] != null)
                {
                    JArray resourcesArray = (JArray)responseJson["content"]["playerStatus"]["resources"];
                    if (resourcesArray.Count > 0)
                    {
                        string resourcesString = resourcesArray[0].ToString();  // Assuming there's at least one item in the array
                        JObject resourceObject = JObject.Parse(resourcesString);


                        // Extracting specific values from the 'resourceObject'
                        double playerPositionX = (double)resourceObject["playerPositionX"];
                        double playerPositionY = (double)resourceObject["playerPositionY"];
                        float playerPositionX_floatvalue = Convert.ToSingle(playerPositionX);
                        float playerPositionY_floatvalue = Convert.ToSingle(playerPositionY);
                        int coinscount = (int)resourceObject["coinscount"];
                        //initialCoinsValue = coinscount;
                        int gemscount = (int)resourceObject["gemscount"];
                        initial_gems_value = gemscount;
                        string time = (string)resourceObject["time"];
                        string date = (string)resourceObject["date"];
                        coins_count = coinscount;
                        gems_count = gemscount;


    // Debugging the values
    Debug.Log($"Extracted Values:\nPlayer Position X: {playerPositionX}\nPlayer Position Y: {playerPositionY}\nCoins Count: {coinscount}\nTime: {time}\nDate: {date}");
                        string json = PlayerPrefs.GetString("PlayerPosition");
                        GameData data = JsonUtility.FromJson<GameData>(json);
                        Debug.Log("Loaded Data: " + json); // Check what is being loaded
                        

                    

                    }
                    else
                    {
                        Debug.Log("No resources found in the response.");
                    }
                }
                else
                {
                    Debug.Log("No 'resources' field found in the response or 'playerStatus' is null.");
                }
            }
            catch (Exception e)
            {
               
            }
        }
        

    }


    private void Awake()
    {
        entryBody = transform.Find("scroll rect");
        if (entryBody == null)
        {
            Debug.LogError("entryBody not found in hierarchy. Check GameObject hierarchy.");
        }
        else
        {
            entryContainer = entryBody.Find("content");
            if (entryContainer == null)
            {
                Debug.LogError("entryContainer not found in hierarchy. Check GameObject hierarchy.");
            }
            else
            {
                entryTemplate = entryContainer.Find("player input");
                if (entryTemplate == null)
                {
                    Debug.LogError("entryTemplate not found in hierarchy. Check GameObject hierarchy.");
                }
                else
                {
                    entryTemplate.gameObject.SetActive(false);
                }
            }
        }

        playerRank = transform.Find("Player rank");
        if (playerRank == null)
        {
            Debug.LogError("playerRank not found in hierarchy. Check GameObject hierarchy.");
        }
        else
        {
            if (playerRank.Find("Score") == null || playerRank.Find("Rank Image/Rank") == null)
            {
                Debug.LogError("Score or Rank component not found under playerRank. Check GameObject hierarchy.");
            }
        }
    }

    private async Task FetchPlayerNamesFromAPI(string jwtToken)
    {
        UnityWebRequest request = UnityWebRequest.Get(profileEndpoint);
        request.SetRequestHeader("Authorization", $"Bearer {jwtToken}");

        var operation = request.SendWebRequest();

        while (!operation.isDone)
        {
            await Task.Yield();
        }

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = request.downloadHandler.text;
            Debug.Log("API Response: " + jsonResponse);

            PlayerListResponse response = JsonUtility.FromJson<PlayerListResponse>(jsonResponse);

            if (response != null && response.userViews != null)
            {
                playerList = new List<Player>();
                foreach (var user in response.userViews)
                {
                    int coins;
                    int gems;

                    if (user.username == authenticatedUsername)
                    {
                        coins = coins_count;
                        gems = gems_count; // Updated to use gems_count
                    }
                    else
                    {
                        coins = Random.Range(0, 100);
                        gems = Random.Range(0, 10);
                    }

                    int score = 3 * coins + 4 * gems;
                    playerList.Add(new Player { name = user.username, score = score });
                }

                playerList.Sort((x, y) => y.score.CompareTo(x.score));

                // Update the leaderboard UI with the fetched player data
                UpdateLeaderboardUI();
            }
            else
            {
                Debug.LogError("Failed to parse API response.");
            }
        }
        else
        {
            Debug.LogError("Request failed: " + request.error);
        }
    }

    private async Task FetchPlayerProfile(string jwtToken)
    {
        UnityWebRequest profileRequest = UnityWebRequest.Get(profileViewEndpoint);
        profileRequest.SetRequestHeader("Authorization", $"Bearer {jwtToken}");

        var operation = profileRequest.SendWebRequest();

        while (!operation.isDone)
        {
            await Task.Yield();
        }

        if (profileRequest.result == UnityWebRequest.Result.Success)
        {
            string profileResponse = profileRequest.downloadHandler.text;
            Debug.Log("Profile API Response: " + profileResponse);

            // Parse profile response
            JObject profileJson = JObject.Parse(profileResponse);
            authenticatedUsername = profileJson["user"]["username"].ToString();

            Debug.Log("Authenticated Username: " + authenticatedUsername);

            // Update UI or perform actions based on the authenticated username
            // Here you could update parts of the UI that depend on the authenticated user.
        }
        else
        {
            Debug.LogError("Failed to fetch player profile: " + profileRequest.error);
        }
    }

    private void UpdateLeaderboardUI()
    {
        if (playerList == null || playerList.Count == 0)
        {
            Debug.LogWarning("Player list is empty or null.");
            return;
        }

        int noOfPlayers = playerList.Count;
        float templateHeight = 70f;

        RectTransform containerRectTransform = entryContainer.GetComponent<RectTransform>();
        containerRectTransform.sizeDelta = new Vector2(containerRectTransform.sizeDelta.x, noOfPlayers * templateHeight + 10f);

        for (int i = 0; i < noOfPlayers; i++)
        {
            Transform entryTransform = Instantiate(entryTemplate, entryContainer);
            RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
            entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * i);
            entryRectTransform.gameObject.SetActive(true);

            int rank = i + 1;
            entryTransform.Find("Rank").GetComponent<TMP_Text>().text = rank.ToString();
            entryTransform.Find("Player").GetComponent<TMP_Text>().text = playerList[i].name;
            entryTransform.Find("Score").GetComponent<TMP_Text>().text = playerList[i].score.ToString();

            Color gold = new Color(1f, 0.843f, 0f, 1f);
            Color silver = new Color(0.75f, 0.75f, 0.75f, 1f);
            Color bronze = new Color(0.803f, 0.498f, 0.196f, 1f);

            Image trophyImage = entryTransform.Find("Trophy").GetComponent<Image>();
            switch (rank)
            {
                default:
                    trophyImage.gameObject.SetActive(false);
                    break;
                case 1:
                    trophyImage.color = gold;
                    trophyImage.gameObject.SetActive(true);
                    break;
                case 2:
                    trophyImage.color = silver;
                    trophyImage.gameObject.SetActive(true);
                    break;
                case 3:
                    trophyImage.color = bronze;
                    trophyImage.gameObject.SetActive(true);
                    break;
            }

            entryTransform.Find("Panel").gameObject.SetActive(false);

            if (playerList[i].name == authenticatedUsername)
            {
                playerRank.Find("Score").GetComponent<TMP_Text>().text = playerList[i].score.ToString();
                playerRank.Find("Rank Image").Find("Rank").GetComponent<TMP_Text>().text = rank.ToString();
                entryTransform.Find("Panel").gameObject.SetActive(true);
            }
        }
    }

    private void ShowLoadingScreen(bool show)
    {
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(show);

        }
    }

    [System.Serializable]
    public class PlayerListResponse
    {
        public List<UserView> userViews;
    }

    [System.Serializable]
    public class UserView
    {
        public string username;
    }

    [System.Serializable]
    public class Player
    {
        public string name;
        public int score;
    }
}


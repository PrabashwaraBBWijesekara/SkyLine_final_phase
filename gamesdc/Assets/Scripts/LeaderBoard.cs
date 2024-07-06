using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using System.Threading.Tasks;

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

    public static int coins_count = 50;
    public static int gems_count = 9;
    public GameObject loadingScreen;

    private async void Start()
    {
        string jwtToken = postmethod.jwtToken;  // Get the JWT token from postmethod

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

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Newtonsoft.Json.Linq;
using System.Collections;
using System;

public class postmethod : MonoBehaviour
{
    private string apiKey = "NjVkNDIyMjNmMjc3NmU3OTI5MWJmZGIzOjY1ZDQyMjIzZjI3NzZlNzkyOTFiZmRhOQ";
    private string loginEndpoint = "http://20.15.114.131:8080/api/login";
    private string profileEndpoint = "http://20.15.114.131:8080/api/user/profile/view";
    public static string jwtToken = "nulljwtToken";  // To pass jwt token to forward scene
    public static string password= "NjVkNDIyMjNmMjc3NmU3OTI5MWJmZGIzOjY1ZDQyMjIzZjI3NzZlNzkyOTFiZmRhOQ";
    public static string username_var = "null";
    public string jwt = "nulljwt";

    public void retryScene()
    {
        SceneManager.LoadScene("StartScene");
    }

    public void quitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public IEnumerator PostRequest()
    {
        string json = "{\"apiKey\":\"" + apiKey + "\"}";
        byte[] data = System.Text.Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest request = UnityWebRequest.PostWwwForm(loginEndpoint, ""))
        {
            request.uploadHandler = new UploadHandlerRaw(data);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string response = request.downloadHandler.text;
                jwt = response;
                Debug.Log("You are successfully authenticated. Your JWT token is: " + jwt);

                try
                {
                    JObject jsonResponse = JObject.Parse(response);
                    jwtToken = (string)jsonResponse["token"];
                    Debug.Log("JWT Token: " + jwtToken);
                    //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                }
                catch (Exception ex)
                {
                    Debug.Log("Error parsing JSON: " + ex.Message);
                }
            }
            else
            {
                Debug.LogError("Authentication failed with error: " + request.error);
                SceneManager.LoadScene("ErrorPage");
            }
        }

        if (!string.IsNullOrEmpty(jwtToken) && jwtToken != "nulljwtToken")
        {
            //Debug.Log("Test Profile Details");
            using (UnityWebRequest requestnew = UnityWebRequest.Get(profileEndpoint))
            {
                requestnew.SetRequestHeader("Authorization", "Bearer " + jwtToken);

                yield return requestnew.SendWebRequest();

                if (requestnew.result == UnityWebRequest.Result.Success)
                {
                    string responsenew = requestnew.downloadHandler.text;
                    JObject jsonResponse = JObject.Parse(responsenew);

                    string firstname = (string)jsonResponse["user"]["firstname"];
                    string lastname = (string)jsonResponse["user"]["lastname"];
                    string username = (string)jsonResponse["user"]["username"];
                    username_var = username;
                    string nic = (string)jsonResponse["user"]["nic"];
                    string phoneNumber = (string)jsonResponse["user"]["phoneNumber"];
                    string email = (string)jsonResponse["user"]["email"];
                    string profilePictureUrl = (string)jsonResponse["user"]["profilePictureUrl"];

                    Debug.Log("Firstname: " + firstname);
                    Debug.Log("Lastname: " + lastname);
                    Debug.Log("Username: " + username);
                    Debug.Log("NIC: " + nic);
                    Debug.Log("Phone Number: " + phoneNumber);
                    Debug.Log("Email: " + email);
                    Debug.Log("Profile Picture URL: " + profilePictureUrl);

                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                }
                else
                {
                    Debug.LogError("Failed to get profile data: " + requestnew.error);
                }
            }
        }
    }

    public void OnButtonClick()
    {
        StartCoroutine(PostRequest());
    }
}



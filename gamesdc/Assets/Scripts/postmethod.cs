using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine.Events;
using System.Net;

public class postmethod : MonoBehaviour
{
    private string apiKey = "NjVkNDIyMjNmMjc3NmU3OTI5MWJmZGIzOjY1ZDQyMjIzZjI3NzZlNzkyOTFiZmRhOQ";
    private string loginEndpoint = "http://20.15.114.131:8080/api/login";
    //private string profileEndpoint = "http://20.15.114.131:8080/api/user/profile/view";
    public static string password = "NjVkNDIyMjNmMjc3NmU3OTI5MWJmZGIzOjY1ZDQyMjIzZjI3NzZlNzkyOTFiZmRhOQ";

    public static string jwtToken = "nulljwtToken";  // To pass jwt token to forward scene
    public string jwt = "nulljwt";
    public void retryScene() {
        SceneManager.LoadScene("StartScene");


    }
    public void quiteGame()
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

        using (UnityWebRequest request = UnityWebRequest.PostWwwForm(loginEndpoint, "")) // request handling

        {

            //request.certificateHandler = null; // Disable certificate validation
            request.SetRequestHeader("Content-Type", "application/json");
            request.uploadHandler = new UploadHandlerRaw(data);

            yield return request.SendWebRequest();

            if (request.responseCode == 200)
            {
                string response = request.downloadHandler.text;
                //string response_up = request.uploadHandler.text;
                //Debug.Log(response_up);
                jwt = response;
                Debug.Log("You are successfully authenticated. Your JWT token is:" + jwt);
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                try
                {
                    // Parse the JSON string
                    JObject jsonResponse = JObject.Parse(response);

                    // Extract the JWT token value
                    jwtToken = (string)jsonResponse["token"];
                    name = jwtToken;


                    Debug.Log("JWT Token: " + jwtToken);
                }
                catch (Exception ex)
                {
                    Debug.Log("Error parsing JSON: " + ex.Message);
                }
            }
            else if (request.responseCode == 400)
            {
                Debug.LogError("Bad Request: " + request.downloadHandler.text);
                // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
                SceneManager.LoadScene("ErrorPage");
            }
            else if (request.responseCode == 401)
            {
                Debug.LogError("Unauthorized: " + request.downloadHandler.text);
                //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
                SceneManager.LoadScene("ErrorPage");
            }
            else
            {
                Debug.LogError("Request failed: " + request.error);
                //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
                SceneManager.LoadScene("ErrorPage");
            }


        }
    }


    public void OnButtonClick()
    {
        StartCoroutine(PostRequest());
    }



}
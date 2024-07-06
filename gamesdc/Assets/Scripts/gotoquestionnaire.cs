
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine.UI;
using System.Net;

public class gotoquestionnaire : MonoBehaviour
{    
    public static string jwt_newone;
    public static int id_value;
    public static string id_value_string;
    [SerializeField] InputField input_firstname;
    [SerializeField] InputField input_lastname;
    [SerializeField] InputField input_username;
    [SerializeField] InputField input_nic;
    [SerializeField] InputField input_phonenumber;
    [SerializeField] InputField input_email;
    [SerializeField] Text resultText;


    public static string input_frist_name;
    public static string input_last_name;
    public static string input_user_name;
    public static string input_nic_name;
    public static string input_phonenumber_name;
    public static string input_email_name;
    public static string input_profileurl_name;
    public static string condition_check = "True";
    public void continueGame()
    {
        if (string.IsNullOrEmpty(input_firstname.text) ||
            string.IsNullOrEmpty(input_lastname.text) ||
            string.IsNullOrEmpty(input_username.text) ||
            string.IsNullOrEmpty(input_nic.text) ||
            string.IsNullOrEmpty(input_phonenumber.text) ||
            string.IsNullOrEmpty(input_email.text) 
            )
        {
            Debug.Log("Please fill in all required information");
            condition_check = "False";
        }
        else
        {
            Debug.Log("Gishan checking this" + input_frist_name);
            Debug.Log("Successfully continued Game");
            StartCoroutine(SendLoginRequest());
           
            if (input_frist_name == "Saman")
            {
                Debug.Log("Yes");
            }
            else
            {
                Debug.Log("Bad");
            }
        }
    }

    IEnumerator SendLoginRequest()
    {
        string username = viewprofile.username_var;
        //string password = "NjVkNDIyMjNmMjc3NmU3OTI5MWJmZGIzOjY1ZDQyMjIzZjI3NzZlNzkyOTFiZmRhOQ";
        //string username_1 = input_user_name.text;
        string password = postmethod.password;
        //Debug.Log("Auto assigned username and password are"+ username_1+password_1);



        var data = new
        {
            username = username,
            password = password
        };

        string jsonData = JsonConvert.SerializeObject(data);

        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);

        UnityWebRequest request = UnityWebRequest.PostWwwForm("http://localhost:8081/api/v1/player/log", jsonData);
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
            Debug.Log("Player id value getting"+response);
            jwt_newone = (string)jsonResponse["token"];
            Debug.Log(jwt_newone);
            int id_value = (int)jsonResponse["id"];

            //Debug.Log("Login successful");
            // string url = "http://localhost:5173/main/1";
            //string url = "https://www.bing.com/";
            //Debug.Log(request.result);
            //string response = request.downloadHandler.text;
            //string response_up = request.uploadHandler.text;
            //Debug.Log(response_up);
            //jwt_newone = response;
            //JObject jsonResponse = JObject.Parse(response);
            jwt_newone = (string)jsonResponse["token"];
            //int id_value = (int)jsonResponse["id"];
            //string url = "http://localhost:8081/api/v1/player/answer/" + id_value;
            //UnityWebRequest requestforcheckingQAdone = new UnityWebRequest(url, "GET");
            //string url = "http://localhost:5173/main/quiz/review/"+id_value +"/" + jwt_newone;
            //Debug.Log("You are successfully authenticated. Your JWT token is:" + jwt_newone);
            //Application.OpenURL(url);
            id_value_string = id_value.ToString();
            Debug.Log("Player id value is"+id_value_string);
            StartCheckingQAdone(id_value_string, jwt_newone);
        }
    }

    void StartCheckingQAdone(string id_value_string, string jwt_newone)
    {
        string url = "http://localhost:8081/api/v1/player/answer/" + id_value_string;
        // Assuming you want to send the request with an access token
        string accessToken = jwt_newone; // Replace this with your actual access token
        StartCoroutine(SendRequest(url, jwt_newone));
    }

    IEnumerator SendRequest(string url, string jwt_newone)
    {
        UnityWebRequest requestforcheckingQAdone = UnityWebRequest.Get(url);
        requestforcheckingQAdone.SetRequestHeader("Authorization", "Bearer " + jwt_newone);

        yield return requestforcheckingQAdone.SendWebRequest();

        if (requestforcheckingQAdone.result != UnityWebRequest.Result.Success)
        {
            string jsonResponse = requestforcheckingQAdone.downloadHandler.text;
            Debug.Log("Response: " + jsonResponse);

            try
            {
                JObject responseJson = JObject.Parse(jsonResponse);
               
                if (responseJson.TryGetValue("content", out JToken contentToken) &&
                    responseJson.TryGetValue("code", out JToken codeToken))
                {
                    if (contentToken.Type == JTokenType.Null && codeToken.ToString() == "01")
                    {
                        Debug.Log("Content is null and code is '01'");
                        // Assuming id_value_string and jwt_newone are accessible here
                        //string reviewUrl = "http://localhost:5173/main/quiz/" + id_value_string + "/" + jwt_newone;
                        string reviewUrl_start = "http://localhost:5173/main/" + id_value_string + "/" + jwt_newone;
                        Debug.Log(reviewUrl_start);
                        Application.OpenURL(reviewUrl_start);
                        //Debug.Log(reviewUrl);
                        //Application.OpenURL(reviewUrl);
                    }
                    else if (contentToken.Type != JTokenType.Null && codeToken.ToString() == "00")
                    {
                        Debug.Log("Content is not null, proceeding...");
                        string reviewUrl_start = "http://localhost:5173/main/quiz/review/" + id_value_string + "/" + jwt_newone;
                        Debug.Log(reviewUrl_start);
                        Application.OpenURL(reviewUrl_start);
                        // Proceed with other actions if needed
                    }
                    else
                    {
                        Debug.Log("Content is null but code is not '01'");
                        // Handle other cases if needed
                    }
                }
                else
                {
                    Debug.Log("No 'content' or 'code' field found in the response.");
                    // Handle missing fields if needed
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error parsing JSON response: " + e.Message);
                
            }

        }
        else
        {
            Debug.Log("Content is not null, proceeding...");
            string reviewUrl_start = "http://localhost:5173/main/quiz/review/" + id_value_string + "/" + jwt_newone;
            Debug.Log(reviewUrl_start);
            Application.OpenURL(reviewUrl_start);
            // Proceed with other actions if needed

            //string reviewUrl = "http://localhost:5173/main/quiz/" + id_value_string + "/" + jwt_newone;
            //Application.OpenURL(reviewUrl);

        }
    }


    public void Login()
    {
        continueGame();
        if (condition_check == "False")
        {
            resultText.text = "Can't go forward"+ "\n" + "Please fill in all required information";
            Debug.Log("Can't go forward");

        }
        else {
            StartCoroutine(SendLoginRequest());
        }
       
    }
}

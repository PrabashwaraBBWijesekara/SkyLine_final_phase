using Newtonsoft.Json.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class viewprofile : MonoBehaviour

{
    // Start is called before the first frame update
   
    public static string user_full_details = "null";
    private string profileEndpoint = "http://20.15.114.131:8080/api/user/profile/view";
    // Update is called once per frame
    public static string firstname_var;
    public static string lastname_var;
    public static string username_var;
    public static string nic_var; 
    public static string phonenumber_var;
    public static string email_var;
    public static string profilePictureUrl_var;


    public void get_profile_details() 
    {
        //Debug.Log(postmethod.jwtToken);
        StartCoroutine(GetProfile());
       
    }
    public IEnumerator GetProfile()
    {
        using (UnityWebRequest requestnew = UnityWebRequest.Get(profileEndpoint))
        {

            Debug.Log(name);

            requestnew.SetRequestHeader("Authorization", "Bearer " + postmethod.jwtToken);

            yield return requestnew.SendWebRequest();

            if (requestnew.result == UnityWebRequest.Result.Success)
            {
                string responsenew = requestnew.downloadHandler.text;
                JObject jsonResponse = JObject.Parse(responsenew);

                // Extracting each piece of data
                string firstname = (string)jsonResponse["user"]["firstname"];
                string lastname = (string)jsonResponse["user"]["lastname"];
                string username = (string)jsonResponse["user"]["username"];
                string nic = (string)jsonResponse["user"]["nic"];
                string phoneNumber = (string)jsonResponse["user"]["phoneNumber"];
                string email = (string)jsonResponse["user"]["email"];
                string profilePictureUrl = (string)jsonResponse["user"]["profilePictureUrl"];

                // Logging each piece of data
                Debug.Log("Firstname: " + firstname);
                Debug.Log("Lastname: " + lastname);
                Debug.Log("Username: " + username);
                Debug.Log("NIC: " + nic);
                Debug.Log("Phone Number: " + phoneNumber);
                Debug.Log("Email: " + email);
                Debug.Log("Profile Picture URL: " + profilePictureUrl);
                firstname_var=firstname;
                lastname_var=lastname;
                username_var=username;
                nic_var=nic;
                phonenumber_var = phoneNumber;
                email_var = email;
                profilePictureUrl_var = profilePictureUrl;
              
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

            }

            else if (requestnew.responseCode == 401)
            {
                Debug.LogError("Failed to get profile data: Unauthorized (401). Check the JWT token validity and permissions.");
            }
            else
            {
                Debug.LogError("Failed to get profile data: " + requestnew.error);
            }
        }
    }
}

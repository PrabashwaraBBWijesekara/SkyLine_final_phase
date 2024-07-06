using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;



public class UpdateOnePlayerDetails : MonoBehaviour

{
    // Make sure to assign the actual JWT token to this variable.
    string jwtToken = postmethod.jwtToken;
    [SerializeField] InputField input_firstname;
    [SerializeField] InputField input_lastname;
    [SerializeField] InputField input_username;
    [SerializeField] InputField input_nic;
    [SerializeField] InputField input_phonenumber;
    [SerializeField] InputField input_email;
  


    public static string input_frist_name;
    public static string input_last_name;
    public static string input_user_name;
    public static string input_nic_name;
    public static string input_phonenumber_name;
    public static string input_email_name;
   


    private void Start()
    {
        input_firstname.text = viewprofile.firstname_var;

        // Update last name
        input_lastname.text = viewprofile.lastname_var;

        // Update username
        input_username.text = viewprofile.username_var;

        // Update NIC
        input_nic.text = viewprofile.nic_var;

        // Update phone number
        input_phonenumber.text = viewprofile.phonenumber_var;

        // Update email
        input_email.text = viewprofile.email_var;

        // Update profile URL
       
    }
    public void UpdateProfileInformations()
    {
        StartCoroutine(SendProfileUpdateRequest());
    }

    IEnumerator SendProfileUpdateRequest()
    {
        // Ensure that the input field is assigned in the Unity Editor.
        if (input_firstname == null)
        {
            Debug.LogError("InputField input_firstname is not assigned in the Unity Editor.");
            yield break;
        }

        input_frist_name = input_firstname.text;
        input_last_name = input_lastname.text;
        input_user_name = input_username.text;
        input_nic_name = input_nic.text;
        input_phonenumber_name = input_phonenumber.text;
        input_email_name = input_email.text;
     

        // Use string.Format or interpolation to insert the variable values into the JSON string.
        string jsonProfileUpdate = string.Format(@"{{
        ""firstname"": ""{0}"",
        ""lastname"": ""{1}"",
        ""username"": ""{2}"",
        ""nic"": ""{3}"",
        ""phoneNumber"": ""{4}"",
        ""email"": ""{5}""
        
    }}", input_frist_name, input_last_name, input_user_name, input_nic_name, input_phonenumber_name, input_email_name);
        //,input_user_name, input_nic_name, input_phonenumber_name, input_email_name, input_profileurl_name
        Debug.Log(input_nic_name);
        Debug.Log(input_phonenumber_name);

        byte[] data = System.Text.Encoding.UTF8.GetBytes(jsonProfileUpdate);

        using (UnityWebRequest request = UnityWebRequest.Put("http://20.15.114.131:8080/api/user/profile/update", data))
        {
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + jwtToken);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                if (request.responseCode == 200)
                {
                    Debug.Log("200");
                }
                Debug.Log("Profile updated successfully.");
            }
            else
            {
                Debug.Log("Error updating profile: " + request.error);
            }
        }
    }
    public void continueGame()
    {


        if (input_frist_name == "Saman")
        {
            Debug.Log("Yes");
        }





        if (string.IsNullOrEmpty(input_firstname.text) ||
            string.IsNullOrEmpty(input_lastname.text) ||
            string.IsNullOrEmpty(input_username.text) ||
            string.IsNullOrEmpty(input_nic.text) ||
            string.IsNullOrEmpty(input_phonenumber.text) ||
            string.IsNullOrEmpty(input_email.text) 
           )
        {
            Debug.Log("Please fill in all required information");
        }
        else
        {
            Debug.Log("Successfully continued Game");
            Debug.Log(input_frist_name);
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



}



using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class view_one_player_details : MonoBehaviour

{

  
    [SerializeField] Text resultText;
    private void Start()
    {
        resultText.text = "Firstname:   " + viewprofile.firstname_var + "\n" +
                 "Lastname:   " + viewprofile.lastname_var + "\n" +
                 "Username:   " + viewprofile.username_var + "\n" +
                 "NIC:   " + viewprofile.nic_var + "\n" +
                 "Phone Number:   " + viewprofile.phonenumber_var + "\n" +
                 "Email:   " + viewprofile.email_var; 
               
    }

    public void ValidateInput()
    { 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}









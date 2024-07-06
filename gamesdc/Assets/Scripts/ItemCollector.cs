
using System.Collections;
using System.Collections.Generic;
using EasyUI.popupmessages;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemCollector : MonoBehaviour
{
    public static int coins;
    public static int gems;
   
    [SerializeField] private AudioSource audioSource; // Add an AudioSource field
    [SerializeField] private AudioClip messageTone;

    void Start()
    {
        //GameManager.Instance.onDataReady += HandleDataReady;
        PlayerMovements.OnValueAssigned += OnValueAssigned;
        Debug.Log("condition gishan check" + PlayerMovements.condition_check_value);
        
    }

    private void OnDestroy()
    {
        // It's important to unsubscribe from the event when the object is destroyed
        PlayerMovements.OnValueAssigned -= OnValueAssigned;
    }



    private void OnValueAssigned()
    {
        // Now you can safely use the value assigned in ScriptA
        if (PlayerMovements.Instance != null) // Check if the instance is valid
        {
            int valueToUse = PlayerMovements.Instance.ImportantValue;
            Debug.Log("Received value in ScriptB: " + valueToUse);
            if (PlayerMovements.Instance.ImportantValue == 1)
            {
                //coins = PlayerMovements.initial_coins_value;
                coins = 100;
                gems = PlayerMovements.Instance.ImportantValue_for_coins;
                //gems = PlayerMovements.initial_gems_value;
                //gems= 100;
                Debug.Log("Initial coins value is"+PlayerMovements.initialCoinsValue);
                Debug.Log("Gishan will Win this competition" + PlayerMovements.Instance.ImportantValue_for_coins);
            }
            else
            {
                coins = PlayerMovements.Instance.ImportantValue_for_coins;
                gems = PlayerMovements.Instance.ImportantValue_for_gems;
                //coins = PlayerMovements.initial_coins_value;
                //gems = PlayerMovements.initial_gems_value;
                //gems = PlayerMovements.initial_gems_value; ;
                Debug.Log("Set player games values2" + PlayerMovements.initial_gems_value);


            }

            // Perform further actions based on the value
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //coins = int.Parse(coinsCount.text);
        if (collision.gameObject.CompareTag("coins"))
        {
            Destroy(collision.gameObject);
            if (audioSource != null && messageTone != null)
            {
                audioSource.PlayOneShot(messageTone);
            }
            //PlayerMovements.initial_coins_value++;
            coins++;
            //coinsCount.text = "" + coins;
            
        }
        //gems = int.Parse(gemsCount.text);
        if (collision.gameObject.CompareTag("gems"))
        {
            Destroy(collision.gameObject);
            if (audioSource != null && messageTone != null)
            {
                audioSource.PlayOneShot(messageTone);
            }
            //PlayerMovements.initial_gems_value++;
            gems++;

            //gemsCount.text = "" + gems;

        }
    }

}




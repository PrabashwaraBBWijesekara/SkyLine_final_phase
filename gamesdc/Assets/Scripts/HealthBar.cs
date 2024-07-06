using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using EasyUI.popupmessages;
using UnityEngine.SceneManagement;
using UnityEditor;

public class HeartBar : MonoBehaviour
{
    public int maxHealth = 100;
    public float currentHealth;
    [SerializeField] private Slider healthBar;
    [SerializeField] private Image healthBarFill;
    [SerializeField] private AudioSource audioSource; // Add an AudioSource field
    [SerializeField] private AudioClip messageTone;
    [SerializeField] private AudioSource warningaudioSource; // Add an AudioSource field
    [SerializeField] private AudioClip warningTone;
    private readonly float damageInterval = 10f;
    private float nextDamageTime = 0f;
    private bool isBlinking = false;

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.maxValue = maxHealth;
        healthBar.value = maxHealth;
    }

    void Update()
    {
        if (Time.time >= nextDamageTime)
        {
            ApplyDamage(10); // Apply some default damage value for testing
            nextDamageTime = Time.time + damageInterval;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("heart"))
        {
            Destroy(collision.gameObject);
            if (audioSource != null && messageTone != null)
            {
                audioSource.PlayOneShot(messageTone);
            }
            currentHealth += 20;
            healthBar.value = currentHealth;

        }


    }

    public void ApplyDamage(float power_change)
    {
        currentHealth -= power_change * 0.1f;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        healthBar.value = currentHealth;

        if (currentHealth < 30)
        {
            if (audioSource != null && messageTone != null)
            {
                audioSource.PlayOneShot(messageTone);
            }
            if (!isBlinking)
            {
                StartCoroutine(BlinkHealthBar());
            }
        }
        else
        {
            if (isBlinking)
            {
                StopCoroutine(BlinkHealthBar());
                healthBarFill.color = Color.red;
                isBlinking = false;
            }
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        PopupMessageUI.Instance
            .SetTitle("Oops")
            .SetMessage("You Lost!!  Try Again")
            .SetReward(false)
            .Show();
        StartCoroutine(WaitAndLoadScene());
    }

    IEnumerator WaitAndLoadScene()
    {
        yield return new WaitForSeconds(5);  // Wait for 10 seconds
        SceneManager.LoadScene("MainMenu");
    }


    IEnumerator BlinkHealthBar()
    {
        isBlinking = true;
        while (currentHealth < 30)
        {
            healthBarFill.color = Color.red;
            yield return new WaitForSeconds(0.5f);
            healthBarFill.color = Color.black;
            yield return new WaitForSeconds(0.5f);
        }
        isBlinking = false;
    }
}

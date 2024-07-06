using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace EasyUI.popupmessages
{
    public static class PopupMessage
    {
        public static string Title;
        public static string Message;
        public static Sprite Image;
    }

    public class PopupMessageUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text messageText;
        [SerializeField] private Image messageUIImage;
        [SerializeField] private Button button;


        [SerializeField] private Sprite reward;
        [SerializeField] private Sprite penalty;
        [SerializeField] private AudioSource audioSource; // Add an AudioSource field
        [SerializeField] private AudioClip messageTone; // Add an AudioClip field for the tone


        private static PopupMessageUI instance;
        public static PopupMessageUI Instance
        {
            get
            {
                if (instance == null)
                {
                    // Find the instance in the scene even if it's inactive
                    instance = FindObjectOfType<PopupMessageUI>(true);

                    instance.Initialize();
                }
                return instance;
            }
        }

        private void Initialize()
        {
            if (button != null)
            {
                button.onClick.AddListener(Hide);
            }
            else
            {
                Debug.LogError("Button is not assigned in the inspector.");
            }
        }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                Initialize();
                gameObject.SetActive(false); // Ensure the panel is initially inactive
            }
            else if (instance != this)
            {
                Destroy(gameObject); // Ensure there's only one instance
            }
        }


        public PopupMessageUI SetTitle(string title)
        {
            PopupMessage.Title = title;
            UpdateUI();
            return Instance;
        }

        public PopupMessageUI SetMessage(string message)
        {
            PopupMessage.Message = message;
            UpdateUI();
            return Instance;
        }

        public PopupMessageUI SetReward(bool isReward)
        {
            // Set the appropriate image based on the parameter
            PopupMessage.Image = isReward ? reward : penalty;
            UpdateUI();
            return Instance;
        }

        private void UpdateUI()
        {
            if (titleText != null)
            {
                titleText.text = PopupMessage.Title;
            }
            if (messageText != null)
            {
                messageText.text = PopupMessage.Message;
            }
            if (messageUIImage != null)
            {
                messageUIImage.sprite = PopupMessage.Image;
                messageUIImage.gameObject.SetActive(PopupMessage.Image != null);
            }

        }

        public void Hide()
        {
            gameObject.SetActive(false); // Hide the popup UI.
        }

        public void Show()
        {
            gameObject.SetActive(true); // Show the popup UI.
            UpdateUI();
            // Play the tone
            if (audioSource != null && messageTone != null)
            {
                audioSource.PlayOneShot(messageTone);
            }
        }
    }
}

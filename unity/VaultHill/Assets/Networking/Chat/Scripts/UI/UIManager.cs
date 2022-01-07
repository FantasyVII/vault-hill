using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject welcomePanel;
    [SerializeField] Button connectToServerButton;
    [SerializeField] InputField usernameInputField;

    [SerializeField] GameObject chatUI;
    [SerializeField] Button sendChatMessageButton;
    [SerializeField] InputField chatMessageInputField;

    [SerializeField] GameObject chatContent;
    [SerializeField] GameObject messagePrefab;

    ChatNetwork chatNetwork;

    void Start()
    {
        chatNetwork = FindObjectOfType<ChatNetwork>();
        chatNetwork.ConnectedToServer += OnConnectedToServer;
        chatNetwork.RecievedChatMessage += OnRecievedChatMessage;

        connectToServerButton.onClick.AddListener(() =>
        {
            chatNetwork.ConnectToServer(usernameInputField.text);
        });

        sendChatMessageButton.onClick.AddListener(() =>
        {
            chatNetwork.SendChatMessage(chatMessageInputField.text);
            GameObject go = Instantiate(messagePrefab, chatContent.transform);
            go.GetComponent<Text>().text = $"{chatNetwork.player.Name}: {chatMessageInputField.text}";
            chatMessageInputField.text = "";
        });
    }

    void Update()
    {

    }

    void OnConnectedToServer()
    {
        welcomePanel.SetActive(false);
        chatUI.SetActive(true);
    }

    void OnRecievedChatMessage(string name, string message)
    {
        GameObject go = Instantiate(messagePrefab, chatContent.transform);
        go.GetComponent<Text>().text = $"{name}: {message}";
    }

    void OnDestroy()
    {
        chatNetwork.ConnectedToServer -= OnConnectedToServer;
    }
}
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

    [SerializeField] GameObject gameUI;
    [SerializeField] Button sendChatMessageButton;
    [SerializeField] InputField chatMessageInputField;

    [SerializeField] GameObject chatContent;
    [SerializeField] GameObject messagePrefab;

    [SerializeField] Button spawnButton;

    ChatNetwork chatNetwork;
    GameNetwork gameNetwork;

    void Start()
    {
        chatNetwork = FindObjectOfType<ChatNetwork>();
        chatNetwork.ConnectedToServer += OnConnectedToChatServer;
        chatNetwork.RecievedChatMessage += OnRecievedChatMessage;

        gameNetwork = FindObjectOfType<GameNetwork>();
        gameNetwork.ConnectedToServer += OnConnectedToGameServer;
        
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

        spawnButton.onClick.AddListener(() =>
        {
            gameNetwork.InstantiatePlayer();
            spawnButton.gameObject.SetActive(false);
        });
    }

    void Update()
    {

    }

    void OnConnectedToChatServer()
    {
        gameNetwork.ConnectToServer(usernameInputField.text);
    }

    private void OnConnectedToGameServer()
    {
        welcomePanel.SetActive(false);
        gameUI.SetActive(true);
    }

    void OnRecievedChatMessage(string name, string message)
    {
        GameObject go = Instantiate(messagePrefab, chatContent.transform);
        go.GetComponent<Text>().text = $"{name}: {message}";
    }

    void OnDestroy()
    {
        chatNetwork.ConnectedToServer -= OnConnectedToChatServer;
        gameNetwork.ConnectedToServer -= OnConnectedToGameServer;
    }
}
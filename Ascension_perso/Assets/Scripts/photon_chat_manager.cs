using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChatManager : MonoBehaviourPunCallbacks
{
    [Header("UI References")]
    public TMP_InputField chatInputField;
    public GameObject chatPanel;
    public GameObject messagePrefab;
    public GameObject chatContent;
    public ScrollRect scrollRect;
    public int maxMessages = 25;

    private List<GameObject> messageList = new List<GameObject>();
    private bool isChatting = false;

    void Start()
    {
        // S'assurer que le panneau de chat est fermé au démarrage
        chatPanel.SetActive(false);
    }

    void Update()
    {
        // Ouvrir/fermer le chat avec la touche T ou Entrée
        if (Input.GetKeyDown(KeyCode.T) || Input.GetKeyDown(KeyCode.Return))
        {
            ToggleChat();
        }

        // Envoyer un message avec Entrée quand le chat est actif
        if (isChatting && Input.GetKeyDown(KeyCode.Return) && chatInputField.text.Trim() != "")
        {
            SendChatMessage();
        }

        // Fermer le chat avec Échap
        if (isChatting && Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleChat();
        }
    }

    void ToggleChat()
    {
        isChatting = !isChatting;
        chatPanel.SetActive(isChatting);

        if (isChatting)
        {
            chatInputField.ActivateInputField();
            chatInputField.Select();
        }
        else
        {
            chatInputField.text = "";
            chatInputField.DeactivateInputField();
        }
    }

    public void SendChatMessage()
    {
        if (chatInputField.text.Trim() == "")
            return;

        string message = chatInputField.text;
        
        // Envoyer le message à tous les joueurs via RPC
        photonView.RPC("RPC_AddChatMessage", RpcTarget.All, 
            PhotonNetwork.NickName, message, PhotonNetwork.LocalPlayer.ActorNumber);

        // Nettoyer le champ de saisie
        chatInputField.text = "";
        chatInputField.ActivateInputField();
    }

    [PunRPC]
    void RPC_AddChatMessage(string senderName, string message, int senderID)
    {
        // Créer un nouvel élément de message
        GameObject newMessage = Instantiate(messagePrefab, chatContent.transform);
        
        // Définir le texte du message
        TextMeshProUGUI messageText = newMessage.GetComponent<TextMeshProUGUI>();
        
        // Mettre en forme différemment les messages du joueur local
        if (senderID == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            messageText.text = "<color=#00FFFF>" + senderName + ":</color> " + message;
        }
        else
        {
            messageText.text = "<color=#FFFF00>" + senderName + ":</color> " + message;
        }
        
        // Faire défiler automatiquement vers le bas
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
        
        // Ajouter le message à la liste
        messageList.Add(newMessage);
        
        // Supprimer les anciens messages si nécessaire
        if (messageList.Count > maxMessages)
        {
            Destroy(messageList[0]);
            messageList.RemoveAt(0);
        }
    }

    // Ajouter un message système
    public void AddSystemMessage(string message)
    {
        photonView.RPC("RPC_AddSystemMessage", RpcTarget.All, message);
    }

    [PunRPC]
    void RPC_AddSystemMessage(string message)
    {
        // Créer un nouvel élément de message
        GameObject newMessage = Instantiate(messagePrefab, chatContent.transform);
        
        // Définir le texte du message
        TextMeshProUGUI messageText = newMessage.GetComponent<TextMeshProUGUI>();
        messageText.text = "<color=#FF00FF>SYSTÈME:</color> " + message;
        
        // Faire défiler automatiquement vers le bas
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
        
        // Ajouter le message à la liste
        messageList.Add(newMessage);
        
        // Supprimer les anciens messages si nécessaire
        if (messageList.Count > maxMessages)
        {
            Destroy(messageList[0]);
            messageList.RemoveAt(0);
        }
    }

    public void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddSystemMessage(newPlayer.Name + " a rejoint la partie");
    }

    public void OnPlayerLeftRoom(Player otherPlayer)
    {
        AddSystemMessage(otherPlayer.Name + " a quitté la partie");
    }
}
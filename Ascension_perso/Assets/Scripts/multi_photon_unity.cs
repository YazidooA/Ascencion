using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerNetworkController : MonoBehaviourPunCallbacks
{
    [Header("Player Settings")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 100f;
    
    [Header("Components")]
    public Camera playerCamera;
    public GameObject playerModel;
    
    private void Start()
    {
        // Désactiver la caméra si ce n'est pas notre joueur local
        if (!photonView.IsMine)
        {
            if (playerCamera != null)
                playerCamera.gameObject.SetActive(false);
        }
    }
    
    private void Update()
    {
        // Ne contrôle le joueur que s'il nous appartient
        if (photonView.IsMine)
        {
            // Gestion du mouvement
            HandleMovement();
            
            // Gestion de la rotation
            HandleRotation();
        }
    }
    
    private void HandleMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        
        // Calculer la direction du mouvement
        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput).normalized;
        
        // Appliquer le mouvement
        transform.Translate(movement * moveSpeed * Time.deltaTime, Space.Self);
    }
    
    private void HandleRotation()
    {
        // Rotation avec la souris horizontalement
        float mouseX = Input.GetAxis("Mouse X");
        transform.Rotate(Vector3.up, mouseX * rotationSpeed * Time.deltaTime);
    }
    
    // Fonction pour envoyer un message à tous les joueurs
    public void SendMessage(string message)
    {
        if (photonView.IsMine)
        {
            photonView.RPC("ReceiveMessage", RpcTarget.All, message);
        }
    }
    
    [PunRPC]
    void ReceiveMessage(string message, PhotonMessageInfo info)
    {
        Debug.Log($"Message de {info.Sender.NickName}: {message}");
        // Ici vous pouvez implémenter l'affichage du message dans l'UI
    }
}
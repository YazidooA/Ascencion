using Photon.Pun;
using UnityEngine;

public class InteractiveObject : MonoBehaviourPun
{
    [SerializeField] private bool isActive = true;
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    private void Awake()
    {
        if (!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateVisuals();
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Vérifier si c'est un joueur et s'il est local
        PlayerController player = other.GetComponent<PlayerController>();
        if (player && player.photonView.IsMine)
        {
            // Si c'est notre joueur local, on peut interagir
            if (Input.GetKeyDown(KeyCode.E))
            {
                // Appelons une méthode RPC pour synchroniser sur tous les clients
                photonView.RPC("ToggleObjectState", RpcTarget.All);
            }
        }
    }
    
    [PunRPC]
    private void ToggleObjectState()
    {
        isActive = !isActive;
        UpdateVisuals();
        
        // Exécuter une logique supplémentaire si nécessaire
        if (isActive)
        {
            // Faire quelque chose quand activé
        }
        else
        {
            // Faire quelque chose quand désactivé
        }
    }
    
    private void UpdateVisuals()
    {
        if (spriteRenderer)
        {
            // Changer l'apparence en fonction de l'état
            spriteRenderer.color = isActive ? Color.green : Color.red;
        }
    }
}
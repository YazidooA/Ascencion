using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlatformerCamera : MonoBehaviour
{
    [Header("Cible")]
    private Transform target;
    
    [Header("Limites")]
    public bool useBoundaries = true;
    public float minX = -10f;
    public float maxX = 10f;
    public float minY = -10f;
    public float maxY = 10f;
    
    [Header("Paramètres")]
    public float smoothTime = 0.3f;
    public Vector3 offset = new Vector3(0, 2, -10);
    
    // Variables privées
    private Vector3 velocity = Vector3.zero;
    
    void Start()
    {
        // Trouver notre joueur local (celui qui a un PhotonView qui nous appartient)
        FindLocalPlayer();
    }
    
    void LateUpdate()
    {
        if (target != null)
        {
            // Position cible
            Vector3 targetPosition = target.position + offset;
            
            // Appliquer un smooth
            Vector3 smoothPosition = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
            
            // Limiter la position si nécessaire
            if (useBoundaries)
            {
                smoothPosition.x = Mathf.Clamp(smoothPosition.x, minX, maxX);
                smoothPosition.y = Mathf.Clamp(smoothPosition.y, minY, maxY);
            }
            
            // Appliquer la position
            transform.position = smoothPosition;
        }
        else
        {
            // Si notre cible a disparu, essayer de la retrouver
            FindLocalPlayer();
        }
    }
    
    void FindLocalPlayer()
    {
        // Chercher tous les PhotonView de la scène
        PhotonView[] views = FindObjectsOfType<PhotonView>();
        
        foreach (PhotonView view in views)
        {
            // Si c'est un joueur et qu'il nous appartient
            if (view.IsMine && view.gameObject.GetComponent<PlatformerController>() != null)
            {
                target = view.transform;
                break;
            }
        }
    }
    
    // Méthode pour définir la cible manuellement
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
    
    // Méthode pour mettre à jour les limites de la caméra en fonction du niveau
    public void UpdateBoundaries(float newMinX, float newMaxX, float newMinY, float newMaxY)
    {
        minX = newMinX;
        maxX = newMaxX;
        minY = newMinY;
        maxY = newMaxY;
    }
    
    // Visualiser les limites de la caméra
    void OnDrawGizmosSelected()
    {
        if (useBoundaries)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(new Vector3(minX, minY, 0), new Vector3(maxX, minY, 0));
            Gizmos.DrawLine(new Vector3(maxX, minY, 0), new Vector3(maxX, maxY, 0));
            Gizmos.DrawLine(new Vector3(maxX, maxY, 0), new Vector3(minX, maxY, 0));
            Gizmos.DrawLine(new Vector3(minX, maxY, 0), new Vector3(minX, minY, 0));
        }
    }
}
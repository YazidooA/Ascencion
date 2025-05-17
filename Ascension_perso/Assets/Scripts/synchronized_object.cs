using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SynchronizedObject : MonoBehaviourPun, IPunObservable
{
    [Header("Synchronization Settings")]
    public bool synchronizePosition = true;
    public bool synchronizeRotation = true;
    public bool synchronizeScale = false;
    
    [Header("Interpolation")]
    public float lerpSpeed = 10f;
    
    // Valeurs réseau qui seront interpolées localement
    private Vector3 networkPosition;
    private Quaternion networkRotation;
    private Vector3 networkScale;
    
    // Pour éviter les mises à jour sur nous-mêmes
    private bool firstTimeSync = true;
    
    private void Awake()
    {
        // Initialiser les valeurs réseau
        networkPosition = transform.position;
        networkRotation = transform.rotation;
        networkScale = transform.localScale;
    }
    
    private void Update()
    {
        // Ne synchroniser que si ce n'est pas notre objet
        if (!photonView.IsMine)
        {
            // Interpoler la position pour un mouvement fluide
            if (synchronizePosition)
                transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * lerpSpeed);
                
            // Interpoler la rotation
            if (synchronizeRotation)
                transform.rotation = Quaternion.Lerp(transform.rotation, networkRotation, Time.deltaTime * lerpSpeed);
                
            // Interpoler l'échelle
            if (synchronizeScale)
                transform.localScale = Vector3.Lerp(transform.localScale, networkScale, Time.deltaTime * lerpSpeed);
        }
    }
    
    // Implémentation de l'interface IPunObservable
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // Si nous sommes celui qui envoie les données
        if (stream.IsWriting)
        {
            // Envoyer position, rotation, échelle selon les paramètres
            if (synchronizePosition)
                stream.SendNext(transform.position);
                
            if (synchronizeRotation)
                stream.SendNext(transform.rotation);
                
            if (synchronizeScale)
                stream.SendNext(transform.localScale);
        }
        // Si nous recevons les données
        else
        {
            // Recevoir les données dans le même ordre
            if (synchronizePosition)
                networkPosition = (Vector3)stream.ReceiveNext();
                
            if (synchronizeRotation)
                networkRotation = (Quaternion)stream.ReceiveNext();
                
            if (synchronizeScale)
                networkScale = (Vector3)stream.ReceiveNext();
                
            // Si c'est la première synchronisation, on applique immédiatement
            if (firstTimeSync)
            {
                firstTimeSync = false;
                
                if (synchronizePosition)
                    transform.position = networkPosition;
                    
                if (synchronizeRotation)
                    transform.rotation = networkRotation;
                    
                if (synchronizeScale)
                    transform.localScale = networkScale;
            }
        }
    }
}
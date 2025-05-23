using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Camera Settings")]
    public float FollowSpeed = 2f;
    public float yOffset = 1f;
    
    [Header("Target Settings")]
    public Transform target;
    public string playerTag = "Player"; // Tag du joueur à chercher
    public bool autoFindPlayer = true; // Chercher automatiquement le joueur
    
    [Header("Search Settings")]
    public float searchInterval = 0.5f; // Intervalle de recherche en secondes
    
    private Coroutine searchCoroutine;

    void Start()
    {
        // Ne pas chercher automatiquement au start
        // La recherche se fera seulement quand le joueur spawn dans la scène
    }

    void Update()
    {
        // Suivre la cible seulement si elle existe
        if (target != null)
        {
            Vector3 newPos = new Vector3(target.position.x, target.position.y + yOffset, -10f);
            transform.position = Vector3.Slerp(transform.position, newPos, FollowSpeed * Time.deltaTime);
        }
        // Pas de recherche automatique en Update - seulement sur demande
    }

    // Méthode pour assigner manuellement une cible (appelée par votre fonction de spawn)
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        StopSearchingForPlayer();
        Debug.Log("Camera target set to: " + newTarget.name);
    }

    // Méthode pour assigner une cible par GameObject
    public void SetTarget(GameObject targetObject)
    {
        if (targetObject != null)
        {
            SetTarget(targetObject.transform);
        }
    }

    // Commencer la recherche automatique du joueur
    public void StartSearchingForPlayer()
    {
        if (searchCoroutine == null)
        {
            searchCoroutine = StartCoroutine(SearchForPlayerCoroutine());
        }
    }

    // Méthode à appeler APRÈS que le joueur ait spawné dans la scène
    public void StartFollowingPlayer()
    {
        FindPlayerNow();
        if (target == null && autoFindPlayer)
        {
            StartSearchingForPlayer();
        }
    }

    // Arrêter la recherche automatique
    public void StopSearchingForPlayer()
    {
        if (searchCoroutine != null)
        {
            StopCoroutine(searchCoroutine);
            searchCoroutine = null;
        }
    }

    // Coroutine qui cherche périodiquement le joueur
    private IEnumerator SearchForPlayerCoroutine()
    {
        while (target == null)
        {
            // Chercher par tag
            GameObject player = GameObject.FindGameObjectWithTag(playerTag);
            
            if (player != null)
            {
                SetTarget(player.transform);
                yield break; // Sortir de la coroutine
            }

            // Attendre avant de chercher à nouveau
            yield return new WaitForSeconds(searchInterval);
        }
        
        searchCoroutine = null;
    }

    // Méthode utilitaire pour forcer une recherche immédiate
    public void FindPlayerNow()
    {
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player != null)
        {
            SetTarget(player.transform);
        }
    }

    // Effacer la cible actuelle
    public void ClearTarget()
    {
        target = null;
        if (autoFindPlayer)
        {
            StartSearchingForPlayer();
        }
    }

    void OnDestroy()
    {
        StopSearchingForPlayer();
    }
}
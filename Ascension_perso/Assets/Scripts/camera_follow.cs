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
    public string playerTag = "Player"; 
    public bool autoFindPlayer = true; 
    
    [Header("Search Settings")]
    public float searchInterval = 0.5f;
    
    private Coroutine searchCoroutine;

    void Start()
    {
        // Ne pas chercher automatiquement au start
        // La recherche se fera seulement quand le joueur spawn dans la scène
    }

    void Update()
    {
        if (target != null)
        {
            Vector3 newPos = new Vector3(target.position.x, target.position.y + yOffset, -10f);
            transform.position = Vector3.Slerp(transform.position, newPos, FollowSpeed * Time.deltaTime);
        }
    }
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        StopSearchingForPlayer();
        Debug.Log("Camera target set to: " + newTarget.name);
    }
    public void SetTarget(GameObject targetObject)
    {
        if (targetObject != null) SetTarget(targetObject.transform);
        
    }
    public void StartSearchingForPlayer()
    {
        if (searchCoroutine == null) searchCoroutine = StartCoroutine(SearchForPlayerCoroutine());
    }

    public void StartFollowingPlayer()
    {
        FindPlayerNow();
        if (target == null && autoFindPlayer) StartSearchingForPlayer();
    }
    public void StopSearchingForPlayer()
    {
        if (searchCoroutine != null)
        {
            StopCoroutine(searchCoroutine);
            searchCoroutine = null;
        }
    }
    private IEnumerator SearchForPlayerCoroutine()
    {
        while (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag(playerTag);
            if (player != null)
            {
                SetTarget(player.transform);
                yield break;
            }
            yield return new WaitForSeconds(searchInterval);
        }
        searchCoroutine = null;
    }
    public void FindPlayerNow()
    {
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player != null) SetTarget(player.transform);
    }
    public void ClearTarget()
    {
        target = null;
        if (autoFindPlayer) StartSearchingForPlayer();
    }

    void OnDestroy() => StopSearchingForPlayer();
    
}
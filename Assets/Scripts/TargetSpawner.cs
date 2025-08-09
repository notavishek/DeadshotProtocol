using UnityEngine;
using System.Collections.Generic;

public class TargetSpawner : MonoBehaviour
{
    [Header("Spawning Settings")]
    public GameObject targetPrefab;
    public Transform spawnPlane;
    public int maxTargets = 3;
    public float minDistanceBetweenTargets = 3f;
    
    [Header("Spawn Area")]
    public Vector2 spawnAreaSize = new Vector2(8f, 8f);
    
    private List<GameObject> activeTargets = new List<GameObject>();
    
    void Start()
    {
        SpawnInitialTargets();
    }
    
    void SpawnInitialTargets()
    {
        for (int i = 0; i < maxTargets; i++)
        {
            SpawnTarget();
        }
    }
    
    public void OnTargetDestroyed(GameObject destroyedTarget)
    {
        if (activeTargets.Contains(destroyedTarget))
        {
            activeTargets.Remove(destroyedTarget);
        }
        
        // Spawn a new target immediately to maintain 3 targets
        SpawnTarget();
    }
    
    void SpawnTarget()
    {
        Vector3 spawnPosition = GetRandomSpawnPosition();
        int attempts = 0;
        
        // Try to find a position that's not too close to existing targets
        while (IsPositionTooClose(spawnPosition) && attempts < 20)
        {
            spawnPosition = GetRandomSpawnPosition();
            attempts++;
        }
        
        GameObject newTarget = Instantiate(targetPrefab, spawnPosition, Quaternion.identity);
        activeTargets.Add(newTarget);
        
        // Make sure the target knows about this spawner
        SimplePerson targetScript = newTarget.GetComponent<SimplePerson>();
        if (targetScript != null)
        {
            targetScript.SetSpawner(this);
        }
    }
    
    Vector3 GetRandomSpawnPosition()
{
    Vector3 basePosition = spawnPlane ? spawnPlane.position : Vector3.zero;
    Vector3 playerPosition = Vector3.zero;
    
    // Find the player position to avoid spawning too close
    GameObject player = GameObject.FindWithTag("Player");
    if (player == null)
    {
        // If no player tag, try to find FPSPlayer
        player = GameObject.Find("FPSPlayer");
    }
    
    if (player != null)
    {
        playerPosition = new Vector3(player.transform.position.x, 0, player.transform.position.z);
    }
    
    Vector3 spawnPosition;
    int attempts = 0;
    
    do
    {
        // Generate random position on the plane
        float randomX = Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2);
        float randomZ = Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2);
        
        spawnPosition = basePosition + new Vector3(randomX, 1f, randomZ);
        attempts++;
        
    } while (Vector3.Distance(new Vector3(spawnPosition.x, 0, spawnPosition.z), playerPosition) < 10f && attempts < 30);
    // Ensures targets spawn at least 5 units away from player
    
    Debug.Log("Generated spawn position: " + spawnPosition + " (distance from player: " + 
              Vector3.Distance(new Vector3(spawnPosition.x, 0, spawnPosition.z), playerPosition) + ")");
    
    return spawnPosition;
}

    
    bool IsPositionTooClose(Vector3 position)
    {
        foreach (GameObject target in activeTargets)
        {
            if (target != null && Vector3.Distance(position, target.transform.position) < minDistanceBetweenTargets)
            {
                return true;
            }
        }
        return false;
    }
}

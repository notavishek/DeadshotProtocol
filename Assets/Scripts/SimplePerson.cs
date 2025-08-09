using UnityEngine;

public class SimplePerson : MonoBehaviour
{
    [Header("Target Settings")]
    public float maxHealth = 1f;
    
    private float currentHealth;
    private bool isDead = false;
    private TargetSpawner spawner;
    
    void Start()
    {
        currentHealth = maxHealth;
        AdjustTargetHeight();
        Debug.Log("SimplePerson target created at: " + transform.position);
    }
    
    void AdjustTargetHeight()
    {
        // Adjust the target height so head is at shooter eye level (around 1.6-1.7 units)
        Transform head = transform.Find("Head");
        Transform body = transform.Find("Body");
        
        if (head != null && body != null)
        {
            // Lower the entire target so head is at eye level
            head.localPosition = new Vector3(0, 0.9f, 0);  // Lowered from 1.5f
            body.localPosition = new Vector3(0, 0.02f, 0);  // Lowered from 0.6f
            
            Debug.Log("Target height adjusted - Head at: " + head.position.y + ", Body at: " + body.position.y);
        }
    }
    
    public void TakeDamage(float damage, bool isHeadshot = false)
    {
        if (isDead) 
        {
            Debug.Log("Target already dead, ignoring damage");
            return;
        }
        
        Debug.Log("TakeDamage called! Damage: " + damage + ", Headshot: " + isHeadshot);
        
        currentHealth -= damage;
        
        // Award points based on hit type
        int points = isHeadshot ? 50 : 25;
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(points);
            
            // Show hit feedback
            if (isHeadshot)
            {
                GameManager.Instance.ShowHitFeedback("HEADSHOT!", Color.red);
            }
            else
            {
                GameManager.Instance.ShowHitFeedback("BODYSHOT", Color.yellow);
            }
        }
        
        // Target dies after any hit
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    void Die()
    {
        if (isDead) return;
        
        isDead = true;
        Debug.Log("Target dying... Spawner reference: " + (spawner != null ? spawner.name : "null"));
        
        // Notify spawner BEFORE destroying the object
        if (spawner != null)
        {
            Debug.Log("Calling OnTargetDestroyed on spawner");
            spawner.OnTargetDestroyed(gameObject);
        }
        else
        {
            Debug.LogError("Spawner is null! Cannot notify of target destruction.");
        }
        
        // Destroy after a short delay to ensure spawner gets the message
        Destroy(gameObject, 0.1f);
    }
    
    public void SetSpawner(TargetSpawner targetSpawner)
    {
        spawner = targetSpawner;
        Debug.Log("Spawner set for target: " + (targetSpawner != null ? targetSpawner.name : "null"));
    }
}

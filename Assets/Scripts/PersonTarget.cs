using UnityEngine;

public class PersonTarget : MonoBehaviour
{
    [Header("Person Target Settings")]
    public float maxHealth = 100f;
    public bool respawnOnDeath = false;
    
    [Header("Body Parts")]
    public GameObject headPart;
    public GameObject bodyPart;
    
    private float currentHealth;
    private bool isDead = false;
    private TargetSpawner spawner;
    
    void Awake()
    {
        SetupPersonTarget();
    }
    
    void Start()
    {
        currentHealth = maxHealth;
    }
    
    void SetupPersonTarget()
    {
        // Clear any existing colliders on the main object
        Collider[] existingColliders = GetComponents<Collider>();
        for (int i = 0; i < existingColliders.Length; i++)
        {
            DestroyImmediate(existingColliders[i]);
        }
        
        // Remove any existing renderers on the main object
        Renderer existingRenderer = GetComponent<Renderer>();
        if (existingRenderer != null)
        {
            DestroyImmediate(existingRenderer);
        }
        
        // Create head if it doesn't exist
        if (headPart == null)
        {
            headPart = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            headPart.transform.SetParent(transform);
            headPart.transform.localPosition = new Vector3(0, 1.5f, 0);
            headPart.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
            headPart.name = "Head";
            headPart.tag = "Head";
            
            // Make head yellow/orange for visibility
            headPart.GetComponent<Renderer>().material.color = new Color(1f, 0.8f, 0.2f); // Orange-yellow
        }
        
        // Create body if it doesn't exist
        if (bodyPart == null)
        {
            bodyPart = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            bodyPart.transform.SetParent(transform);
            bodyPart.transform.localPosition = new Vector3(0, 0.6f, 0);
            bodyPart.transform.localScale = new Vector3(0.6f, 1.2f, 0.6f);
            bodyPart.name = "Body";
            bodyPart.tag = "Body";
            
            // Make body blue for visibility
            bodyPart.GetComponent<Renderer>().material.color = new Color(0.2f, 0.4f, 1f); // Blue
        }
        
        Debug.Log("Person target created with head and body parts");
    }
    
    public void TakeDamage(float damage, bool isHeadshot = false)
    {
        if (isDead) return;
        
        currentHealth -= damage;
        
        // Visual feedback
        StartCoroutine(FlashWhite(isHeadshot));
        
        // Award points based on hit type
        int points = isHeadshot ? 50 : 25;
        GameManager.Instance.AddScore(points);
        
        Debug.Log("Target took " + damage + " damage. Headshot: " + isHeadshot + ". Points awarded: " + points);
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    void Die()
    {
        isDead = true;
        
        // Notify spawner to spawn a new target
        if (spawner != null)
        {
            spawner.OnTargetDestroyed(gameObject);
        }
        
        Destroy(gameObject);
    }
    
    public void SetSpawner(TargetSpawner targetSpawner)
    {
        spawner = targetSpawner;
    }
    
    System.Collections.IEnumerator FlashWhite(bool isHeadshot)
    {
        Color flashColor = isHeadshot ? Color.red : Color.white;
        
        if (headPart != null)
        {
            Renderer headRenderer = headPart.GetComponent<Renderer>();
            Color originalHeadColor = headRenderer.material.color;
            headRenderer.material.color = flashColor;
            yield return new WaitForSeconds(0.15f);
            if (headRenderer != null)
                headRenderer.material.color = originalHeadColor;
        }
        
        if (bodyPart != null)
        {
            Renderer bodyRenderer = bodyPart.GetComponent<Renderer>();
            Color originalBodyColor = bodyRenderer.material.color;
            bodyRenderer.material.color = flashColor;
            yield return new WaitForSeconds(0.15f);
            if (bodyRenderer != null)
                bodyRenderer.material.color = originalBodyColor;
        }
    }
}

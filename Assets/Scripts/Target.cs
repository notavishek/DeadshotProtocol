using UnityEngine;

public enum TargetType
{
    Basic,
    Armored,
    Fast,
    Giant
}

public class Target : MonoBehaviour
{
    [Header("Target Settings")]
    public TargetType targetType = TargetType.Basic;
    public float maxHealth = 100f;
    public int pointValue = 10;
    public bool respawnOnDeath = true;
    public float respawnTime = 3f;
    
    [Header("Movement (for Fast targets)")]
    public bool shouldMove = false;
    public float moveSpeed = 2f;
    public float moveRange = 5f;
    
    private float currentHealth;
    private Vector3 originalPosition;
    private Vector3 targetPosition;
    private bool isDead = false;
    // Removed unused variable: private bool movingToTarget = true;
    
    void Start()
    {
        SetupTargetType();
        currentHealth = maxHealth;
        originalPosition = transform.position;
        
        if (shouldMove)
        {
            SetNewTargetPosition();
        }
    }
    
    void SetupTargetType()
    {
        switch (targetType)
        {
            case TargetType.Basic:
                maxHealth = 50f;
                pointValue = 10;
                GetComponent<Renderer>().material.color = Color.red;
                transform.localScale = Vector3.one;
                break;
                
            case TargetType.Armored:
                maxHealth = 150f;
                pointValue = 25;
                GetComponent<Renderer>().material.color = Color.gray;
                transform.localScale = Vector3.one;
                break;
                
            case TargetType.Fast:
                maxHealth = 30f;
                pointValue = 20;
                GetComponent<Renderer>().material.color = Color.yellow;
                transform.localScale = Vector3.one * 0.7f;
                shouldMove = true;
                moveSpeed = 4f;
                break;
                
            case TargetType.Giant:
                maxHealth = 300f;
                pointValue = 50;
                GetComponent<Renderer>().material.color = Color.blue;
                transform.localScale = Vector3.one * 2f;
                break;
        }
    }
    
    void Update()
    {
        if (shouldMove && !isDead)
        {
            MoveTarget();
        }
    }
    
    void MoveTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            SetNewTargetPosition();
        }
    }
    
    void SetNewTargetPosition()
    {
        Vector3 randomDirection = Random.insideUnitCircle.normalized * moveRange;
        targetPosition = originalPosition + new Vector3(randomDirection.x, 0, randomDirection.y);
    }
    
    public void TakeDamage(float damage)
    {
        if (isDead) return;
        
        currentHealth -= damage;
        StartCoroutine(FlashWhite());
        
        // Show damage number
        ShowDamageNumber(damage);
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    void ShowDamageNumber(float damage)
    {
        // Create floating damage text
        GameObject damageText = new GameObject("DamageText");
        damageText.transform.position = transform.position + Vector3.up * 2f;
        
        TextMesh textMesh = damageText.AddComponent<TextMesh>();
        textMesh.text = "-" + damage.ToString("F0");
        textMesh.color = Color.red;
        textMesh.fontSize = 20;
        textMesh.anchor = TextAnchor.MiddleCenter;
        
        // Make it face camera
        damageText.transform.LookAt(Camera.main.transform);
        damageText.transform.Rotate(0, 180, 0);
        
        Destroy(damageText, 1f);
    }
    
    void Die()
    {
        isDead = true;
        GameManager.Instance.AddScore(pointValue);
        
        if (respawnOnDeath)
        {
            StartCoroutine(RespawnTarget());
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    System.Collections.IEnumerator FlashWhite()
    {
        Renderer renderer = GetComponent<Renderer>();
        Color originalColor = renderer.material.color;
        
        renderer.material.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        
        if (renderer != null)
            renderer.material.color = originalColor;
    }
    
    System.Collections.IEnumerator RespawnTarget()
    {
        gameObject.SetActive(false);
        yield return new WaitForSeconds(respawnTime);
        
        currentHealth = maxHealth;
        isDead = false;
        
        Vector3 newPos = originalPosition + new Vector3(
            Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f)
        );
        transform.position = newPos;
        originalPosition = newPos;
        
        gameObject.SetActive(true);
    }
}

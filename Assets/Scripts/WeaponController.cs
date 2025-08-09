using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Weapon Settings")]
    public float damage = 1f; // Only need 1 damage since targets have 1 health
    public float range = 100f;
    public float fireRate = 15f;
    public int maxAmmo = 30;
    public float reloadTime = 1f;
    
    [Header("References")]
    public Camera fpsCam;
    public Transform bulletSpawnPoint;
    public ParticleSystem muzzleFlash;
    
    [Header("Audio")]
    public AudioClip shootSound;
    public AudioClip reloadSound;
    public AudioClip emptyClipSound;
    private AudioSource audioSource;
    
    private int currentAmmo;
    private float nextTimeToFire = 0f;
    private bool isReloading = false;
    
    void Start()
    {
        currentAmmo = maxAmmo;
        
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        
        if (fpsCam == null)
        {
            fpsCam = GetComponentInParent<Camera>();
            if (fpsCam == null)
                fpsCam = Camera.main;
        }
            
        if (bulletSpawnPoint == null)
            bulletSpawnPoint = transform;
            
        GenerateSoundEffects();
    }
    
    void GenerateSoundEffects()
    {
        if (shootSound == null)
            shootSound = GenerateShootSound();
        if (reloadSound == null)
            reloadSound = GenerateReloadSound();
        if (emptyClipSound == null)
            emptyClipSound = GenerateEmptySound();
    }
    
    AudioClip GenerateShootSound()
    {
        int sampleRate = 44100;
        float duration = 0.1f;
        int samples = Mathf.FloorToInt(sampleRate * duration);
        float[] audioData = new float[samples];
        
        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / samples;
            float frequency = Mathf.Lerp(800f, 200f, t);
            float amplitude = Mathf.Lerp(0.3f, 0f, t);
            
            audioData[i] = amplitude * Mathf.Sin(2 * Mathf.PI * frequency * t) * 
                          (Random.Range(0.8f, 1.2f));
        }
        
        AudioClip clip = AudioClip.Create("GeneratedShootSound", samples, 1, sampleRate, false);
        clip.SetData(audioData, 0);
        return clip;
    }
    
    AudioClip GenerateReloadSound()
    {
        int sampleRate = 44100;
        float duration = 0.3f;
        int samples = Mathf.FloorToInt(sampleRate * duration);
        float[] audioData = new float[samples];
        
        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / samples;
            
            if (t < 0.1f || (t > 0.2f && t < 0.3f))
            {
                float frequency = 400f;
                audioData[i] = 0.2f * Mathf.Sin(2 * Mathf.PI * frequency * t) * Random.Range(0.5f, 1f);
            }
        }
        
        AudioClip clip = AudioClip.Create("GeneratedReloadSound", samples, 1, sampleRate, false);
        clip.SetData(audioData, 0);
        return clip;
    }
    
    AudioClip GenerateEmptySound()
    {
        int sampleRate = 44100;
        float duration = 0.05f;
        int samples = Mathf.FloorToInt(sampleRate * duration);
        float[] audioData = new float[samples];
        
        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / samples;
            audioData[i] = 0.1f * Mathf.Sin(2 * Mathf.PI * 1000f * t) * (1 - t);
        }
        
        AudioClip clip = AudioClip.Create("GeneratedEmptySound", samples, 1, sampleRate, false);
        clip.SetData(audioData, 0);
        return clip;
    }
    
    void Update()
    {
        if (isReloading)
            return;
            
        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo)
        {
            StartCoroutine(Reload());
            return;
        }
        
        if (Input.GetMouseButtonDown(0) && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
        }
    }
    
    void Shoot()
    {
        if (currentAmmo <= 0)
        {
            if (emptyClipSound != null)
                audioSource.PlayOneShot(emptyClipSound);
            StartCoroutine(Reload());
            return;
        }
        
        currentAmmo--;
        
        if (shootSound != null)
            audioSource.PlayOneShot(shootSound);
        
        if (muzzleFlash != null)
            muzzleFlash.Play();
        
        // Raycast shooting with headshot detection
        RaycastHit hit;
        Vector3 rayOrigin = fpsCam.transform.position;
        Vector3 rayDirection = fpsCam.transform.forward;
        
        if (Physics.Raycast(rayOrigin, rayDirection, out hit, range))
        {
            bool isHeadshot = false;
            
            // Check if we hit a head or body part
            if (hit.collider.CompareTag("Head"))
            {
                isHeadshot = true;
            }
            else if (hit.collider.CompareTag("Body"))
            {
                isHeadshot = false;
            }
            
            // Find the SimplePerson target component (might be on parent)
            SimplePerson target = hit.collider.GetComponent<SimplePerson>();
            if (target == null)
            {
                target = hit.collider.GetComponentInParent<SimplePerson>();
            }
            
            if (target != null)
            {
                target.TakeDamage(damage, isHeadshot);
            }
            
            CreateImpactEffect(hit.point, hit.normal);
        }
    }
    
    void CreateImpactEffect(Vector3 position, Vector3 normal)
    {
        GameObject impact = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        impact.transform.position = position;
        impact.transform.localScale = Vector3.one * 0.1f;
        
        Renderer renderer = impact.GetComponent<Renderer>();
        if (renderer != null)
            renderer.material.color = Color.red;
        
        Destroy(impact, 0.5f);
    }
    
    System.Collections.IEnumerator Reload()
    {
        isReloading = true;
        
        if (reloadSound != null)
            audioSource.PlayOneShot(reloadSound);
        
        yield return new WaitForSeconds(reloadTime);
        
        currentAmmo = maxAmmo;
        isReloading = false;
    }
    
    void OnGUI()
    {
        // UI style for ammo
        GUIStyle uiBoldStyle = new GUIStyle(GUI.skin.label);
        uiBoldStyle.fontStyle = FontStyle.Bold;
        uiBoldStyle.normal.textColor = Color.black;
        uiBoldStyle.fontSize = 14;
        GUI.Label(new Rect(10, 10, 200, 28), "Ammo: " + currentAmmo + "/" + maxAmmo, uiBoldStyle);

        if (isReloading)
            GUI.Label(new Rect(10, 38, 200, 20), "RELOADING...", uiBoldStyle);
    }
}

using UnityEngine;

public class MultiWeaponController : MonoBehaviour
{
    [Header("Weapon System")]
    public WeaponData[] availableWeapons;
    public Transform weaponHolder;
    public Camera fpsCam;
    public Transform bulletSpawnPoint;
    
    private int currentWeaponIndex = 0;
    private WeaponData currentWeapon;
    private int currentAmmo;
    private float nextTimeToFire = 0f;
    private bool isReloading = false;
    private AudioSource audioSource;
    private GameObject currentWeaponModel;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
            
        if (fpsCam == null)
            fpsCam = GetComponentInParent<Camera>();
            
        if (availableWeapons.Length > 0)
            SwitchWeapon(0);
    }
    
    void Update()
    {
        HandleWeaponSwitching();
        
        if (isReloading)
            return;
            
        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < currentWeapon.maxAmmo)
        {
            StartCoroutine(Reload());
            return;
        }
        
        HandleShooting();
    }
    
    void HandleWeaponSwitching()
    {
        // Switch weapons with number keys
        for (int i = 0; i < availableWeapons.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                SwitchWeapon(i);
                break;
            }
        }
        
        // Mouse wheel switching
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f)
        {
            SwitchWeapon((currentWeaponIndex + 1) % availableWeapons.Length);
        }
        else if (scroll < 0f)
        {
            SwitchWeapon((currentWeaponIndex - 1 + availableWeapons.Length) % availableWeapons.Length);
        }
    }
    
    void SwitchWeapon(int weaponIndex)
    {
        if (weaponIndex >= availableWeapons.Length || weaponIndex < 0)
            return;
            
        // Destroy old weapon model
        if (currentWeaponModel != null)
            Destroy(currentWeaponModel);
            
        currentWeaponIndex = weaponIndex;
        currentWeapon = availableWeapons[weaponIndex];
        currentAmmo = currentWeapon.maxAmmo;
        isReloading = false;
        
        // Spawn new weapon model
        if (currentWeapon.weaponModel != null && weaponHolder != null)
        {
            currentWeaponModel = Instantiate(currentWeapon.weaponModel, weaponHolder);
        }
        
        Debug.Log("Switched to: " + currentWeapon.weaponName);
    }
    
    void HandleShooting()
    {
        bool shouldShoot = false;
        
        if (currentWeapon.isAutomatic)
        {
            shouldShoot = Input.GetButton("Fire1");
        }
        else
        {
            shouldShoot = Input.GetButtonDown("Fire1");
        }
        
        if (shouldShoot && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / currentWeapon.fireRate;
            Shoot();
        }
    }
    
    void Shoot()
    {
        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }
        
        currentAmmo--;
        
        // Play shooting sound
        if (currentWeapon.shootSound != null)
            audioSource.PlayOneShot(currentWeapon.shootSound);
        
        // Muzzle flash
        if (currentWeapon.muzzleFlash != null)
            currentWeapon.muzzleFlash.Play();
        
        // Shooting logic
        RaycastHit hit;
        Vector3 rayOrigin = fpsCam.transform.position;
        Vector3 rayDirection = fpsCam.transform.forward;
        
        if (Physics.Raycast(rayOrigin, rayDirection, out hit, currentWeapon.range))
        {
            Target target = hit.transform.GetComponent<Target>();
            if (target != null)
            {
                target.TakeDamage(currentWeapon.damage);
            }
            
            CreateImpactEffect(hit.point, hit.normal);
        }
    }
    
    void CreateImpactEffect(Vector3 position, Vector3 normal)
    {
        GameObject impact = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        impact.transform.position = position;
        impact.transform.localScale = Vector3.one * 0.1f;
        impact.GetComponent<Renderer>().material.color = Color.yellow;
        Destroy(impact, 0.5f);
    }
    
    System.Collections.IEnumerator Reload()
    {
        isReloading = true;
        
        if (currentWeapon.reloadSound != null)
            audioSource.PlayOneShot(currentWeapon.reloadSound);
        
        yield return new WaitForSeconds(currentWeapon.reloadTime);
        
        currentAmmo = currentWeapon.maxAmmo;
        isReloading = false;
    }
    
    void OnGUI()
    {
        // UI styles for weapon/ammo
        GUIStyle uiBoldStyle = new GUIStyle(GUI.skin.label);
        uiBoldStyle.fontStyle = FontStyle.Bold;
        uiBoldStyle.normal.textColor = Color.black;
        uiBoldStyle.fontSize = 14;

        GUI.Label(new Rect(10, 10, 200, 28), "Weapon: " + currentWeapon.weaponName, uiBoldStyle);
        GUI.Label(new Rect(10, 38, 200, 28), "Ammo: " + currentAmmo + "/" + currentWeapon.maxAmmo, uiBoldStyle);

        if (isReloading)
            GUI.Label(new Rect(10, 66, 200, 20), "RELOADING...", uiBoldStyle);

        GUI.Label(new Rect(10, 94, 300, 20), "Switch: 1-" + availableWeapons.Length + " or Mouse Wheel", uiBoldStyle);
    }
}

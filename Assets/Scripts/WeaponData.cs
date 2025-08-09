using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapons/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("Weapon Info")]
    public string weaponName;
    public int weaponID;
    
    [Header("Stats")]
    public float damage = 25f;
    public float range = 100f;
    public float fireRate = 15f;
    public int maxAmmo = 30;
    public float reloadTime = 1f;
    public bool isAutomatic = false;
    
    [Header("Audio")]
    public AudioClip shootSound;
    public AudioClip reloadSound;
    
    [Header("Visual")]
    public GameObject weaponModel;
    public ParticleSystem muzzleFlash;
}

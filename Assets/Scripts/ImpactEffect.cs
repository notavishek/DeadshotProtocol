using UnityEngine;

public class ImpactEffect : MonoBehaviour
{
    public static void CreateImpact(Vector3 position, Vector3 normal)
    {
        // Create impact particle system
        GameObject impactGO = new GameObject("Impact Effect");
        impactGO.transform.position = position;
        impactGO.transform.LookAt(position + normal);
        
        ParticleSystem particles = impactGO.AddComponent<ParticleSystem>();
        
        var main = particles.main;
        main.startLifetime = 0.5f;
        main.startSpeed = 5f;
        main.startSize = 0.1f;
        main.startColor = Color.yellow;
        main.maxParticles = 20;
        
        var emission = particles.emission;
        emission.SetBursts(new ParticleSystem.Burst[]
        {
            new ParticleSystem.Burst(0f, 15)
        });
        
        var shape = particles.shape;
        shape.shapeType = ParticleSystemShapeType.Hemisphere;
        shape.radius = 0.1f;
        
        // Destroy after particles finish
        Destroy(impactGO, 2f);
    }
}

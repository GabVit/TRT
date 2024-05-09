using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


public class StaticShooter : Weapon
{
    [SerializeField] private Projectile bulletPrefab;
    [SerializeField] private float shootingInterval = 3f; // Intervalo de tiempo entre disparos en segundos
    [SerializeField] private float debugRayDuration = 2f; // Duración del rayo de debug en segundos
    public GameObject bulletHolePrefab;
    private void Update()
    {
        // Dibujar el rayo de debug en cada frame
        DrawDebugRaycast();
    }
    
    private void DrawDebugRaycast()
    {
        // Dibujar el rayo de debug
        Debug.DrawRay(bulletSpawn.position, bulletSpawn.forward * 100f, Color.yellow, debugRayDuration);
    }

    protected virtual void Start()
    {
        StartCoroutine(ShootRoutine()); // Comienza la rutina de disparo
    }

    protected IEnumerator ShootRoutine()
    {
        while (true)
        {
            // Draw debug raycast from bullet spawn point
           
            yield return new WaitForSeconds(shootingInterval); // Espera el intervalo de tiempo antes del próximo disparo
            Shoot(); // Realiza un disparo
        }
    }



    protected override void Shoot()
    {
        
        base.Shoot();
        
        Projectile projectileInstance = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
        projectileInstance.Init(this);
        projectileInstance.Launch();
        
         RaycastHit hit;
        if (Physics.Raycast(bulletSpawn.position, bulletSpawn.forward, out hit))
        {
             if (hit.collider.CompareTag("Enemy") || hit.collider.CompareTag("Civil"))
            {
            // No hacer nada si el objeto impactado es un enemigo o un civil
            return; 
            }
            // Instanciar el objeto bullet hole en el punto de impacto// Instanciar el objeto bullet hole en el punto de impacto
            InstantiateBulletHole(hit.point, hit.normal, hit.collider.transform);
            
        }


    }

    private void InstantiateBulletHole(Vector3 position, Vector3 normal, Transform parent)
    {
        // Calcular la rotación necesaria para alinear el eje hacia arriba con la normal de la superficie
    Quaternion rotation = Quaternion.FromToRotation(Vector3.up, normal);

    // Instanciar el objeto bullet hole en el punto de impacto con la rotación adecuada y como hijo del objeto impactado
    GameObject bulletHoleInstance = Instantiate(bulletHolePrefab, position, rotation, parent);
    Vector3 normalizedScale = new Vector3(
        0.4f / parent.localScale.x,
        0.005f / parent.localScale.y,
        0.4f / parent.localScale.z
    );

    // Establecer la escala normalizada para el agujero de bala
    bulletHoleInstance.transform.localScale = normalizedScale;
}

}
using UnityEngine;

public class VegetationScatterer : MonoBehaviour
{
    [Header("Zone de plantation")]
    public MeshRenderer groundRenderer; 

    [Header("Les Végétaux (Prefabs)")]
    public GameObject[] grassPrefabs;   // Herbes
    public GameObject[] flowerPrefabs;  // Fleurs
    public GameObject[] fernPrefabs;    // Fougères

    [Header("Répartition")]
    [Range(10, 2000)]
    public int totalObjects = 500; 
    
    [Tooltip("Pourcentage de chance d'apparaître")]
    [Range(0f, 100f)] public float pourcentageHerbe = 75f;
    [Range(0f, 100f)] public float pourcentageFleurs = 15f;
    [Range(0f, 100f)] public float pourcentageFougeres = 10f;

    [Header("Variations Organiques")]
    public float minScale = 0.8f;
    public float maxScale = 1.2f;

    [Header("Limites (Boule à neige)")]
    public Vector2 centreCercle = new Vector2(-5f, 5f);
    public float rayonCercle = 25f;

    void Start()
    {
    }

    [ContextMenu("Générer la Végétation")]
    public void ScatterVegetation()
    {
        if (groundRenderer == null)
        {
            Debug.LogError("VegetationScatterer : Il manque le Ground Renderer !");
            return;
        }

        Collider groundCollider = groundRenderer.GetComponent<Collider>();
        if (groundCollider == null)
        {
            Debug.LogError("ERREUR : Ton sol n'a pas de Collider !");
            return;
        }

        Transform oldContainer = transform.Find("Vegetation_Automatique");
        if (oldContainer != null)
        {
            DestroyImmediate(oldContainer.gameObject);
        }

        Bounds groundBounds = groundRenderer.bounds;

        GameObject parentContainer = new GameObject("Vegetation_Automatique");
        parentContainer.transform.parent = this.transform;

        int spawnedCount = 0;
        // J'augmente un peu la limite de sécurité car on va "rejeter" beaucoup de points hors du cercle
        int maxAttempts = totalObjects * 20; 
        int attempts = 0;

        while (spawnedCount < totalObjects && attempts < maxAttempts)
        {
            attempts++;

            float randomX = Random.Range(groundBounds.min.x, groundBounds.max.x);
            float randomZ = Random.Range(groundBounds.min.z, groundBounds.max.z);

            // --- NOUVEAU : On vérifie si le point est dans le cercle ---
            Vector2 pointActuel = new Vector2(randomX, randomZ);
            if (Vector2.Distance(pointActuel, centreCercle) > rayonCercle)
            {
                continue; // Le point est en dehors du dôme, on l'ignore et on passe au suivant
            }

            Vector3 rayStart = new Vector3(randomX, groundBounds.max.y + 10f, randomZ);

            if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, 100f))
            {
                if (hit.collider.gameObject == groundRenderer.gameObject)
                {
                    Vector3 spawnPosition = hit.point;

                    // --- CHOIX DU TYPE DE PLANTE SELON LES POURCENTAGES ---
                    GameObject[] selectedArray = null;
                    float randomRoll = Random.Range(0f, 100f);

                    if (randomRoll < pourcentageHerbe)
                    {
                        selectedArray = grassPrefabs;
                    }
                    else if (randomRoll < pourcentageHerbe + pourcentageFleurs)
                    {
                        selectedArray = flowerPrefabs;
                    }
                    else
                    {
                        selectedArray = fernPrefabs;
                    }

                    // Sécurité : si la liste choisie est vide, on passe à la tentative suivante
                    if (selectedArray == null || selectedArray.Length == 0) continue;

                    // On pioche au hasard dans la liste sélectionnée
                    int prefabIndex = Random.Range(0, selectedArray.Length);
                    GameObject prefabToSpawn = selectedArray[prefabIndex];

                    // On conserve la rotation simple uniquement sur l'axe Y
                    Quaternion randomRotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
                    float randomScaleMult = Random.Range(minScale, maxScale);
                    Vector3 finalScale = prefabToSpawn.transform.localScale * randomScaleMult;

                    GameObject spawnedObj = Instantiate(prefabToSpawn, spawnPosition, randomRotation);
                    spawnedObj.transform.parent = parentContainer.transform; 
                    spawnedObj.transform.localScale = finalScale; 

                    spawnedCount++; 
                }
            }
        }

        Debug.Log($"Végétation générée ! {spawnedCount} objets placés dans le cercle. ({attempts} tentatives)");
    }
}
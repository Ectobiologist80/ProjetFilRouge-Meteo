using UnityEngine;

public class BirdFlight : MonoBehaviour
{
    [Header("Trajectoire")]
    public Vector2 centreCercle = new Vector2(2f, 4f);
    public float rayonCercle = 16f; // Un peu plus petit que 25 pour éviter de taper la vitre !
    public float hauteurVol = 12f;  // Hauteur dans le ciel
    public float vitesseVol = 0.5f; 

    [Tooltip("Décalage (en secondes) pour éviter que 2 oiseaux se superposent")]
    public float decalageTemps = 0f;

    void Update()
    {
        float angle = (Time.time * vitesseVol) + decalageTemps;

        float nouveauX = centreCercle.x + Mathf.Cos(angle) * rayonCercle;
        float nouveauZ = centreCercle.y + Mathf.Sin(angle) * rayonCercle;
        
        Vector3 nouvellePosition = new Vector3(nouveauX, hauteurVol, nouveauZ);

        Vector3 directionDeVol = nouvellePosition - transform.position;
        if (directionDeVol != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(directionDeVol);
        }

        transform.position = nouvellePosition;
    }
}
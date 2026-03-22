using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using TMPro;

public class SimpleWeather : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI temperatureText;
    public TextMeshProUGUI windspeedText;

    [Header("Weather Objects")]
    public GameObject birdsContainer;
    public GameObject sun;
    public GameObject snow;
    public GameObject clouds;
    public GameObject rain;

    [Header("Objets sensibles à la neige")]
    [Tooltip("Glisser ici les objets ou dossiers qui doivent disparaître quand il neige (ex: herbe, les fleurs)")]
    public GameObject[] objectsToHideInSnow; // tableau d'objets à cacher

    [Header("Wind Settings")]
    public Transform windsleeveTransform; // 0 = North, 90 = East, 180 = South, 270 = West
    public AudioSource windAudioSource;
    //animation de la windsleeve
    private float currentWindDirection = 0f;
    private float currentWindSpeed = 0f;

    [Header("Lighting & Sky")]
    public Light sunLight;

    [Header("Ambiance Sonore")]
    public AudioSource birdsAudioSource; // Source audio pour les oiseaux

    // URL de l'API Open-Meteo
    private const string API_URL = "https://api.open-meteo.com/v1/forecast?latitude=48.25&longitude=-71.03&current_weather=true";

    void Start()
    {
        // ---- INITIALISATION DE LA SCÈNE ----
        if (Borodar.FarlandSkies.LowPoly.SkyboxDayNightCycle.Instance != null)
        {
            Borodar.FarlandSkies.LowPoly.SkyboxDayNightCycle.Instance.TimeOfDay = 50f;
            RenderSettings.reflectionIntensity = 1.0f; 
            DynamicGI.UpdateEnvironment();
        }

        StartCoroutine(GetWeather());
        StartCoroutine(AnimateWind());
    }

    // ---- COROUTINE DE RÉCUPÉRATION DE LA MÉTÉO ----
    IEnumerator GetWeather()
    {
        UnityWebRequest request = UnityWebRequest.Get(API_URL);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            WeatherData data = JsonUtility.FromJson<WeatherData>(request.downloadHandler.text);
            CurrentWeather current = data.current_weather;

            // ---- RECUPERATION DES DONNÉES ----
            float temp = current.temperature;
            int code = current.weathercode;
            float vitesseVent = current.windspeed;
            float directionVent = current.winddirection;
            int isDay = current.is_day;

            // ---- MISE À JOUR DES INTERFACES (UI) ----
            temperatureText.text = temp + "°C";
            windspeedText.text = vitesseVent + " km/h";

            // ---- GESTION DU VENT (Rotation windsleeve & Volume) ---- 
            currentWindDirection = directionVent;
            currentWindSpeed = vitesseVent;
            if (windAudioSource != null)
            {
                windAudioSource.volume = Mathf.Clamp01(vitesseVent / 40f);
            }

            // ---- GESTION DES OISEAUX ----
            if (birdsAudioSource != null)
            {
                // On considère qu'il fait beau s'il n'y a pas de pluie/neige (code <= 3) ET qu'il fait jour
                if (code <= 3 && isDay == 1)
                {
                    if (!birdsAudioSource.isPlaying) birdsAudioSource.Play();
                }
                else
                {
                    if (birdsAudioSource.isPlaying) birdsAudioSource.Stop();
                }
            }

            if (birdsContainer != null)
            {
                birdsContainer.SetActive(code <= 3 && isDay == 1);
            }

            // ---- GESTION DES OBJETS MÉTÉO (soleil, nuages, pluie, neige) ----
            if (clouds != null) clouds.SetActive((code >= 1 && code <= 3) || (code >= 51 && code <= 67) || (code >= 80 && code <= 82) || (code >= 71 && code <= 77));
            if (rain != null) rain.SetActive((code >= 51 && code <= 67) || (code >= 80 && code <= 82));
            
                // --- GESTION DE LA NEIGE ET DES OBJETS ---
            bool isSnowing = (code >= 71 && code <= 77); // On vérifie s'il neige
            if (snow != null) snow.SetActive(isSnowing);
            // On passe en revue tous les objets dans la liste
            foreach (GameObject obj in objectsToHideInSnow)
            {
                if (obj != null)
                {
                    // Si ça neige, on les désactive (faux). Sinon on les active (vrai).
                    obj.SetActive(!isSnowing); 
                }
            }

            // ---- GESTION DU JOUR ET DE LA NUIT ----
            if (Borodar.FarlandSkies.LowPoly.SkyboxDayNightCycle.Instance != null)
            {
                if (isDay == 1)
                {
                    // JOUR : Ciel à midi et reflets forts
                    Borodar.FarlandSkies.LowPoly.SkyboxDayNightCycle.Instance.TimeOfDay = 50f;
                    RenderSettings.reflectionIntensity = 1.0f; // 100% de reflets
                }
                else
                {
                    // NUIT : Ciel à minuit et reflets très faibles
                    Borodar.FarlandSkies.LowPoly.SkyboxDayNightCycle.Instance.TimeOfDay = 0f;
                    RenderSettings.reflectionIntensity = 0.1f; // 10% de reflets (juste assez pour la lune/étoiles)
                }
                DynamicGI.UpdateEnvironment();
            }
        }
        else
        {
            Debug.LogError("Erreur lors de la récupération de la météo : " + request.error);
        }
    }

    // ---- COROUTINE D'ANIMATION DE LA WINDSLEEVE ----
    IEnumerator AnimateWind()
    {
        ParticleSystem[] snowParticles = snow != null ? snow.GetComponentsInChildren<ParticleSystem>() : new ParticleSystem[0];
        ParticleSystem[] rainParticles = rain != null ? rain.GetComponentsInChildren<ParticleSystem>() : new ParticleSystem[0];

        while (true) 
        {
            if (windsleeveTransform != null && currentWindSpeed > 0)
            {
                float oscillation = Mathf.Sin(Time.time * (currentWindSpeed * 0.05f)); 
                float amplitude = Mathf.Clamp(currentWindSpeed * 0.1f, 0f, 5f); 
                float angleRafale = oscillation * amplitude;

                windsleeveTransform.rotation = Quaternion.Euler(angleRafale, currentWindDirection, 0);

                float forceVentBase = currentWindSpeed * 0.2f; 
                float forceBourrasque = forceVentBase + (oscillation * forceVentBase * 0.5f); 

                float angleRad = currentWindDirection * Mathf.Deg2Rad;
                float ventX = Mathf.Sin(angleRad) * forceBourrasque;
                float ventZ = Mathf.Cos(angleRad) * forceBourrasque;

                foreach (var ps in snowParticles)
                {
                    var vol = ps.velocityOverLifetime;
                    vol.enabled = true; 
                    vol.space = ParticleSystemSimulationSpace.World;
                    vol.x = new ParticleSystem.MinMaxCurve(ventX);
                    vol.z = new ParticleSystem.MinMaxCurve(ventZ);
                }

                foreach (var ps in rainParticles)
                {
                    var vol = ps.velocityOverLifetime;
                    vol.enabled = true; 
                    vol.space = ParticleSystemSimulationSpace.World;
                    vol.x = new ParticleSystem.MinMaxCurve(ventX);
                    vol.z = new ParticleSystem.MinMaxCurve(ventZ);
                }
            }
            
            yield return null; 
        }
    }
}

[System.Serializable]
public class WeatherData
{
    public CurrentWeather current_weather;
}

[System.Serializable]
public class CurrentWeather
{
    public float temperature;
    public int weathercode;
    public float windspeed;
    public float winddirection;
    public int is_day;
}
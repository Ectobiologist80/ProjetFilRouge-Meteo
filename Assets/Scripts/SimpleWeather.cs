using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using TMPro;

public class SimpleWeather : MonoBehaviour
{
    public TextMeshProUGUI temperatureText;
    public GameObject sun;
    public GameObject snow;
    public GameObject clouds;
    public GameObject rain;
    public float windspeed; 
    public float winddirection; // 0 = North, 90 = East, 180 = South, 270 = West
    public Transform windsleeveTransform; // Adapter la rotation en fonction de la direction du vent
    public AudioSource windAudioSource; // Adapter le volume en fonction de la vitesse du vent

    void Start()
    {
        StartCoroutine(GetWeather());
    }

    IEnumerator GetWeather()
    {
        string url = "https://api.open-meteo.com/v1/forecast?latitude=48.25&longitude=-71.03&current_weather=true";
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            WeatherData data = JsonUtility.FromJson<WeatherData>(request.downloadHandler.text);

            float temp = data.current_weather.temperature;
            int code = data.current_weather.weathercode;
            float directionVent = data.current_weather.winddirection;
            
            //Rotation & Volume de la windsleeve en fonction de la direction et vitesse du vent
            windsleeveTransform.rotation = Quaternion.Euler(0, directionVent, 0);
            float vitesseVent = data.current_weather.windspeed;
            windAudioSource.volume = Mathf.Clamp01(vitesseVent / 40f);  

            // Affichage de la température sur Therometre
            temperatureText.text = temp + "°C";
            code = 75;
            //temperatureText.text = temp + "�C\nCode : " + code;

            if (sun != null) sun.SetActive(code == 0);
            if (clouds != null) clouds.SetActive((code >= 1 && code <= 3) || (code >= 51 && code <= 67) || (code >= 80 && code <= 82) || (code >= 71 && code <= 77));
            if (rain != null) rain.SetActive((code >= 51 && code <= 67) || (code >= 80 && code <= 82));
            if (snow != null) snow.SetActive(code >= 71 && code <= 77);
        }
        else
        {
            Debug.LogError(request.error);
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
}
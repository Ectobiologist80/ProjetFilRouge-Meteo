using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using TMPro;

public class SimpleWeather : MonoBehaviour
{
    public TextMeshProUGUI temperatureText;
    public GameObject sun;
    public GameObject snow;

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

            temperatureText.text = temp + "°C";

            //if (snow != null) snow.SetActive(code >= 71 && code <= 77);
            //if (sun != null) sun.SetActive(code == 0);
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
}
using UnityEngine;
using UnityEngine.Video;
using System.IO;
using UnityEngine.SceneManagement;

public class VideoLoader : MonoBehaviour
{
    [Header("Ustawienia")]
    [Tooltip("Wpisz tutaj nazwê pliku z rozszerzeniem, np. cutscenka1.mp4")]
    public string fileName;
    public string sceneName;
    float skipTime = 3;
    float currentSkipTime = 0;

    // Opcjonalnie: jeœli nie przypniesz VideoPlayera rêcznie, skrypt sam go znajdzie
    public VideoPlayer videoPlayer;

    void Start()
    {
        // 1. Jeœli zapomnia³eœ przypi¹æ VideoPlayera w Inspectorze, szukamy go na tym samym obiekcie
        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }

        // 2. Sprawdzamy czy wpisa³eœ nazwê pliku
        if (string.IsNullOrEmpty(fileName))
        {
            Debug.LogError("B³¹d: Nie wpisa³eœ nazwy pliku wideo w Inspectorze!");
            return;
        }

        // 3. Budowanie œcie¿ki do StreamingAssets
        string sciezka = Path.Combine(Application.streamingAssetsPath, fileName);

        // 4. Ustawienie i odpalenie
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = sciezka;

        videoPlayer.isLooping = false;

        // Dodatkowe zabezpieczenie: sprawdŸ czy plik istnieje (dzia³a tylko w Editorze/PC, na Androidzie czasem fa³szuje)
        if (!File.Exists(sciezka) && Application.platform != RuntimePlatform.Android)
        {
            Debug.LogError("Nie znaleziono pliku w StreamingAssets: " + sciezka);
        }

        videoPlayer.Prepare();
        // Play() zadzia³a automatycznie jeœli w VideoPlayerze zaznaczone jest "Play On Awake",
        // ale mo¿na to te¿ wywo³aæ rêcznie:
        videoPlayer.Play();
        videoPlayer.loopPointReached += EndOfVideo;
    }
    void EndOfVideo(VideoPlayer vp)
    {
        SceneManager.LoadScene(sceneName);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Return))
        {
            currentSkipTime += Time.deltaTime;
            Debug.Log(Time.deltaTime);
        }
        else
        {
            currentSkipTime = 0;
        }
        if (currentSkipTime >= skipTime)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
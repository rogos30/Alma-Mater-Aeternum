using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class CutsceneManager : MonoBehaviour
{
    public static CutsceneManager instance;
    public GameObject[] videoPlayer;
    [SerializeField] Slider skipSlider;
    [SerializeField] TMP_Text skipText;
    int cutsceneId;
    float currentSkipTime = 0, currentShowTime = 0;
    [SerializeField] float showTime = 3f;
    // Start is called before the first frame update

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        Time.timeScale = 1;
        cutsceneId = PlayerPrefs.GetInt("cutsceneId");
        videoPlayer[cutsceneId].SetActive(true);
        skipSlider.gameObject.SetActive(false);
        skipText.gameObject.SetActive(false);
        //videoPlayer.clip = cutscenes[0];
    }

    // Update is called once per frame
    void Update()
    {
        currentShowTime += Time.deltaTime;
        if (currentShowTime >= showTime)
        {
            skipText.gameObject.SetActive(false);
        }
        if (Input.anyKey)
        {
            skipText.gameObject.SetActive(true);
            currentShowTime = 0;
        }
        if (Input.GetKey(KeyCode.Return))
        {
            currentSkipTime += Time.deltaTime;
            skipSlider.gameObject.SetActive(true);
            Debug.Log(currentSkipTime);
        }
        else
        {
            currentSkipTime = 0;
            skipSlider.gameObject.SetActive(false);
        }
        skipSlider.value = currentSkipTime;
    }
}

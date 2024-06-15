using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public GameObject selectUI;
    public GameObject selectUI1;
    public GameObject selectUI2;

   // public AudioClip enter_audio;
    //public AudioClip enter_audio1;
    //public AudioClip enter_audio2;
    

    // Start is called before the first frame update
    void Start()
    {
        selectUI.SetActive(false);
        selectUI1.SetActive(false);
        selectUI2.SetActive(false);

        //enter_audio = GetComponent<AudioClip>();
       // enter_audio1 = GetComponent<AudioClip>();
       // enter_audio2 = GetComponent<AudioClip>();
    }

    // Update is called once per frame
    void Update()
    {
       // selectUI.SetActive(false);
    }
    public void ClickStartButton()
    {
        SceneManager.LoadScene(0);
    }
    public void ClickOptionButton()
    {

    }
    public void ClickExitButton()
    {
        Application.Quit();
    }
    public void SelectUIEvent()
    {
        selectUI.SetActive(true);
        selectUI1.SetActive(false);
        selectUI2.SetActive(false);
       // AudioSource.PlayClipAtPoint(enter_audio, Camera.main.transform.position);
        
    }
    public void UnSelectUIEvent()
    {
        selectUI.SetActive(false);
    }
    public void SelectUI1Event()
    {
        selectUI.SetActive(false);
        selectUI1.SetActive(true);
        selectUI2.SetActive(false);
       // AudioSource.PlayClipAtPoint(enter_audio1, Camera.main.transform.position);
    }
    public void UnSelectUI1Event()
    {
        selectUI1.SetActive(false);
    }
    public void SelectUI2Event()
    {
        selectUI.SetActive(false);
        selectUI1.SetActive(false);
        selectUI2.SetActive(true);
       // AudioSource.PlayClipAtPoint(enter_audio2, Camera.main.transform.position);
    }
    public void UnSelectUI2Event()
    {
        selectUI2.SetActive(false);
    }
}

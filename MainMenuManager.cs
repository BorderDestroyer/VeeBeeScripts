using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField]
    GameObject deleteDataParent;

    [SerializeField]
    GameObject trigger1Obj;
    SpriteRenderer trigger1Sr;
    AudioSource trigger1Ass;
    [SerializeField]
    GameObject trigger2Obj;
    SpriteRenderer trigger2Sr;
    AudioSource trigger2Ass;
    [SerializeField]
    GameObject tutorial;

    [SerializeField]
    Image crossFade;

    [SerializeField]
    Color triggerOneFirst;
    [SerializeField]
    Color triggerOneSecond;
    [SerializeField]
    Color triggerTwoFirst;
    [SerializeField]
    Color triggerTwoSecond;

    AudioSource ass;
    bool fadeMusic = false;
    void Start()
    { 
        trigger1Sr = trigger1Obj.GetComponent<SpriteRenderer>();
        trigger2Sr = trigger2Obj.GetComponent<SpriteRenderer>();
        trigger1Ass = trigger1Obj.GetComponent<AudioSource>();
        trigger2Ass = trigger2Obj.GetComponent<AudioSource>();
        ass = GetComponent<AudioSource>();

        deleteDataParent.SetActive(false);
        tutorial.SetActive(false);

        crossFade.CrossFadeAlpha(0, 1, true);
        Invoke("ChangeRaycast", .75f);
    }

    void Update()
    {
        if (fadeMusic)
        {
            ass.volume -= .01f;
        }
        else if(!fadeMusic && ass.volume <=.6f)
        {
            ass.volume += .01f;
        }

        if(Input.GetKeyDown(KeyCode.V))
        {
            trigger1Sr.color = triggerOneSecond;
            StartCoroutine(resetColor(trigger1Sr, triggerOneFirst));
            trigger1Ass.Play();
        }
        else if(Input.GetKeyDown(KeyCode.B))
        {
            trigger2Sr.color = triggerTwoSecond;
            trigger2Ass.Play();
            StartCoroutine(resetColor(trigger2Sr, triggerTwoFirst));
        }

        if(Input.GetKeyDown(KeyCode.V) && tutorial.activeSelf == true)
        {
            tutorial.SetActive(false);
        }
        else if (Input.GetKeyDown(KeyCode.B) && tutorial.activeSelf == true)
        {
            tutorial.SetActive(false);
        }
    }

    IEnumerator resetColor(SpriteRenderer sr, Color color)
    {
        yield return new WaitForSeconds(.1f);
        sr.color = color;
        StopCoroutine(resetColor(sr, color));
    }

    public void Play()
    {
        fadeMusic = true;
        crossFade.raycastTarget = true;
        crossFade.CrossFadeAlpha(2, 1, true);
        StartCoroutine(loadScene(1));
    }

    public void Tutorial()
    {
        if(tutorial.activeSelf == false)
        {
            tutorial.SetActive(true);
        }
        else
        {
            tutorial.SetActive(false);
        }
    }

    public void TrashInit(float Val)
    {
        if(Val == 0)
        {
            deleteDataParent.SetActive(true);
        }
        else if (Val == 1)
        {
            PlayerPrefs.SetInt("HighScore", 0);
            deleteDataParent.SetActive(false);
        }
        else if(Val == 2)
        {
            deleteDataParent.SetActive(false);
        }
    }

    void ChangeRaycast()
    {
        if(crossFade.raycastTarget == true)
        {
            crossFade.raycastTarget = false;
        }
        else
        {
            crossFade.raycastTarget = true;
        }
    }

    IEnumerator loadScene(int ScenetoLoad)
    {
        yield return new WaitForSeconds(1.25f);
        SceneManager.LoadScene(ScenetoLoad);
    }

    public void Socials(int Val)
    {
        switch(Val)
        {
            case 0:
                Application.OpenURL("https://www.youtube.com/channel/UCKeSimtpEbtM1ZsA-Dqs6zg");
                break;
            case 1:
                Application.OpenURL("https://twitter.com/border_game");
                break;
            case 2:
                Application.OpenURL("https://www.instagram.com/borderdestroyer/");
                break;
            case 3:
                Application.OpenURL("https://www.tiktok.com/@borderdestroyer");
                break;
            case 4:
                Application.OpenURL("https://discord.gg/dcsdq3xE");
                break;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    float transitionSpeed;
    float contrastTimer;
    float interTimerB;
    float interTimerV;

    [SerializeField]
    int score;
    [SerializeField]
    int ScoreRef;
    [SerializeField]
    GameObject scoreTextObj;
    [SerializeField]
    Text scoreText;
    [SerializeField]
    GameObject scoreText2Obj;
    [SerializeField]
    Text scoreText2;

    GameObject[] points;

    [SerializeField]
    GameObject firstPoint;
    [SerializeField]
    GameObject FirstGameOne;
    [SerializeField]
    GameObject SecondGameOne;
    [SerializeField]
    GameObject FirstGameTwo;
    [SerializeField]
    GameObject SecondGameTwo;
    [SerializeField]
    GameObject dedMenu;
    [SerializeField]
    GameObject background;

    [SerializeField]
    Image crossFade;

    [SerializeField]
    Vector3[] FirstGameOnePoints;
    [SerializeField]
    Vector3[] FirstGameTwoPoints;
    [SerializeField]
    Vector3 ScaledDown;
    Vector3 backgroundMove;

    [SerializeField]
    Color firstUnpressColor;
    [SerializeField]
    Color firstPressColor;
    [SerializeField]
    Color SecondUnpressColor;
    [SerializeField]
    Color SecondPressColor;

    [SerializeField]
    GameObject[] firstTriggers;
    [SerializeField]
    GameObject[] secondTriggers;
    AudioSource firstTriggerAss;
    [SerializeField]
    AudioSource secondTriggerAss;

    [SerializeField]
    GameObject gameMusicObj;
    AudioSource gameMusic;
    [SerializeField]
    AudioClip[] music;

    PostProcessVolume ppv;
    ColorGrading CG;
    LensDistortion LD;
    ChromaticAberration CA;

    [SerializeField]
    bool transition;
    int transitionCount;
    [SerializeField]
    bool testing;
    private void Start()
    {
        ppv = GameObject.FindGameObjectWithTag("MainCamera").GetComponentInChildren<PostProcessVolume>();
        ppv.profile.TryGetSettings(out CG);
        ppv.profile.TryGetSettings(out LD);
        ppv.profile.TryGetSettings(out CA);
        backgroundMove = new Vector3(.01f, 0, 0);
        StartCoroutine(changeBackgroundMovement());
        gameMusic = gameMusicObj.GetComponent<AudioSource>();
        firstTriggerAss = firstTriggers[0].GetComponent<AudioSource>();

        score = 0;
        ScoreRef = 10;
        gameMusic.volume = 0;

        dedMenu.SetActive(false);

        points = GameObject.FindGameObjectsWithTag("Point");
        for (int i = 0; i < points.Length; i++)
        {
            BlockMove script = points[i].GetComponent<BlockMove>();
            script.canMove = false;
        }

        crossFade.CrossFadeAlpha(0, 1, true);
        Invoke("MakeMove", 1.5f);
        StartCoroutine(changeVolume(gameMusic, .5f));
    }

    private void Update()
    {
        interTimerV += Time.deltaTime;
        interTimerB += Time.deltaTime;
        
        if(score >= ScoreRef && !transition)
        {
            Debug.Log("Score >= ScoreREF");
            points = GameObject.FindGameObjectsWithTag("Point");
            transition = true;
            for (int i = 0; i < points.Length; i++)
            {
                BlockMove script = points[i].GetComponent<BlockMove>();
                script.canMove = false;
            }
        }

        background.transform.position = background.transform.position + backgroundMove;

        switch(transitionCount)
        {
            case 0:
                if (Vector2.Distance(FirstGameOne.transform.position, FirstGameOnePoints[0]) < .05f)
                {
                    ScoreRef = 50;
                    transitionCount += 1;
                    transition = true;
                }
                break;
            case 1:
                if (Vector2.Distance(FirstGameTwo.transform.position, FirstGameTwoPoints[0]) < .05f)
                {
                    transitionCount += 1;
                    transition = false;
                    Invoke("StartGame", 2);
                }
                break;
            case 2:
                if(Vector2.Distance(SecondGameOne.transform.position, FirstGameOnePoints[1]) < .05f)
                {
                    ScoreRef = 100;
                    transitionCount += 1;
                    transition = false;
                    Invoke("StartGame", 2);

                }
                break;
            case 3:
                if(CG.saturation.value <= -100)
                {
                    print("trigger");
                    ScoreRef = 200;
                    transitionCount += 1;
                    transition = false;
                    Invoke("StartGame", 2);
                }
                break;
            case 4:
                if(CG.contrast.value >= 100 && CG.brightness.value <= -76)
                {
                    ScoreRef = 300;
                    transitionCount += 1;
                    transition = false;
                    Invoke("StartGame", 2);
                }
                break;
            case 5:
                if(contrastTimer >= .9f)
                {
                    contrastTimer = 0;
                    CG.brightness.value = 0;
                    CG.saturation.value = 0;
                    CG.contrast.value = 0;
                    ScoreRef = 350;
                    transitionCount += 1;
                    transition = false;
                    Invoke("StartGame", 2);
                }
                break;
            case 6:
                if(contrastTimer >= .9f)
                {
                    contrastTimer = 0;
                    LD.intensity.value = -73;
                    ScoreRef = 500;
                    transitionCount += 1;
                    transition = false;
                    Invoke("StartGame", 2);
                }
                break;
            case 7:
                if(contrastTimer >= .9f)
                {
                    contrastTimer = 0;
                    LD.intensity.value = 28.8f;
                    ScoreRef = 600;
                    transitionCount += 1;
                    transition = false;
                    Invoke("StartGame", 2);
                }
                break;
            case 8:
                if(CA.intensity.value >= .70f)
                { 
                    CA.intensity.value = .70f;
                    ScoreRef = 700;
                    transitionCount += 1;
                    transition = false;
                    Invoke("StartGame", 2);
                }
                break;
            case 9:
                if(CA.intensity.value <=0)
                {
                    CA.intensity.value = 0;
                    ScoreRef = 1000000000;
                    transitionCount += 1;
                    transition = false;
                    Invoke("StartGame", 2);
                }
                break;
        }

        if(Input.GetKeyDown(KeyCode.V) && interTimerV >= .2f)
        {
            interTimerV = 0;
            firstTriggerAss.Play();
            for (int i = 0; i < firstTriggers.Length; i++)
            {
                if(firstTriggers[i].activeSelf)
                {
                    SpriteRenderer sr = firstTriggers[i].GetComponent<SpriteRenderer>();
                    sr.color = firstPressColor;
                    StartCoroutine(resetTriggers(sr, firstUnpressColor));
                }
            }

            points = GameObject.FindGameObjectsWithTag("Point");
            for (int i = 0; i < points.Length; i++)
            {
                BlockMove script = points[i].GetComponent<BlockMove>();
            }
        }
        if(Input.GetKeyDown(KeyCode.B) &&interTimerB >= .2f)
        {
            interTimerB = 0;
            secondTriggerAss.Play();
            for (int i = 0; i < secondTriggers.Length; i++)
            {
                if (secondTriggers[i].activeSelf)
                {
                    SpriteRenderer sr = secondTriggers[i].GetComponent<SpriteRenderer>();
                    sr.color = SecondPressColor;
                    StartCoroutine(resetTriggers(sr, SecondUnpressColor));
                }
            }

            points = GameObject.FindGameObjectsWithTag("Point");
            for (int i = 0; i < points.Length; i++)
            {
                BlockMove script = points[i].GetComponent<BlockMove>();
            }
        }
    }
    
    void MakeMove()
    {
        points = GameObject.FindGameObjectsWithTag("Point");
        for (int i = 0; i < points.Length; i++)
        {
            BlockMove script = points[i].GetComponent<BlockMove>();
            script.canMove = true;
        }
    }

    IEnumerator resetTriggers(SpriteRenderer trigger, Color color)
    {
        yield return new WaitForSeconds(.1f);
        trigger.color = color;
        StopCoroutine(resetTriggers(trigger, color));
    }

    IEnumerator changeBackgroundMovement()
    {
        while(true)
        {
            yield return new WaitForSecondsRealtime(10);
            backgroundMove = new Vector3(0, .01f, 0);
            yield return new WaitForSecondsRealtime(10);
            backgroundMove = new Vector3(-.01f, 0, 0);
            yield return new WaitForSecondsRealtime(10);
            backgroundMove = new Vector3(0, -.01f, 0);
            yield return new WaitForSecondsRealtime(10);
            backgroundMove = new Vector3(.01f, 0, 0);
        }
    }

    public void HitDeathplane()
    {
        if(!transition && !testing)
        {
            Debug.Log("ded");
            points = GameObject.FindGameObjectsWithTag("Point");
            for (int i = 0; i < points.Length; i++)
            {
                BlockMove script = points[i].GetComponent<BlockMove>();
                script.canMove = false;
            }
            scoreTextObj.SetActive(false);
            dedMenu.SetActive(true);

            int highScore = PlayerPrefs.GetInt("HighScore");
            if(score > highScore)
            {
                PlayerPrefs.SetInt("HighScore", score);
                scoreText2.text = "NEW HIGH SCORE: " + score.ToString();
                StartCoroutine(flashing(scoreText2Obj, 1));
            }
            else
            {
                scoreText2.text = "SCORE: " + score.ToString();
            }
        }
    }

    IEnumerator changeVolume(AudioSource ass, float volume)
    {
        while(ass.volume != volume)
        {
            if(ass.volume < volume)
            {
                ass.volume = Mathf.MoveTowards(ass.volume, volume, .01f);
            }
            else if(ass.volume > volume)
            {
                ass.volume = Mathf.MoveTowards(ass.volume, volume, .003f);
            }
            yield return new WaitForEndOfFrame();
        }

        StopCoroutine(changeVolume(ass, volume));
    }

    public void Retry()
    {
        crossFade.CrossFadeAlpha(2, 1, true);
        crossFade.raycastTarget = true;
        StartCoroutine(changeVolume(gameMusic, 0));
        StartCoroutine(LoadScene(SceneManager.GetActiveScene().buildIndex));
    }

    public void MainMenu()
    {
        crossFade.CrossFadeAlpha(2, 1, true);
        crossFade.raycastTarget = true;
        StartCoroutine(changeVolume(gameMusic, 0));
        StartCoroutine(LoadScene(0));
    }

    public void QuitGame()
    {
        crossFade.CrossFadeAlpha(2, 1, true);
        crossFade.raycastTarget = true;
        StartCoroutine(changeVolume(gameMusic, 0));
        StartCoroutine(LoadScene(-1));
    }

    IEnumerator LoadScene(int toLoad)
    {
        yield return new WaitForSeconds(1.25f);
        if(toLoad < 0)
        {
            Application.Quit();
        }
        else
        {
            SceneManager.LoadScene(toLoad);
        }
    }

    private void FixedUpdate()
    {
        if (transition)
        {
            switch (transitionCount)
            {
                case 0:
                    gameMusic.Stop();
                    FirstGameOne.transform.localScale = Vector2.MoveTowards(FirstGameOne.transform.localScale, ScaledDown, transitionSpeed/8.5f);
                    FirstGameOne.transform.position = Vector2.MoveTowards(FirstGameOne.transform.position, FirstGameOnePoints[0], transitionSpeed);
                    firstPoint.transform.position = firstPoint.transform.position + new Vector3(.0015f, 0, 0);
                    break;
                case 1:
                    gameMusic.Stop();
                    gameMusic.clip = music[transitionCount];
                    FirstGameTwo.transform.position = Vector2.MoveTowards(FirstGameTwo.transform.position, FirstGameTwoPoints[0], transitionSpeed);
                    FirstGameTwo.SetActive(true);
                    break;
                case 2:
                    gameMusic.Stop();
                    SecondGameOne.SetActive(true);
                    SecondGameTwo.SetActive(true);
                    gameMusic.clip = music[transitionCount];
                    SecondGameOne.transform.position = Vector2.MoveTowards(SecondGameOne.transform.position, FirstGameOnePoints[1], transitionSpeed);
                    SecondGameTwo.transform.position = Vector2.MoveTowards(SecondGameTwo.transform.position, FirstGameTwoPoints[1], transitionSpeed);
                    break;
                case 3:
                    gameMusic.Stop();
                    CG.saturation.value -= 2;
                    break;
                case 4:
                    gameMusic.Stop();
                    CG.brightness.value -= 1;
                    CG.contrast.value += 2;
                    break;
                case 5:
                    contrastTimer += Time.deltaTime;
                    gameMusic.Stop();
                    CG.brightness.value += 1;
                    CG.contrast.value -= 1;
                    CG.saturation.value += 1;
                    break;
                case 6:
                    gameMusic.Stop();
                    LD.intensity.value -= 2.2f;
                    contrastTimer += Time.deltaTime;
                    break;
                case 7:
                    gameMusic.Stop();
                    LD.intensity.value += 2.2f;
                    contrastTimer += Time.deltaTime;
                    break;
                case 8:
                    gameMusic.Stop();
                    CA.intensity.value += .1f;
                    break;
                case 9:
                    gameMusic.Stop();
                    CA.intensity.value -= .1f;
                    break;
            }
        }
    }

    IEnumerator flashing(GameObject toFlash, float speed)
    {
        while(true)
        {
            toFlash.SetActive(false);
            yield return new WaitForSeconds(speed);
            toFlash.SetActive(true);
            yield return new WaitForSeconds(speed);
        }
    }

    void StartGame()
    {
        Debug.Log("Started");
        gameMusic.Play();
        points = GameObject.FindGameObjectsWithTag("Point");
        for (int i = 0; i < points.Length; i++)
        {
            BlockMove script = points[i].GetComponent<BlockMove>();
            script.canMove = true;
            if(script.down)
            {
                script.canDie = false;
            }
            else
            {
                if(transitionCount == 2)
                {
                    script.multiplier = .85f;
                }
                else
                {
                    script.multiplier = .5f;
                }

            }
        }
    }

    public void updateScore()
    {
        score += 1;
        scoreText.text = "Score: " + score.ToString();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockMove : MonoBehaviour
{
    [SerializeField]
    float _speed;
    float interTimerV;
    float interTimerB;
    public float multiplier;

    float downTimer;
    [SerializeField]
    float downTimerREF;

    [SerializeField]
    int ID;

    Rigidbody2D rb;

    [SerializeField]
    GameObject particles;
    ParticleSystem ps;

    AudioSource pointDestroy;

    [SerializeField]
    Vector2 movement;
    [SerializeField]
    GameObject startPoint;

    GameManager gm;

    public bool canInteract;
    public bool canMove;
    [SerializeField]
    public bool down;
    public bool canDie;
    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        rb = GetComponent<Rigidbody2D>();
        ps = particles.GetComponent<ParticleSystem>();
        pointDestroy = GetComponent<AudioSource>();


        transform.position = startPoint.transform.position;

        if(!down)
        {
            canDie = true;
        }
        else
        {
            canDie = false;
        }
    }

    void Update()
    {
        interTimerB += Time.deltaTime;
        interTimerV += Time.deltaTime;
        if(canInteract && Input.GetKeyDown(KeyCode.V) && !down && canMove && interTimerV >= .2f)
        {
            interTimerV = 0;
            particles.transform.position = transform.position;
            transform.position = startPoint.transform.position;
            ps.Play();
            pointDestroy.Play();
            _speed = Random.Range(.5f, 1.5f);
            gm.updateScore();
        }
        else if(canInteract && Input.GetKeyDown(KeyCode.B) && down && canMove && interTimerB >= .2f)
        {
            interTimerB = 0;
            canDie = false;
            Invoke("resetDeath", 1);
            particles.transform.position = transform.position;
            transform.position = startPoint.transform.position;
            ps.Play();
            pointDestroy.Play();
            _speed = Random.Range(.4f, .5f);
            gm.updateScore();
        }

        if (particles.activeSelf == false)
        {
            particles.SetActive(true);
        }

        if(down && !canDie)
        {
            downTimer += Time.deltaTime;
            if(downTimer >= downTimerREF)
            {
                downTimer = 0;
                canDie = true;
            }
        }
    }

    private void FixedUpdate()
    {
        if (canMove)
        {
            rb.MovePosition(rb.position + movement * _speed * multiplier);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("BlockCheck"))
        {
            canInteract = true;
        }
        else if(collision.CompareTag("Deathplane") && !down)
        {
            gm.HitDeathplane();
        }

        if(down && ID.ToString() == collision.name && collision.CompareTag("BlockCheck"))
        {
            canInteract = true;
        }
    }
    
    void resetDeath()
    {
        canDie = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("BlockCheck"))
        {
            if(down && Vector2.Distance(transform.position, startPoint.transform.position) >= .5f && canDie && ID.ToString() == collision.name)
            {
                gm.HitDeathplane();
            }
            canInteract = false;
        }
    }
}

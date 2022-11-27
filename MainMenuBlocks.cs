using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuBlocks : MonoBehaviour
{
    [SerializeField]
    float _speed;

    [SerializeField]
    bool down;

    [SerializeField]
    GameObject particles;
    ParticleSystem ps;

    [SerializeField]
    Vector2 movement;

    Rigidbody2D rb;

    AudioSource ass;

    [SerializeField]
    Vector2 firstStartMin;
    [SerializeField]
    Vector2 firstStartMax;
    [SerializeField]
    Vector2 secondStartMin;
    [SerializeField]
    Vector2 secondStartMax;

    bool canInteract;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ps = particles.GetComponent<ParticleSystem>();
        ass = GetComponent<AudioSource>();
    }

    void UpdatePos()
    {
        if(!down)
        {
            transform.position = new Vector2(firstStartMin.x, Random.Range(firstStartMin.y, firstStartMax.y));
        }
        else
        {
            transform.position = new Vector2(Random.Range(secondStartMin.x, secondStartMax.x), secondStartMin.y);
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.V) && !down && canInteract)
        {
            particles.transform.position = transform.position;
            ps.Play();
            ass.Play();
            UpdatePos();
            _speed = Random.Range(.5f, 1.5f);
        }

        if (Input.GetKeyDown(KeyCode.B) && down && canInteract)
        {
            particles.transform.position = transform.position;
            ps.Play();
            ass.Play();
            UpdatePos();
            _speed = Random.Range(.4f, .5f);
        }

        if(particles.activeSelf == false)
        {
            particles.SetActive(true);
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * _speed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("BlockCheck"))
        {
            if (!down)
            {
                if(collision.name == "1")
                {
                    canInteract = true;
                }
            }
            else
            {
                if(collision.name == "2")
                {
                    canInteract = true;
                }
            }
        }
        else if (collision.CompareTag("Deathplane"))
        {
            UpdatePos();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("BlockCheck"))
        {
            canInteract = false;
        }
    }
}

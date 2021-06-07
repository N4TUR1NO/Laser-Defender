using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float padding = 0.5f;
    [SerializeField] int health = 200;
    [SerializeField] AudioClip deathSFX;
    [SerializeField] [Range(0f, 1f)] float deathSFXVolume = 0.75f;
    [SerializeField] AudioClip shootSFX;
    [SerializeField] [Range(0f, 1f)] float shootSFXVolume = 0.25f;

    [Header("Projectile")]
    [SerializeField] GameObject laserPrefab;
    [SerializeField] float laserSpeed = 20f;
    [SerializeField] float laserFiringPeriod = 0.1f;

    Coroutine firingCoroutine;

    float xMin, xMax, yMin, yMax;
    void Start()
    {
        SetUpMoveBounaries();
    }

    void Update()
    {
        Move();
        Fire();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        DamageDealer damageDealer = other.gameObject.GetComponent<DamageDealer>();
        if (!damageDealer)
            return;
        health -= damageDealer.GetDamage();
        damageDealer.Hit();
        if (health <= 0)
        {
            Destroy(gameObject);
            AudioSource.PlayClipAtPoint(deathSFX, Camera.main.transform.position, deathSFXVolume);
            FindObjectOfType<Level>().LoadGameOver();
        }
    }
    private void Fire()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            firingCoroutine = StartCoroutine(FireContinuusly());
        }
        if (Input.GetButtonUp("Fire1"))
        {
            StopCoroutine(firingCoroutine);
        }
    }
    IEnumerator FireContinuusly()
    {
        while (true)
        {
            GameObject laser = Instantiate(laserPrefab, transform.position, Quaternion.identity) as GameObject;
            laser.GetComponent<Rigidbody2D>().velocity = new Vector2(0, laserSpeed);
            AudioSource.PlayClipAtPoint(shootSFX, Camera.main.transform.position, shootSFXVolume);
            yield return new WaitForSeconds(laserFiringPeriod);
        }
    }
    private void Move()
    {   
        var deltaX = Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed;
        var deltaY = Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;
        
        var newXPos = Mathf.Clamp(transform.position.x + deltaX, xMin, xMax);
        var newYPos = Mathf.Clamp(transform.position.y + deltaY, yMin, yMax);
        transform.position = new Vector2(newXPos, newYPos);
    }
    private void SetUpMoveBounaries()
    {
        Camera gameCamera = Camera.main;
        xMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + padding;
        xMax = gameCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - padding;

        yMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y + padding;
        yMax = gameCamera.ViewportToWorldPoint(new Vector3(0, 1, 0)).y - padding;
    }

    public int GetHealth()
    {
        return health;
    }
}

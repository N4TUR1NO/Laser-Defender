using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float health = 100;
    [SerializeField] int scoreValue = 50;
    [SerializeField] float shotCounter;
    [SerializeField] float minTimesBetweenShots = 0.1f;
    [SerializeField] float maxTimesBetweenShots = 1f;
    [SerializeField] GameObject laserPrefab;
    [SerializeField] float laserSpeed = 0.2f;
    [SerializeField] GameObject deathVFX;
    [SerializeField] AudioClip deathSFX;
    [SerializeField] [Range(0f, 1f)] float deathSFXVolume;
    [SerializeField] AudioClip shootSFX;
    [SerializeField] [Range(0f, 1f)] float shootSFXVolume = 0.25f;

    void Start()
    {
        shotCounter = UnityEngine.Random.Range(minTimesBetweenShots, maxTimesBetweenShots);
    }

    void Update()
    {
        CountDownAndShoot();
    }

    private void CountDownAndShoot()
    {
        shotCounter -= Time.deltaTime;
        if (shotCounter <= 0f)
        {
            Fire();
            shotCounter = UnityEngine.Random.Range(minTimesBetweenShots, maxTimesBetweenShots);
        }
    }

    private void Fire()
    {
        GameObject laser = Instantiate(laserPrefab, transform.position, Quaternion.identity) as GameObject;
        laser.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -laserSpeed);
        AudioSource.PlayClipAtPoint(shootSFX, Camera.main.transform.position, shootSFXVolume);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        DamageDealer damageDealer = other.gameObject.GetComponent<DamageDealer>();
        if (!damageDealer)
            return;
        ProcessHit(damageDealer);
    }

    private void ProcessHit(DamageDealer damageDealer)
    {
        health -= damageDealer.GetDamage();
        damageDealer.Hit();
        if (health <= 0)
        {
            FindObjectOfType<GameSession>().AddToScore(scoreValue);
            Destroy(gameObject);
            GameObject particles = Instantiate(deathVFX, transform.position, Quaternion.identity) as GameObject;
            Destroy(particles, 1f);
            AudioSource.PlayClipAtPoint(deathSFX, Camera.main.transform.position, deathSFXVolume);
        }
    }
}

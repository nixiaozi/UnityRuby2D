﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubyController : MonoBehaviour
{
    public float speed = 3.0f;

    public int maxHealth = 5;
    public int Health 
    { 
        get
        {
            return currentHealth;
        } 
    }

    int currentHealth;

    public float timeInvincible = 2.0f;
    bool isInvincible;
    float invincibleTimer;


    Rigidbody2D rigidbody2d;

    Animator animator;
    Vector2 lookDirection = new Vector2(1, 0);

    public ParticleSystem HitEffect;
    public ParticleSystem ResumEffect;

    /// <summary>
    /// 这个是一个可修改的外置对象
    /// </summary>
    public GameObject projectilePrefab;

    AudioSource audioSource;

    /// <summary>
    /// 发射子弹 和 受伤的音效
    /// </summary>
    public AudioClip lanuchAudioClip;
    public AudioClip hitedAudioClip;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        /*QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 10;*/
        rigidbody2d = GetComponent<Rigidbody2D>();

        currentHealth = maxHealth;

        audioSource= GetComponent<AudioSource>();

        HitEffect.Stop();
        ResumEffect.Stop();
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector2 move = new Vector2(horizontal, vertical);

        if(!Mathf.Approximately(move.x,0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }

        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        //Vector2 position = transform.position; 获取物理位置
        Vector2 position = rigidbody2d.position; // 获取刚体位置
        /*position.x += speed * horizontal * Time.deltaTime;
        position.y += speed * vertical * Time.deltaTime;*/
        position = position + move * speed * Time.deltaTime; // 等效以上注释代码
        //transform.position = position; 改变物理位置

        rigidbody2d.MovePosition(position); // 改变刚体位置

        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if(invincibleTimer < 0)
            {
                isInvincible = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            Launch();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, 
                lookDirection, 1.5f, LayerMask.GetMask("NPC")); //选择了检测层Mask
            if (hit.collider != null)
            {
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                {
                    character.DisplayDialog();
                }
            }
        }

    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible)
                return;

            isInvincible = true;
            invincibleTimer = timeInvincible;

        }
        animator.SetTrigger("Hit"); // 受伤的动画
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        Debug.Log(currentHealth + "/" +maxHealth);
        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
        if (amount < 0)
        {
            HitEffect.Play();
            this.PlaySound(hitedAudioClip);
        }
        else if (amount > 0)
        {
            ResumEffect.Play();
        }
    }

    void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, 
            rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);

        animator.SetTrigger("Launch");
        this.PlaySound(lanuchAudioClip);
    }

}

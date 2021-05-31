using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    // ============================================================
    // Enums
    // ============================================================

    public enum State
    {
        Handgun,
        Shotgun,
    }

    public enum Animation
    {
        Idle,
        Meleeatack,
        Move,
        Reload,
        Run,
        Shoot,
    }

    private State _currentState;

    public State CurrentState
    {
        get { return _currentState; }
        set
        {
            _currentState = value;

            switch (CurrentState)
            {
                case State.Handgun:
                    break;
                case State.Shotgun:
                    break;
            }

            UpdateAnimation();
        }
    }

    private Animation _currentAnimation;

    public Animation CurrentAnimation
    {
        get { return _currentAnimation; }
        set
        {
            _currentAnimation = value;
            UpdateAnimation();
        }
    }

    public string AnimationName
    {
        get
        {
            var action = CurrentAnimation.ToString();
            var weapon = CurrentState.ToString();

            return "Player_" + action + "_" + weapon;
        }
    }

    // ============================================================
    // Variables
    // ============================================================

    AudioSource audioSource;

    public Transform handgunFirePoint;
    public Transform shotgunFirePoint;
    public int health;

    [SerializeField] private float moveSpeed;
    [SerializeField] private int stamina;

    private bool iFrameOn = false;
    private float iFrameTimer = 0;
    private float iFrame = 2;
    private float staminaTimer = 0;

    private Animator Animator;
    private Rigidbody2D Rigidbody2D;
    private Vector2 movement;

    private int handgunAmmo = 7;
    private int handgunAmmoMax = 7;
    private int shotgunAmmo = 2;
    private int shotgunAmmoMax = 2;
    private int ammo;
    private int ammoMax;

    // ============================================================
    // Status
    // ============================================================

    public Text Life;
    public Text Ammo;
    public Text Stamina;

    // ============================================================
    // Awake, Start, Update & FixedUpdate
    // ============================================================

    private void Awake()
    {
        Animator = GetComponent<Animator>();
        Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        CurrentState = State.Handgun;
        CurrentAnimation = Animation.Idle;
        iFrameTimer = iFrame;
        ammo = handgunAmmo;
        ammoMax = handgunAmmoMax;
    }

    private void Update()
    {
        // Life

        Life.text = "Life : " + health.ToString();
        Stamina.text = "Stamina : " + stamina.ToString();
        Ammo.text = "Ammo : " + ammo.ToString() + "/" + ammoMax.ToString();

        // iFrame

        if (iFrameOn)
        {
            iFrameTimer -= Time.deltaTime;
            if (iFrameTimer < 0)
            {
                iFrameTimer = iFrame;
                iFrameOn = false;
            }
        }

        // XY movement

        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Weapon changing

        if (Input.GetKeyDown(KeyCode.Alpha1)) // 1
        {
            CurrentState = State.Handgun;
            ammo = handgunAmmo;
            ammoMax = handgunAmmoMax;
            GameManager.Instance.SoundManager.Play(SoundManager.Sfx.Switch);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) // 2
        {
            CurrentState = State.Shotgun;
            ammo = shotgunAmmo;
            ammoMax = shotgunAmmoMax;
            GameManager.Instance.SoundManager.Play(SoundManager.Sfx.Switch);
        }

        // Reloading

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (CurrentState == State.Handgun)
            {
                if (handgunAmmo != handgunAmmoMax)
                {
                    ammoMax = handgunAmmoMax;
                    handgunAmmo = handgunAmmoMax;
                    CurrentAnimation = Animation.Reload; // Reload animation
                    GameManager.Instance.SoundManager.Play(SoundManager.Sfx.Handgun_Reload);
                    ammo = handgunAmmo;
                }
            }
            else if (CurrentState == State.Shotgun)
            {
                if (shotgunAmmo != shotgunAmmoMax)
                {
                    shotgunAmmo = shotgunAmmoMax;
                    CurrentAnimation = Animation.Reload; // Reload animation
                    GameManager.Instance.SoundManager.Play(SoundManager.Sfx.Shotgun_Reload);
                    ammo = shotgunAmmo;
                }
            }
        }

        // Shooting

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (CurrentState == State.Handgun)
            {
                if (handgunAmmo >= 1)
                {
                    CurrentAnimation = Animation.Shoot;
                    handgunAmmo--;
                    GameManager.Instance.PrefabManager.Spawn(PrefabManager.Global.bulletTest, handgunFirePoint.position, transform.rotation); // Bullet
                    GameManager.Instance.SoundManager.Play(SoundManager.Sfx.Handgun_Shoot); // Handgun SFX
                    ammo = handgunAmmo;
                    ammoMax = handgunAmmoMax;
                }
                else
                {
                    CurrentAnimation = Animation.Shoot;
                    GameManager.Instance.SoundManager.Play(SoundManager.Sfx.Handgun_Empty); // Empty gun sound
                }
            }
            else if (CurrentState == State.Shotgun)
            {
                if (shotgunAmmo >= 1)
                {
                    CurrentAnimation = Animation.Shoot;
                    shotgunAmmo--;
                    GameManager.Instance.PrefabManager.Spawn(PrefabManager.Global.bulletTest, shotgunFirePoint.position, transform.rotation); // Bullet
                    GameManager.Instance.SoundManager.Play(SoundManager.Sfx.Shotgun_Shoot); // Handgun SFX
                    ammo = shotgunAmmo;
                    ammoMax = shotgunAmmoMax;

                }
                else
                {
                    CurrentAnimation = Animation.Shoot;
                    GameManager.Instance.SoundManager.Play(SoundManager.Sfx.Shotgun_Empty); // Empty gun sound
                }
            }
        }

        // Logic manager

        if (CurrentAnimation != Animation.Reload && CurrentAnimation != Animation.Shoot)
        {
            MoveOrIdle();
        }
        else
        {
            if (CurrentAnimation == Animation.Reload &&
                Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && 
                !Animator.IsInTransition(0))
            {
                MoveOrIdle();
            }
            else if (CurrentAnimation == Animation.Shoot &&
                     Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && 
                     !Animator.IsInTransition(0))
            {
                MoveOrIdle();
            }
        }
    }

    private void FixedUpdate()
    {
        Rigidbody2D.MovePosition(Rigidbody2D.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    // ============================================================
    // Functions
    // ============================================================

    private void StaminaRegen()
    {
        if (stamina < 5)
        {
            staminaTimer += Time.deltaTime;
            if (staminaTimer >= 2)
            {
                stamina++;
                staminaTimer = 0;
            }
        }
    }

    private void UpdateAnimation()
    {
        var animationName = AnimationName;
        Animator.Play(animationName);
    }

    private void MoveOrIdle()
    {
        if (((movement.x != 0f) || (movement.y != 0f)) && !Input.GetKey(KeyCode.LeftShift))
        {
            CurrentAnimation = Animation.Move; // Move
            moveSpeed = 2f;

            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }

            StaminaRegen();
        }
        else if (((movement.x != 0f) || (movement.y != 0f)) && Input.GetKey(KeyCode.LeftShift))
        {
            if (stamina >= 1)
            {
                CurrentAnimation = Animation.Run; // Run
                moveSpeed = 4f;

                if (!audioSource.isPlaying)
                {
                    audioSource.Play();
                }

                if (stamina > 0)
                {
                    staminaTimer += Time.deltaTime;
                    if (staminaTimer >= 1)
                    {
                        stamina--;
                        staminaTimer = 0;
                    }
                }
            }
            else
            {
                CurrentAnimation = Animation.Move; // Move
                moveSpeed = 2f;
            }
        }
        else
        {
            StaminaRegen();
            moveSpeed = 2f;
            CurrentAnimation = Animation.Idle; // Idle
            audioSource.Stop();
        }
    }

    // ============================================================
    // Collisions
    // ============================================================

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!iFrameOn)
        {
            if (collision.gameObject.CompareTag("Zombie"))
            {
                iFrameOn = true;
                health--;

                if (health <= 0)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
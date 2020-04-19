﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    // Consts
    const float MAX_SPEED = 1000;
    const float WATER_SHOOT_INTERVAL = 0.05f;
    const int WATER_DEPLETION_IN_FIRE = 10;
    const int CAR_CRASH_DAMAGE = 2;

    // Member vars
    Vector2 moveDirection = Vector2.zero;
    Vector2 lastMoveDirection;
    [SerializeField] float dashSpeed = 500f;
    [SerializeField] float dashDuration = 0.1f;
    [SerializeField] int dashCost = 40; // Water ammo cost per dash
    enum State {
        Driving,
        Dashing
    }
    [SerializeField]
    State state;

    // Unity vars
    [SerializeField] SessionData sessionData;
    [SerializeField] PlayerData playerData;
    [SerializeField] Sprite8Directional sprite8Directional;
    [SerializeField] GameObject waterPelletPrefab;
    [SerializeField] GameObject collisionPrefab;
    [SerializeField] GameObject personPrefab;
    [SerializeField] Rigidbody2D body;
    [SerializeField] Transform helicopter;
    [SerializeField] AudioClip waterSound;
    [SerializeField] AudioClip thudSound;

    public void RescuePeople() {
        for (int i = 0; i < playerData.people; i++) {
            float progress = 1f * i / playerData.people;
            Vector3 offset = MathUtils.PolarToCartesian(360 * progress, 16);
            GameObject person = Instantiate(personPrefab, helicopter.position + offset, Quaternion.identity, transform.parent);
            person.GetComponent<Person>().PickupAfter(1 + progress);
        }

        sessionData.peopleSaved += playerData.people;
        playerData.UnloadPeople();
    }

    public bool IsDashing() {
        return state == State.Dashing;
    }

    void Start() {
        state = State.Driving;
        StartCoroutine(ShootWaterRoutine());
    }

    void Update() {
        playerData.position = body.position;
    }

    void FixedUpdate() {
        moveDirection = new Vector2(Input.GetAxisRaw("PlayerHorizontal"), Input.GetAxisRaw("PlayerVertical"));
        if (moveDirection != Vector2.zero) {
            lastMoveDirection = moveDirection;
        }

        if (state == State.Driving) {
            body.AddForce(moveDirection.normalized * GetSpeed());

            float currentTime = Time.time;
            if (Input.GetAxisRaw("PlayerDash") > 0 && playerData.water >= dashCost) {
                state = State.Dashing;
                playerData.DepleteWater(dashCost);
                StartCoroutine(DashRoutine());
            }
        }
    }

    void LateUpdate() {
        sprite8Directional.SetAngle(MathUtils.VectorToAngle(body.velocity));
    }

    void OnCollisionStay2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Fire")) {
            playerData.DepleteWater(WATER_DEPLETION_IN_FIRE);
        }
    }

    float GetSpeed() {
        return (5f / (5 + playerData.people)) * MAX_SPEED;
    }

    IEnumerator ShootWaterRoutine() {
        while (true) {
            if (playerData.water > 0 && state == State.Driving) {
                float x = Input.GetAxisRaw("WaterHorizontal");
                float y = Input.GetAxisRaw("WaterVertical");
                if (x != 0 || y != 0) {
                    float angle = Mathf.Atan2(y, x);
                    WaterPellet waterPellet = Instantiate(waterPelletPrefab, transform.position + Vector3.up * 20, Quaternion.identity, transform.parent).GetComponent<WaterPellet>();
                    waterPellet.Shoot(angle);
                    playerData.DepleteWater();
                    SoundManager.Play(waterSound);
                }
            }
            yield return new WaitForSeconds(WATER_SHOOT_INTERVAL);
        }
    }

    IEnumerator DashRoutine() {
        float elapsed = 0f;
        Vector2 dashVector = lastMoveDirection * dashSpeed;
        while (elapsed <= dashDuration) {
            elapsed += Time.deltaTime;
            body.velocity = dashVector;
            yield return null;
        }

        state = State.Driving;
    }

    void OnCollisionEnter2D(Collision2D collision) {
        GameObject other = collision.gameObject;
        Person person = other.GetComponent<Person>();
        if (person) {
            playerData.AddPerson();
            person.Remove();
            return;
        }
        // Moving fast enough
        if (body.velocity.sqrMagnitude > 3000) {
            SoundManager.Play(thudSound);
            Instantiate(collisionPrefab, collision.GetContact(0).point, Quaternion.identity, transform.parent);
            SpriteSquish spriteSquish = other.GetComponent<SpriteSquish>();
            if (spriteSquish) {
                spriteSquish.SquishThin();
            }
            Car car = other.GetComponent<Car>();
            if (car && car.IsMoving()) {
                other.GetComponentInChildren<Flammable>().SetOnFire();
            }
        }
    }
}

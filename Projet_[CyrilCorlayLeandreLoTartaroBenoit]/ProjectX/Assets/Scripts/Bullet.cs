using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float bulletDestroyTimer = 2f;
    private int bulletSpeed = 30;

    private void Update()
    {
        bulletDestroyTimer -= Time.deltaTime;
        if (bulletDestroyTimer <= 0)
        {
            Destroy(gameObject);
        }
        transform.position += transform.right * bulletSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(gameObject);
    }
}
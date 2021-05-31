using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    private Player Player;

    void Start()
    {
        Player = FindObjectOfType<Player>();
    }

    void Update()
    {
        transform.position = Player.transform.position.WithZ(transform.position.z);
    }
}
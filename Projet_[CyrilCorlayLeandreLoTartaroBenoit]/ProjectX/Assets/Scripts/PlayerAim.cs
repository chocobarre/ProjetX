using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    private Camera camera;
    private FieldOfView fieldOfView;
    private Vector2 mousePos;
    

    private void Start()
    {
        camera = FindObjectOfType<Camera>();
        fieldOfView = FindObjectOfType<FieldOfView>();
    }

    private void Update()
    {
        mousePos = camera.ScreenToViewportPoint(Input.mousePosition);
    }

    private void FixedUpdate()
    {
        mousePos = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
        float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        if(fieldOfView != null)
        {
            fieldOfView.SetAimDirection(mousePos);
            fieldOfView.SetOrigin(transform.position);
        }
        
    }
}
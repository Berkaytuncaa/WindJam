using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaloonBehaviour : MonoBehaviour
{
    private Rigidbody2D _rb;
    private float _speed = 0.5f;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private bool _reachedLimit = false;
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Block"))
        {
            _reachedLimit = true;
            _rb.velocity = Vector2.zero;
            return;
        }
        if (other.CompareTag("Player") && !_reachedLimit)
        {
            _rb.velocity = Vector2.up * _speed;
        }
    }
}

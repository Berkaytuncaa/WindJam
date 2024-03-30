using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaloonBehaviour : MonoBehaviour
{
    private Rigidbody2D _rb;
    private float _speed = 0.5f;
    private float _stopSpeed = 0f;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _rb.velocity = Vector2.up * _speed;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _rb.velocity = Vector2.up * _stopSpeed;
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    [SerializeField] private Vector2 openPosition;
    [SerializeField] private float moveSpeed;

    private void MovePlatform()
    {
        transform.position = Vector2.MoveTowards(transform.position, openPosition, moveSpeed * Time.deltaTime);
    }
}

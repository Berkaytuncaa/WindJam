using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FireElemental;

public class Wind : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            FireElementalController.Death();
        }
    }
}

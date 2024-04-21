using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balk : MonoBehaviour
{
    //public Transform balk;
    public float speed = 5f;

    void Update()
    {
        transform.Translate(Vector3.back * speed * Time.deltaTime);
    }
}

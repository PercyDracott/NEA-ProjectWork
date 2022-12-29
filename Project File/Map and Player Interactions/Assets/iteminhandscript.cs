using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class iteminhandscript : MonoBehaviour
{
    // Start is called before the first frame update
    SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        spriteRenderer.sprite = FindObjectOfType<HUDManager>().PasstoHand();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ObjectInteraction : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    public float transparency;

    void Start()
    {
        // Make the box collider trigger
        GetComponent<BoxCollider2D>().isTrigger = true;
    }

    private void OnTriggerStay2D(Collider2D col)
    {

        if (col.gameObject.tag == "Player")
        {
            col.gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, transparency);
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        col.gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
    }
}

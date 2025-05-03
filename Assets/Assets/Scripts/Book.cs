using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Book : MonoBehaviour
{
    public GameObject cover;

    void Update()
    {
        // Check for a mouse click
        if (Input.GetMouseButtonDown(0)) // 0 is for left click
        {
            // Raycast from the mouse position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // If the raycast hits this object
            if (Physics.Raycast(ray, out hit))
            {
                // Check if the hit object is the current object
                if (hit.transform.gameObject == cover)
                {
                    // Set the object inactive
                    cover.SetActive(false);
                }
            }
        }
    }
}

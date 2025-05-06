using UnityEngine;

public class CutCloth : MonoBehaviour
{
    public GameObject[] clothPieces; // Assign the 4 cloth pieces in the Inspector
    private Camera cam;

    void Start()
    {
        cam = Camera.main;

        // Ensure the cloth pieces start inactive
        foreach (GameObject piece in clothPieces)
        {
            piece.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.C) && Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    CutCloths();
                }
            }
        }
    }

    void CutCloths()
    {
        // Deactivate the main cloth
        gameObject.SetActive(false);

        // Activate all cloth pieces
        foreach (GameObject piece in clothPieces)
        {
            piece.SetActive(true);
        }
    }
}

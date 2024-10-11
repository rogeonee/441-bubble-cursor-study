using System.Collections.Generic;
using UnityEngine;

public class PointCursor : MonoBehaviour
{
    private Camera mainCam;

    private void Awake()
    {
        mainCam = Camera.main;
    }

    private void Update()
    {
        // Update cursor position based on mouse position
        UpdateCursorPosition();

        // Handle mouse click selection
        HandleSelection();
    }

    private void UpdateCursorPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 10f;  // Set to avoid occlusion by camera
        transform.position = mainCam.ScreenToWorldPoint(mousePosition);
    }

    private void HandleSelection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.zero);
            if (hit.collider != null && hit.collider.TryGetComponent(out Target target))
            {
                target.OnSelect();
            }
        }
    }
}

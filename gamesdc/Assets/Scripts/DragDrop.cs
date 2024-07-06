using UnityEngine;

public class DragDrop : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 offset;
    private CameraController cameraController;

    void Start()
    {
        cameraController = Camera.main.GetComponent<CameraController>();
    }

    void Update()
    {
        if (isDragging)
        {
            // target position based on mouse position
            Vector3 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + offset;
            targetPosition.z = transform.position.z;
            transform.position = targetPosition;
        }
    }


    void OnMouseDown()
    {
        isDragging = true;
        offset = (transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition));
        // Notify the camera controller dragging has started
        cameraController.StartDragging(transform);
    }

    void OnMouseUp()
    {
        isDragging = false;
        // Notify the camera controller dragging has stopped
        cameraController.StopDragging();
    }
}

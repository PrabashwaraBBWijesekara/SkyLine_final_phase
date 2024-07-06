
using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform target;
    private Transform playerTarget;
    private Transform draggedObject;

    [SerializeField] private float smoothTime = 0.3f;
    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        playerTarget = GameObject.FindGameObjectWithTag("Player").transform;
        target = playerTarget; //focusing player as default
    }

    void FixedUpdate()
    {
        if (target != null)
        {
            Vector3 targetPosition = new Vector3(target.position.x, target.position.y, transform.position.z);
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void StartDragging(Transform draggedObjectTransform)
    {
        draggedObject = draggedObjectTransform;
        SetTarget(draggedObject);
    }

    public void StopDragging()
    {
        StartCoroutine(FocusOnDraggedObjectForSeconds(1f)); // Focus on dragged object for some seconds
    }

    private IEnumerator FocusOnDraggedObjectForSeconds(float duration)
    {
        SetTarget(draggedObject);

        yield return new WaitForSeconds(duration);
        SetTarget(playerTarget); // Switch back to the player
    }

}


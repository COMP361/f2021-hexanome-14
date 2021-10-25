using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]

public class CameraMovement2 : MonoBehaviour
{

    [SerializeField]
    private Camera cam;

    private Vector3 dragOrigin;
    private bool drag = false;

    [SerializeField]
    private float zoomStep, minCamSize, maxCamSize;


    private SpriteRenderer mapRenderer;
    private float mapMinX, mapMaxX, mapMinY, mapMaxY;


    // Use this for initialization
    void Awake()
    {
        mapRenderer = GetComponent<SpriteRenderer>();

        mapMinX = mapRenderer.transform.position.x - mapRenderer.bounds.size.x / 2;
        mapMaxX = mapRenderer.transform.position.x + mapRenderer.bounds.size.x / 2;

        mapMinY = mapRenderer.transform.position.y - mapRenderer.bounds.size.y / 2;
        mapMaxY = mapRenderer.transform.position.y + mapRenderer.bounds.size.y / 2;

    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnMouseDown()
    {
        Vector2 position = cam.ScreenToWorldPoint(Input.mousePosition);

        if (EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log("MouseDown on: UI Element");
        } else
        {
            RaycastHit2D hit = Physics2D.Raycast(position, Vector2.zero);
            if (hit.collider != null)
            {
                Debug.Log("MouseDown on: " + hit.collider.gameObject.name);

                if (hit.collider.gameObject == this.gameObject)
                {
                    drag = true;
                    dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
                }
            }
        }
    }

    private void OnMouseDrag()
    {
        if (drag)
        {
            Vector3 difference = dragOrigin - cam.ScreenToWorldPoint(Input.mousePosition);

            cam.transform.position = ClampCamera(cam.transform.position + difference);
        }
    }

    private void OnMouseUp()
    {
        drag = false;
    }

    public void ZoomIn()
    {
        Debug.Log("ZoomIn");
        float newSize = cam.orthographicSize - zoomStep;
        cam.orthographicSize = Mathf.Clamp(newSize, minCamSize, maxCamSize);

        cam.transform.position = ClampCamera(cam.transform.position);
    }


    public void ZoomOut()
    {
        Debug.Log("ZoomOut");
        float newSize = cam.orthographicSize + zoomStep;
        cam.orthographicSize = Mathf.Clamp(newSize, minCamSize, maxCamSize);

        cam.transform.position = ClampCamera(cam.transform.position);
    }

    private Vector3 ClampCamera(Vector3 targetPosition)
    {
        float camHeight = cam.orthographicSize;
        float camWidth = cam.orthographicSize * cam.aspect;

        float minX = mapMinX + camWidth;
        float maxX = mapMaxX - camWidth;
        float minY = mapMinY + camHeight;
        float maxY = mapMaxY - camHeight;

        float newX = Mathf.Clamp(targetPosition.x, minX, maxX);
        float newY = Mathf.Clamp(targetPosition.y, minY, maxY);

        return new Vector3(newX, newY, targetPosition.z);
    }
}

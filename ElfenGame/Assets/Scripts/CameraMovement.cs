using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    private Camera cam;

    private Vector3 dragOrigin;
    private bool drag = false;

    [SerializeField]
    private float zoomStep, minCamSize, maxCamSize;

    [SerializeField]
    private SpriteRenderer mapRenderer;

    [SerializeField]
    private GraphicRaycaster graphicRayCaster;

    private float mapMinX, mapMaxX, mapMinY, mapMaxY;

    // [SerializeField]
    // private EventSystem eventSystem;

    private PointerEventData pointerData;

    private void Awake() {
        pointerData = new PointerEventData(null);

        mapMinX = mapRenderer.transform.position.x - mapRenderer.bounds.size.x / 2;
        mapMaxX = mapRenderer.transform.position.x + mapRenderer.bounds.size.x / 2;

        mapMinY = mapRenderer.transform.position.y - mapRenderer.bounds.size.y / 2;
        mapMaxY = mapRenderer.transform.position.y + mapRenderer.bounds.size.y / 2;
    }

    // Update is called once per frame
    void Update()
    {
        PanCamera();
    }

    private void PanCamera()
    {
        //save position of mouse in world space when drag starts (first time clicked)

        pointerData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();

        graphicRayCaster.Raycast(pointerData, results);
        
        // if (results.Count > 0)
        //     return;

        if (Input.GetMouseButtonDown(0) && results.Count == 0)
        {
            drag = true;
            dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
        }
        //calculate distance between drag origin and new position if it is still held down

        if (drag)
        {
            Vector3 difference = dragOrigin - cam.ScreenToWorldPoint(Input.mousePosition);

            Debug.Log("origin " + dragOrigin + " newPosition " + cam.ScreenToWorldPoint(Input.mousePosition));
            //move the camera by that distance

            cam.transform.position = ClampCamera(cam.transform.position + difference);
        }

        if (Input.GetMouseButtonUp(0))
        {
            drag = false;
        }
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
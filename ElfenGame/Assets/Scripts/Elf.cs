using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Elf : MonoBehaviour
{
    private Vector3 dragOrigin;
    private bool drag = false;

    [SerializeField]
    Camera cam;

    [SerializeField]
    MouseActivityManager mouseActivityManager;


    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log("MouseDown on: UI Element");
        }
        else
        {
            RaycastHit2D hit = Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                Debug.Log("MouseDown on: " + hit.collider.gameObject.name);

                if (hit.collider.gameObject == this.gameObject)
                {
                    drag = true;
                    dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
                    dragOrigin = new Vector3(dragOrigin.x, dragOrigin.y, transform.position.z);
                    transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
                    GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 0.65f);
                    mouseActivityManager.beginElfDrag();

                }
            }

        }
    }

    private void OnMouseDrag()
    {
        if (drag)
        {
            Vector2 MousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            Vector2 objPosition = Camera.main.ScreenToWorldPoint(MousePosition);
            transform.position = new Vector3(objPosition.x, objPosition.y, dragOrigin.z);
            mouseActivityManager.whileElfDrag();
        }
    }

    private void OnMouseUp()
    {
        if (drag)
        {
            NewTown town = mouseActivityManager.endElfDrag(this);

            transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);

            if (town == null)
            {
                transform.position = dragOrigin;
            } else
            {
                transform.position = new Vector3(town.gameObject.transform.position.x, town.gameObject.transform.position.y, dragOrigin.z);
            }

            drag = false;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseActivityManager : MonoBehaviour
{

    [SerializeField]
    private Camera cam;

    private NewTown hoveringOverTown;

    void Awake()
    {
        hoveringOverTown = null;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    

    // Update is called once per frame
    void Update()
    {
        
    }

    public void beginElfDrag()
    {
        hoveringOverTown = getMouseTown();
        if (hoveringOverTown != null)
        {
            hoveringOverTown.OnDragEnter();
        }
    }

    private NewTown getMouseTown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return null;

        Vector2 position = cam.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D[] results = Physics2D.RaycastAll(position, Vector2.zero);


        foreach (RaycastHit2D hit in results)
        {
            if (hit.collider == null || hit.collider.gameObject == null)
                continue;

            NewTown town = hit.collider.gameObject.GetComponent<NewTown>();

            if (town == null)
                continue;

            return town;

        }

        return null;
    }


    public void whileElfDrag()
    {
        NewTown curTown = getMouseTown();

        if (hoveringOverTown == curTown)
            return;

        if (hoveringOverTown != null)
            hoveringOverTown.OnDragExit();

        if (curTown != null)
            curTown.OnDragEnter();

        hoveringOverTown = curTown;
    }

    public NewTown endElfDrag(Elf elf)
    {
        if (hoveringOverTown != null)
            hoveringOverTown.OnDragExit();

        return getMouseTown();

    }


}

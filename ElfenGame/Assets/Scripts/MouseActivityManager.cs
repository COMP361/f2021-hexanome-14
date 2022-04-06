using UnityEngine;
using UnityEngine.EventSystems;

public class MouseActivityManager : MonoBehaviour
{
    #region singleton 

    private static MouseActivityManager _instance;

    public static MouseActivityManager manager
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<MouseActivityManager>();
            }
            return _instance;
        }
    }

    #endregion   

    [SerializeField]
    public Camera cam;

    private IDragOver dragOver;

    void Awake()
    {
        dragOver = null;
    }

    public void BeginDrag<T>() where T : IDragOver
    {
        dragOver = GetMouseElement<T>();
        if (dragOver != null)
        {
            dragOver.OnDragEnter();
        }
    }

    public void WhileDrag<T>() where T : IDragOver
    {
        T curElement = GetMouseElement<T>();


        if (dragOver != null && !dragOver.Equals(curElement))
        {
            dragOver.OnDragExit();
        }

        if (curElement != null && !curElement.Equals(dragOver))
        {
            curElement.OnDragEnter();
        }

        dragOver = curElement;
    }

    public T EndDrag<T>() where T : IDragOver
    {
        if (dragOver != null)
            dragOver.OnDragExit();

        return GetMouseElement<T>();

    }

    private T GetMouseElement<T>()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return default(T);

        Vector2 position = cam.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D[] results = Physics2D.RaycastAll(position, Vector2.zero);


        foreach (RaycastHit2D hit in results)
        {
            if (hit.collider == null || hit.collider.gameObject == null)
                continue;

            T element = hit.collider.gameObject.GetComponent<T>();

            if (element == null)
                continue;

            return element;

        }

        return default(T);
    }


}

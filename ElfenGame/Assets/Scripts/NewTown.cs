using UnityEngine;

public class NewTown : MonoBehaviour, IDragOver
{
    private GameObject pointsHolder;
    private int goldValue;

    public void Awake()
    {
        pointsHolder = (GameObject)Instantiate(Resources.Load("PointsHolder"), transform);
    }

    public GameObject pointPrefab;

    private GameObject goldObject = null, redX = null;
    public void OnDragEnter()
    {
        //Not a built in method
        transform.localScale = new Vector3(1.2f, 1.2f, 1.0f);
    }

    public void OnDragExit()
    {
        //Not a built in method
        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }

    public int getGoldValue()
    {
        return goldValue;
    }

    public void DisplayVisited()
    {
        GridManager pointsManager = pointsHolder.GetComponent<GridManager>();
        pointsManager.Clear();

        foreach (Player p in Player.GetAllPlayers())
        {
            bool isVisited = p.isVisited(name);
            if (!isVisited)
            {
                GameObject g = Instantiate(pointPrefab);
                SpriteRenderer gs = g.transform.GetComponent<SpriteRenderer>();
                gs.color = p.playerColor.GetColor();
                g.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                pointsManager.AddElement(g);
            }
        }
    }

    internal void SetEndTown()
    {
        // instantiate the end town if it does not exist
        if (redX != null) return;
        redX = (GameObject)Instantiate(Resources.Load("RedX"), transform);
        redX.name = "EndTownMarker";
        redX.transform.localPosition = new Vector3(0.0f, 0.0f, -1.0f);
    }

    internal void SetGold(int v)
    {
        goldValue = v;
        if (v == 0) return; // Don't display anything for elvenhold
        if (goldObject == null)
        {
            goldObject = (GameObject)Instantiate(Resources.Load("GoldValue"), transform);
            goldObject.transform.localPosition = new Vector3(0.0f, 0.0f, -1.0f);
        }

        goldObject.GetComponentInChildren<TextMesh>().text = v.ToString();
    }
}

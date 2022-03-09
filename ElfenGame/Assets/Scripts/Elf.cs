using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;

public class Elf : MonoBehaviour 
{
    private Vector3 dragOrigin;
    private GridManager dragOriginManager;
    private bool drag = false;

    private Player player;

    public void LinkToPlayer(Player p)
    {
        player = p;
        p.SetElf(this);
        GetComponent<SpriteRenderer>().sprite = p.playerColor.GetSprite();
        name = p.userName;
    }

    public void UpdateColor()
    {
        if (player != null) GetComponent<SpriteRenderer>().sprite = player.playerColor.GetSprite();
    }

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log("MouseDown on: UI Element");
        }
        else
        {
            RaycastHit2D hit = Physics2D.Raycast(GameConstants.mainCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                Debug.Log("MouseDown on: " + hit.collider.gameObject.name);

                if (hit.collider.gameObject == gameObject)
                {
                    if (!Player.GetLocalPlayer().IsMyTurn() || Player.GetLocalPlayer() != player) return;
                    drag = true;
                    dragOrigin = GameConstants.mainCamera.ScreenToWorldPoint(Input.mousePosition);
                    dragOrigin = new Vector3(dragOrigin.x, dragOrigin.y, transform.position.z);
                    transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
                    GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 0.65f);

                    GridManager parentGrid = GetComponentInParent<GridManager>();
                    if (parentGrid != null)
                    {
                        parentGrid.RemoveElement(gameObject);
                    }
                    dragOriginManager = parentGrid;
                    GameConstants.mouseActivityManager.BeginDrag<NewTown>();

                }
            }

        }
    }

    private void OnMouseDrag()
    {
        if (drag)
        {
            Vector2 MousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            Vector2 objPosition = GameConstants.mainCamera.ScreenToWorldPoint(MousePosition);
            transform.position = new Vector3(objPosition.x, objPosition.y, dragOrigin.z);
            GameConstants.mouseActivityManager.WhileDrag<NewTown>();
        }
    }

    private void OnMouseUp()
    {
        if (drag)
        {
            NewTown town = GameConstants.mouseActivityManager.EndDrag<NewTown>();

            transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);

            if (town == null)
            {
                if (dragOriginManager != null)
                {
                    dragOriginManager.AddElement(gameObject);
                } else
                {
                    transform.position = dragOrigin;
                }
                
            } else
            {
                //transform.position = new Vector3(town.gameObject.transform.position.x, town.gameObject.transform.position.y, dragOrigin.z);
                player.curTown = town.name;
            }

            drag = false;
        }
    }

    public void MoveToTown(string destination, string source)
    {
        NewTown sourceTown = GameConstants.townDict[source];
        if (sourceTown != null)
        {
            sourceTown.GetComponent<GridManager>().RemoveElement(gameObject);
	    }

        NewTown destinationTown = GameConstants.townDict[destination];
        if (destinationTown == null)
        {
            GameConstants.townDict = null;
            destinationTown = GameConstants.townDict[destination];
        }
        Debug.Log($"Elf {player.userName} moved to {destination}");
        destinationTown.GetComponent<GridManager>().AddElement(gameObject);

    }

}

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
                    if (!Player.GetLocalPlayer().IsMyTurn() || Player.GetLocalPlayer() != player || Game.currentGame.curPhase != GamePhase.Travel) return;
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
                    MouseActivityManager.manager.BeginDrag<NewTown>();

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
            MouseActivityManager.manager.WhileDrag<NewTown>();
        }
    }

    private void OnMouseUp()
    {
        if (drag)
        {
            drag = false;
            NewTown town = MouseActivityManager.manager.EndDrag<NewTown>();

            transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);

            if (town != null && MainUIManager.manager)
            {
                List<CardEnum> cards = MainUIManager.manager.GetSelectedCards();
                foreach (PathScript pathScript in GameConstants.roadDict.Values)
                {
                    if (pathScript.CanMoveOnPath(GameConstants.townDict[player.curTown], town, cards))
                    {
                        player.curTown = town.name;
                        player.RemoveCards(cards.ToArray());

                        MainUIManager.manager.ResetRoadColors();
                        return;
                    }
                }
            }
            if (dragOriginManager != null)
            {
                dragOriginManager.AddElement(gameObject);
            }

        }
    }

    public void SetTown(string destination)
    {
        NewTown destinationTown = GameConstants.townDict[destination];
        GridManager sourceTown = GetComponentInParent<GridManager>();

        if (sourceTown != null)
        {
            sourceTown.RemoveElement(gameObject);
        }

        if (destinationTown != null)
        {
            destinationTown.GetComponent<GridManager>().AddElement(gameObject);
        }
        Debug.Log($"{player.userName} moved to {destination}");
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.GroupsModels;
using System;

public class PlayfabManager : MonoBehaviour
{

    private string _playfabId;
    private string _groupID = "";

    public void Login(string username)
    {
        var request = new LoginWithCustomIDRequest { CustomId = username, CreateAccount = true };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnFailure);
    }

    public void AddGenericID(string username)
    {
        var request = new AddGenericIDRequest { GenericId = new GenericServiceId { UserId = username, ServiceName = "ElfenGame" } };
        PlayFabClientAPI.AddGenericID(request, OnAddGenericIDSuccess, OnFailure);
    }

    private void OnAddGenericIDSuccess(AddGenericIDResult obj)
    {
        Debug.Log("Added Generic ID");
    }

    public void CreateAccount(string username, string password)
    {
        var request = new LoginWithCustomIDRequest { CustomId = username, CreateAccount = true };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnFailure);
    }

    public string GetGroupId()
    {
        return _groupID;
    }

    public void AddPlayersToGroup(List<string> players)
    {
        if (_groupID == "")
        {
            Debug.Log("No group ID");
            return;
        }

        GetEntityKeys(players, (GetPlayFabIDsFromGenericIDsResult result) =>
        {
            foreach (var entity in result.Data)
            {
                Debug.Log("Got Entity Key for " + entity.GenericId.UserId);
                AddMemberToGroup(entity.PlayFabId);
            }
        });


    }

    public void GetEntityKeys(List<string> usernames, Action<GetPlayFabIDsFromGenericIDsResult> callback)
    {
        List<GenericServiceId> genericIds = new List<GenericServiceId>();
        foreach (string username in usernames)
        {
            genericIds.Add(new GenericServiceId { UserId = username, ServiceName = "ElfenGame" });
        }
        var request = new GetPlayFabIDsFromGenericIDsRequest { GenericIDs = genericIds };
        PlayFabClientAPI.GetPlayFabIDsFromGenericIDs(request, callback, OnFailure);

    }

    private void OnLoginSuccess(LoginResult obj)
    {
        Debug.Log("PlayFab Login Success");
        _playfabId = obj.PlayFabId;
        AddGenericID(Lobby.myUsername);
    }

    // Create a new group
    public void CreateGroup()
    {
        System.Guid guid = System.Guid.NewGuid();
        string groupName = guid.ToString();
        var request = new CreateGroupRequest { GroupName = groupName };
        PlayFabGroupsAPI.CreateGroup(request, OnCreateGroupSuccess, OnFailure);
    }

    private void OnCreateGroupSuccess(CreateGroupResponse obj)
    {
        Debug.Log("PlayFab Create Group Success");
        // Save Group ID
        _groupID = obj.Group.Id;

        Game.currentGame.playfabId = _groupID;
        Game.currentGame.SyncGameProperties();

        Debug.Log("Group ID: " + _groupID);

        // Add other players to group

        List<string> players = Game.currentGame.mPlayers;
        players.Remove(Lobby.myUsername);

        if (players.Count > 0)
            AddPlayersToGroup(players);

    }

    // Handle all failure cases
    private void OnFailure(PlayFabError obj)
    {
        Debug.LogError($"PlayFab Failure: {obj.GenerateErrorReport()}");
    }

    public void AddMemberToGroup(string playfabId)
    {
        PlayFab.GroupsModels.EntityKey groupKey = new PlayFab.GroupsModels.EntityKey { Id = _groupID, Type = "group" };

        PlayFab.GroupsModels.EntityKey entityKey = new PlayFab.GroupsModels.EntityKey { Id = playfabId, Type = "master_player_account" };


        var request = new AddMembersRequest { Group = groupKey, Members = new List<PlayFab.GroupsModels.EntityKey> { entityKey } };
        PlayFabGroupsAPI.AddMembers(request, OnAddMemberToGroupSuccess, OnFailure);
    }

    // public void CheckInGroup(string groupId)
    // {
    //     _groupID = groupId;
    //     PlayFab.GroupsModels.EntityKey playerKey = new PlayFab.GroupsModels.EntityKey { Id = _playfabId, Type = "master_player_account" };

    //     var request = new ListMembershipRequest { Entity = playerKey };
    //     PlayFabGroupsAPI.ListMembership(request, OnCheckInGroupSuccess, OnFailure);
    // }

    // private void OnCheckInGroupSuccess(ListMembershipResponse obj)
    // {
    //     Debug.Log("PlayFab Check In Group Success");
    //     // Check if the group is in the list
    //     bool isInGroup = false;
    //     foreach (var group in obj.Groups)
    //     {
    //         if (group.Group.Id == _groupID)
    //         {
    //             isInGroup = true;
    //         }
    //     }

    //     if (!isInGroup)
    //     {
    //     }
    // }

    private void OnAddMemberToGroupSuccess(PlayFab.GroupsModels.EmptyResponse obj)
    {
        Debug.Log($"Playfab Add Member to Group Success");
    }

    public void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}

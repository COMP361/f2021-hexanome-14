using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System;
using PlayFab.GroupsModels;

public class PlayfabManager : MonoBehaviour
{

    private string _playfabId;
    private string _groupID = "";

    public void Login(string username)
    {
        var request = new LoginWithCustomIDRequest { CustomId = username, CreateAccount = true };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);

    }

    public string GetGroupId()
    {
        return _groupID;
    }

    private void OnLoginFailure(PlayFabError obj)
    {
        Debug.LogError($"PlayFab Login Failure: {obj.GenerateErrorReport()}");
    }

    private void OnLoginSuccess(LoginResult obj)
    {
        Debug.Log("PlayFab Login Success");
        _playfabId = obj.PlayFabId;
    }

    // Create a new group
    public void CreateGroup(string groupName)
    {
        var request = new CreateGroupRequest { GroupName = groupName };
        PlayFabGroupsAPI.CreateGroup(request, OnCreateGroupSuccess, OnCreateGroupFailure);
    }

    private void OnCreateGroupFailure(PlayFabError obj)
    {
        Debug.LogError($"PlayFab Create Group Failure: {obj.GenerateErrorReport()}");
    }

    private void OnCreateGroupSuccess(CreateGroupResponse obj)
    {
        Debug.Log("PlayFab Create Group Success");
        // Save Group ID
        _groupID = obj.Group.Id;

        // Add the current user to the group
    }

    // Handle all failure cases
    private void OnFailure(PlayFabError obj)
    {
        Debug.LogError($"PlayFab Failure: {obj.GenerateErrorReport()}");
    }

    public void AddMemberToGroup(string groupId, string memberId)
    {
        PlayFab.GroupsModels.EntityKey groupKey = new PlayFab.GroupsModels.EntityKey { Id = groupId, Type = "group" };

        PlayFab.GroupsModels.EntityKey entityKey = new PlayFab.GroupsModels.EntityKey { Id = memberId, Type = "title_player_account" };

        var request = new AddMembersRequest { Group = groupKey, Members = new List<PlayFab.GroupsModels.EntityKey> { entityKey } };
        PlayFabGroupsAPI.AddMembers(request, OnAddMemberToGroupSuccess, OnFailure);
    }

    public void CheckInGroup(string groupId)
    {

        PlayFab.GroupsModels.EntityKey playerKey = new PlayFab.GroupsModels.EntityKey { Id = _playfabId, Type = "title_player_account" };

        var request = new ListMembershipRequest { Entity = playerKey };
        PlayFabGroupsAPI.ListMembership(request, OnCheckInGroupSuccess, OnFailure);
    }

    private void OnCheckInGroupSuccess(ListMembershipResponse obj)
    {
        Debug.Log("PlayFab Check In Group Success");
        // Check if the group is in the list
        bool isInGroup = false;
        foreach (var group in obj.Groups)
        {
            if (group.Group.Id == _groupID)
            {
                isInGroup = true;
            }
        }

        if (!isInGroup)
        {
            AddMemberToGroup(_groupID, _playfabId);
        }
    }

    private void OnAddMemberToGroupSuccess(PlayFab.GroupsModels.EmptyResponse obj)
    {
        Debug.Log($"Playfab Add Member to Group Success");
    }

    public void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}

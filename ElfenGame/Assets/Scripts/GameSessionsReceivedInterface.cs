using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Lobby;

public interface GameSessionsReceivedInterface
{
    void OnUpdatedGameListReceived(List<GameSession> gameSessions);

}

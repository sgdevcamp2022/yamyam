package com.matchservice.core.domain;

import com.matchservice.core.domain.match.Match;
import java.util.Map;
import java.util.concurrent.ConcurrentHashMap;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.config.ConfigurableBeanFactory;
import org.springframework.context.annotation.Scope;
import org.springframework.messaging.simp.SimpMessageSendingOperations;
import org.springframework.stereotype.Component;

@Component
@Scope(value = ConfigurableBeanFactory.SCOPE_SINGLETON)
public class Lobby {

    private static Logger log = LoggerFactory.getLogger(Lobby.class);
    private static final Map<String, Player> connectedPlayers = new ConcurrentHashMap<>();

    public void addPlayer(String sessionId, Player player) {
        connectedPlayers.put(sessionId, player);
    }

    public void removePlayer(String sessionId) {
        connectedPlayers.remove(sessionId);
    }

    public static SimpMessageSendingOperations getPlayerOperation(String sessionId) {
        return connectedPlayers.get(sessionId).getOperate();
    }

    public Player getPlayerInfo(String sessionId) {
        return connectedPlayers.get(sessionId);
    }
}

package com.pokerservice.adapter.in.ws;

import com.pokerservice.core.domain.Game;
import com.pokerservice.core.domain.GameType;
import com.pokerservice.core.domain.Player;
import java.util.List;
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
public class GameManager {

    private static final Logger logger = LoggerFactory.getLogger(GameManager.class);

    private static final Map<String, Player> connectedPlayers = new ConcurrentHashMap<>();
    private static final Map<Long, Game> activateGames = new ConcurrentHashMap<>();

    public static void removePlayer(String sessionId) {
        connectedPlayers.remove(sessionId);
    }

    public static void activateGame(Game game) {
        activateGames.put(game.getId(), game);
    }

    public static Game getActivateGame(long gameId) {
        Game game = activateGames.get(gameId);
        if (game == null) {
            game = new Game(gameId, GameType.P2);
            activateGames.put(game.getId(), game);
        }
        return activateGames.get(gameId);
    }

    public static void addPlayerSessionInfo(Player player) {
        connectedPlayers.put(player.getSession(), player);
    }

    public static List<Player> getPlayersByGameId(long gameId) {
        return activateGames.get(gameId).getPlayers();
    }

    public static Player findPlayerBySessionId(String userSessionId) {
        return connectedPlayers.get(userSessionId);
    }

    public void exitGame(Player player) {
        activateGames.get(player.getGameId()).exitGame(player);
    }

    public static SimpMessageSendingOperations getPlayerOperation(String sessionId) {
        return connectedPlayers.get(sessionId).getOperate();
    }

    public Player getPlayerInfo(String sessionId) {
        return connectedPlayers.get(sessionId);
    }
}

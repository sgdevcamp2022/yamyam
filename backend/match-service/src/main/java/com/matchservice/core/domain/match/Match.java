package com.matchservice.core.domain.match;

import com.matchservice.core.domain.Lobby;
import com.matchservice.core.domain.Player;
import com.matchservice.core.domain.message.MatchMessage;
import java.util.Set;
import java.util.concurrent.ConcurrentHashMap;
import org.springframework.messaging.handler.annotation.Payload;
import org.springframework.messaging.simp.SimpMessageSendingOperations;

public class Match implements Comparable<Match> {

    private long id;
    private GameType gameType;

    public enum GameType {

        P2(2),
        P4(4);

        private int playerSize;

        GameType(int playerSize) {
            this.playerSize = playerSize;
        }

        public int getPlayerSize() {
            return playerSize;
        }
    }

    private Set<Player> waitingPlayers;
    private MatchStatus matchStatus;

    public Match(GameType gameType) {
        this.gameType = gameType;
        waitingPlayers = ConcurrentHashMap.newKeySet();
        matchStatus = MatchStatus.WAITING;
    }

    public boolean isFull() {
        return gameType.getPlayerSize() == waitingPlayers.size();
    }

    public void addPlayer(Player player) {
        if (!waitingPlayers.contains(player)) {
            waitingPlayers.add(player);
        }
    }

    public void deletePlayer(Player player) {
        waitingPlayers.remove(player);
    }

    public void setId(long id) {
        this.id = id;
    }

    public long getId() {
        return id;
    }

    public GameType getGameType() {
        return gameType;
    }

    public void changeStatus(MatchStatus matchStatus) {
        this.matchStatus = matchStatus;
    }

    public MatchStatus getMatchStatus() {
        return matchStatus;
    }

    protected void send(Player player, @Payload MatchMessage matchMessage) {
        SimpMessageSendingOperations template = Lobby.getPlayerOperation(player.getSession());
        template.convertAndSend("/topic/match/" + player.getSession(), matchMessage);
    }

    public void sendAll(MatchMessage matchMessage) {
        for (Player waitingPlayer : waitingPlayers) {
            send(waitingPlayer, matchMessage);
        }
    }

    public GameType getMatchType() {
        return gameType;
    }

    public Set<Player> getWaitingPlayers() {
        return waitingPlayers;
    }

    @Override
    public int compareTo(Match o) {
        return o.getWaitingPlayers().size() - this.getWaitingPlayers().size();
    }
}

package com.matchservice.core.domain;

import java.util.Comparator;
import java.util.Set;
import java.util.concurrent.ConcurrentHashMap;
import org.springframework.messaging.handler.annotation.Payload;
import org.springframework.messaging.simp.SimpMessageSendingOperations;

public class Match implements Comparable<Match> {

    private long id;
    private MatchType matchType;
    private Set<Player> waitingPlayers;
    private Set<Player> acceptPlayers;

    public Match(MatchType matchType) {
        this.matchType = matchType;
        waitingPlayers = ConcurrentHashMap.newKeySet();
        acceptPlayers = ConcurrentHashMap.newKeySet();
    }

    public boolean isFull() {
        return matchType.getPlayerAmount() == waitingPlayers.size();
    }

    public void addPlayer(Player player) {
        waitingPlayers.add(player);
    }

    public void deletePlayer(Player player) {
        waitingPlayers.remove(player);
    }

    public void acceptMatching(Player player) {
        if (waitingPlayers.contains(player)) {
            acceptPlayers.add(player);
        }
    }

    public boolean allAccept() {
        return matchType.getPlayerAmount() == acceptPlayers.size();
    }

    public void setId(long id) {
        this.id = id;
    }

    public long getId() {
        return id;
    }

    protected void send(Player player, @Payload MatchMessage matchMessage) {
        SimpMessageSendingOperations template = Lobby.getSender(player.getSession());
        template.convertAndSendToUser(player.getSession(), "/topic/match", matchMessage);
    }

    public void sendAll(MatchMessage matchMessage) {
        for (Player waitingPlayer : waitingPlayers) {
            send(waitingPlayer, matchMessage);
        }
    }

    public MatchType getMatchType() {
        return matchType;
    }

    public Set<Player> getWaitingPlayers() {
        return waitingPlayers;
    }

    public Set<Player> getAcceptPlayers() {
        return acceptPlayers;
    }

    @Override
    public int compareTo(Match o) {
        return o.getWaitingPlayers().size() - this.getWaitingPlayers().size();
    }
}

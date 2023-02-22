package com.matchservice.core.domain;

import java.time.LocalDateTime;
import java.util.List;
import java.util.Objects;

public class Team {

    private Player leader;
    private List<Player> players;
    private LocalDateTime createdAt;

    public Team(Player leader, List<Player> players) {
        this.leader = leader;
        this.players = players;
        this.createdAt = LocalDateTime.now();
    }

    public void exitTeam(Player player) {
        if (players.contains(player)) {
            players.remove(player);
        }
    }

    public Player getLeader() {
        return leader;
    }

    public List<Player> getPlayers() {
        return players;
    }

    @Override
    public boolean equals(Object o) {
        if (this == o) {
            return true;
        }
        if (o == null || getClass() != o.getClass()) {
            return false;
        }
        Team team = (Team) o;
        return Objects.equals(leader, team.leader);
    }

    @Override
    public int hashCode() {
        return Objects.hash(leader);
    }
}

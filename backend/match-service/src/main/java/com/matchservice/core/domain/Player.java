package com.matchservice.core.domain;

import java.time.LocalDateTime;
import java.util.Objects;

public class Player {

    private long id;
    private String session;
    private LocalDateTime penaltyTime;

    public Player(long id, String session) {
        this.id = id;
        this.session = session;
        this.penaltyTime = LocalDateTime.now().minusMinutes(1);
    }

    public long getId() {
        return id;
    }

    public void setId(long id) {
        this.id = id;
    }

    public String getSession() {
        return session;
    }

    public void setSession(String session) {
        this.session = session;
    }

    public boolean isPenaltyUser() {
        return LocalDateTime.now().isBefore(penaltyTime);
    }

    public void givePenalty() {
        penaltyTime = LocalDateTime.now().plusMinutes(5);
    }

    @Override
    public boolean equals(Object o) {
        if (this == o) {
            return true;
        }
        if (o == null || getClass() != o.getClass()) {
            return false;
        }
        Player player = (Player) o;
        return id == player.id;
    }

    @Override
    public int hashCode() {
        return Objects.hash(id);
    }
}

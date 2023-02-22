package com.matchservice.core.domain;

import java.util.Objects;
import org.springframework.messaging.simp.SimpMessageSendingOperations;

public class Player {

    private long id;
    private String session;
    private SimpMessageSendingOperations operate;

    public Player(String session, SimpMessageSendingOperations operate) {
        this.session = session;
        this.operate = operate;
    }

    public Player(long id, String session, SimpMessageSendingOperations operate) {
        this.id = id;
        this.session = session;
        this.operate = operate;
    }

    public void setId(long id) {
        this.id = id;
    }

    public long getId() {
        return id;
    }

    public String getSession() {
        return session;
    }

    public SimpMessageSendingOperations getOperate() {
        return operate;
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
        return id == player.id && Objects.equals(session, player.session);
    }

    @Override
    public int hashCode() {
        return Objects.hash(id, session);
    }

    @Override
    public String toString() {
        return "Player{" +
            "id=" + id +
            ", session='" + session + '\'' +
            '}';
    }
}

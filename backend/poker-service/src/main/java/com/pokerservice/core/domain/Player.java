package com.pokerservice.core.domain;

import java.util.Objects;
import org.springframework.messaging.simp.SimpMessageSendingOperations;

public class Player {

    private final long id;
    private final String session;
    private final String nickname;
    private int currentBetAmount;
    private int chip;
    private int card;
    private int order;
    private long gameId;
    private PlayerStatus status;
    private SimpMessageSendingOperations operate;

    private Player(long id, String session, String nickname, long gameId) {
        this.id = id;
        this.session = session;
        this.nickname = nickname;
        this.chip = 100;
        this.status = PlayerStatus.PLAYING;
        this.gameId = gameId;
    }

    public static Player create(long userId, String session, String nickname, long gameId) {
        return new Player(userId, session, nickname, gameId);
    }

    public void die() {
        status = PlayerStatus.DIE;
    }

    public void betting(int betAmount) {
        this.currentBetAmount += betAmount;
        this.chip -= betAmount;
    }

    public int allIn() {
        int allInChip = chip;
        this.chip = 0;
        this.currentBetAmount += allInChip;
        return allInChip;
    }

    public void setOperate(SimpMessageSendingOperations operate) {
        this.operate = operate;
    }

    public SimpMessageSendingOperations getOperate() {
        return operate;
    }

    public long getId() {
        return id;
    }

    public String getSession() {
        return session;
    }

    public String getNickname() {
        return nickname;
    }

    public int getCurrentBetAmount() {
        return currentBetAmount;
    }

    public int getChip() {
        return chip;
    }

    public void addChip(int chipAmount) {
        this.chip += chipAmount;
    }

    public void minusChip(int chipAmount) {
        this.chip -= chipAmount;
    }

    public int getCard() {
        return card;
    }

    public void setCardInfo(int card) {
        this.card = card;
    }

    public int getOrder() {
        return order;
    }

    public void setOrder(int order) {
        this.order = order;
    }

    public PlayerStatus getStatus() {
        return status;
    }

    public void changeStatus(PlayerStatus status) {
        this.status = status;
    }

    public long getGameId() {
        return gameId;
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
    public String toString() {
        return "Player{" +
            "userId=" + id +
            ", session='" + session + '\'' +
            ", nickname='" + nickname + '\'' +
            ", currentBetAmount=" + currentBetAmount +
            ", chip=" + chip +
            ", card=" + card +
            ", order=" + order +
            ", status=" + status +
            ", operate=" + operate +
            '}';
    }

    @Override
    public int hashCode() {
        return Objects.hash(id);
    }
}

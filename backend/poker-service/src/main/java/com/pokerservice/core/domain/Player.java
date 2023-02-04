package com.pokerservice.core.domain;

import java.util.Objects;

public class Player {

    private final long userId;
    private final String session;
    private final String nickname;
    private int currentBetAmount;
    private int chip;
    private int card;
    private int seatNo;
    private PlayerStatus status;

    private Player(long userId, String session, String nickname) {
        this.userId = userId;
        this.session = session;
        this.nickname = nickname;
        this.chip = 100;
        this.status = PlayerStatus.PLAYING;
    }

    public static Player create(long userId, String session, String nickname) {
        return new Player(userId, session , nickname);
    }

    public long getUserId() {
        return userId;
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

    public void betting(int betAmount) {
        this.currentBetAmount = betAmount;
    }

    public int getChip() {
        return chip;
    }

    public void updateChip(int chipAmount) {
        this.chip += chip;
    }

    public int getCard() {
        return card;
    }

    public void drawCard(int card) {
        this.card = card;
    }

    public int getSeatNo() {
        return seatNo;
    }

    public void setSeatNo(int seatNo) {
        this.seatNo = seatNo;
    }

    public PlayerStatus getStatus() {
        return status;
    }

    public void changeStatus(PlayerStatus status) {
        this.status = status;
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
        return userId == player.userId && Objects.equals(session, player.session)
            && Objects.equals(nickname, player.nickname);
    }

    @Override
    public int hashCode() {
        return Objects.hash(userId, session, nickname);
    }

    @Override
    public String toString() {
        return "Player{" +
            "userId=" + userId +
            ", session='" + session + '\'' +
            ", nickname='" + nickname + '\'' +
            ", currentBetAmount=" + currentBetAmount +
            ", chip=" + chip +
            ", card=" + card +
            ", seatNo=" + seatNo +
            ", status=" + status +
            '}';
    }
}

package com.pokerservice.core.domain;

public enum GameType {

    P2(2),
    P4(4);

    private final int playerCount;

    GameType(int playerCount) {
        this.playerCount = playerCount;
    }

    public int getPlayerCount() {
        return playerCount;
    }
}

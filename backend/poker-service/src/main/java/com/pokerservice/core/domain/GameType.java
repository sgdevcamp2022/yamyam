package com.pokerservice.core.domain;

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

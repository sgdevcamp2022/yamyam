package com.matchservice.core.domain;

public enum MatchType {

    P2(2),
    P4(4);

    private int playerAmount;

    MatchType(int playerAmount) {
        this.playerAmount = playerAmount;
    }

    public int getPlayerAmount() {
        return playerAmount;
    }
}

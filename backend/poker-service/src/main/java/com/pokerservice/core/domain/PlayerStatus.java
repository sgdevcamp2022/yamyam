package com.pokerservice.core.domain;

public enum PlayerStatus {
    PLAYING(1),
    DIE(2),
    LOOSE(8),
    EXIT(9);

    private final int flag;

    PlayerStatus(int flag) {
        this.flag = flag;
    }

    public int getFlag() {
        return flag;
    }
}

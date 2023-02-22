package com.pokerservice.core.domain;

public enum PlayerStatus {
    PLAYING(1),
    RAISE(2),
    CALL(3),
    DIE(4),
    ALLIN(5),
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

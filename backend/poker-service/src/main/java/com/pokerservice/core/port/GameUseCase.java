package com.pokerservice.core.port;

public interface GameUseCase {

    boolean gameStart(long gameId);

    void betting(long gameId, int bettingAmount);
}

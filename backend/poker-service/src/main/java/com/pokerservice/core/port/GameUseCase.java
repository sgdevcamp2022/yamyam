package com.pokerservice.core.port;

public interface GameUseCase {

    void betting(long gameId, int bettingAmount);
}

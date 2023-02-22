package com.pokerservice.core.port.in;

public interface BattleUseCase {

    void calcGameResult(long gameId, long winnerId);
}

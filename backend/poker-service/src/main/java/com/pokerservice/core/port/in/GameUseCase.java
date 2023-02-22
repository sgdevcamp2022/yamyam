package com.pokerservice.core.port.in;

import com.pokerservice.core.domain.Player;

public interface GameUseCase {

    void joinGame(long gameId, Player player);

    void betting(long gameId, long playerId, int bettingAmount);

    boolean checkReady(long gameId, long playerId);

    void sendFocus(long gameId);

    void drawCard(long gameId);

    void die(long gameId, long playerId);
}

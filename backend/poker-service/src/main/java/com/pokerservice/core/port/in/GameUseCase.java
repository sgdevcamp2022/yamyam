package com.pokerservice.core.port.in;

import com.pokerservice.adapter.in.ws.message.PokerMessage.MessageType;
import com.pokerservice.adapter.in.ws.message.content.clientContent.BetRequestContent;
import com.pokerservice.adapter.in.ws.message.content.serverContent.PlayerResultInfo;
import com.pokerservice.core.domain.Player;
import java.util.List;

public interface GameUseCase {

    void joinGame(long gameId, Player player);

    void betting(MessageType messageType, BetRequestContent content);

    boolean checkReady(long gameId, long playerId);

    void sendFocus(long gameId);

    void settingGame(long gameId);

    void die(long gameId, long playerId);

    void summary(long gameId, List<PlayerResultInfo> playerResultInfos);

    void exitGame(String socketUserId);
}

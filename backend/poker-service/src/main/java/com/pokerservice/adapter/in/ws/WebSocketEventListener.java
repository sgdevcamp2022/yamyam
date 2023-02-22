package com.pokerservice.adapter.in.ws;

import com.pokerservice.core.domain.Player;
import com.pokerservice.core.port.in.GameUseCase;
import java.util.Objects;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.context.event.EventListener;
import org.springframework.messaging.simp.SimpMessageSendingOperations;
import org.springframework.messaging.simp.stomp.StompHeaderAccessor;
import org.springframework.stereotype.Component;
import org.springframework.web.socket.messaging.SessionConnectedEvent;
import org.springframework.web.socket.messaging.SessionDisconnectEvent;
import org.springframework.web.socket.messaging.SessionSubscribeEvent;
import org.springframework.web.socket.messaging.SessionUnsubscribeEvent;

@Component
public class WebSocketEventListener {

    private final SimpMessageSendingOperations messageSendingOperations;
    private static final Logger log = LoggerFactory.getLogger(WebSocketEventListener.class);
    private final GameUseCase gameUseCase;


    public WebSocketEventListener(SimpMessageSendingOperations messageSendingOperations,
        GameUseCase gameUseCase) {
        this.messageSendingOperations = messageSendingOperations;
        this.gameUseCase = gameUseCase;
    }

    @EventListener
    public void handleWebSocketConnectListener(SessionConnectedEvent event) {
        StompHeaderAccessor stompHeaderAccessor = StompHeaderAccessor.wrap(event.getMessage());
        log.info("websocket Connect Success - {}", stompHeaderAccessor.getSessionId());
        log.info("username - {}", stompHeaderAccessor.getUser().getName());

    }

    @EventListener
    public void handleSessionConnectEvent(SessionConnectedEvent event) {
        StompHeaderAccessor stompHeaderAccessor = StompHeaderAccessor.wrap(event.getMessage());
        log.info("session Connect Success - {}", stompHeaderAccessor.getSessionId());
        log.info("username - {}", stompHeaderAccessor.getUser().getName());
    }

    @EventListener
    public void handleWebSocketDisconnectListener(SessionDisconnectEvent event) {
        log.info("Poker Server DisConnect Success");
        StompHeaderAccessor headerAccessor = StompHeaderAccessor.wrap(event.getMessage());
        String sessionId = headerAccessor.getUser().getName();

//        Player player = gameManager.getPlayerInfo(sessionId);
//        gameManager.exitGame(player);
//        gameManager.removePlayer(sessionId);
    }

    @EventListener
    public void handleWebSocketSubscribeListener(SessionSubscribeEvent event) {
        StompHeaderAccessor headerAccessor = StompHeaderAccessor.wrap(event.getMessage());
        String session = Objects.requireNonNull(headerAccessor.getUser()).getName();
        String destination = headerAccessor.getDestination();

        long userId = Long.parseLong(headerAccessor.getNativeHeader("userId").get(0));
        String nickName = headerAccessor.getNativeHeader("nickName").get(0);
        long gameId = Long.parseLong(destination.substring("/topic/game/".length()));

        Player player = Player.create(userId, session, nickName, gameId);
        player.setOperate(messageSendingOperations);
        gameUseCase.joinGame(gameId, player);

        log.info("{} subscribe to {}", session, gameId);
    }

    @EventListener
    public void handleWebSocketUnSubscribeListener(SessionUnsubscribeEvent event) {

    }
}

package com.matchservice.adapter.in.ws;

import com.matchservice.core.domain.Lobby;
import com.matchservice.core.domain.Player;
import com.matchservice.core.domain.message.MatchMessage;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.context.event.EventListener;
import org.springframework.messaging.Message;
import org.springframework.messaging.simp.SimpMessageSendingOperations;
import org.springframework.messaging.simp.stomp.StompHeaderAccessor;
import org.springframework.stereotype.Component;
import org.springframework.web.socket.messaging.SessionConnectedEvent;
import org.springframework.web.socket.messaging.SessionDisconnectEvent;

@Component
public class WebSocketEventListener {

    private final SimpMessageSendingOperations messageSendingOperations;
    private static final Logger log = LoggerFactory.getLogger(WebSocketEventListener.class);
    private final Lobby lobby;

    public WebSocketEventListener(SimpMessageSendingOperations messageSendingOperations,
        Lobby lobby) {
        this.messageSendingOperations = messageSendingOperations;
        this.lobby = lobby;
    }

    @EventListener
    public void handleWebSocketConnectListener(SessionConnectedEvent event) {
        StompHeaderAccessor stompHeaderAccessor = StompHeaderAccessor.wrap(event.getMessage());
        String sessionId = stompHeaderAccessor.getUser().getName();
        Player player = new Player(sessionId, messageSendingOperations);
        log.info("sessionId : {} - add user to lobby", sessionId);
        lobby.addPlayer(sessionId, player);
    }

    @EventListener
    public void handleSessionConnectEvent(SessionConnectedEvent event) {
    }

    @EventListener
    public void handleWebSocketDisconnectListener(SessionDisconnectEvent event) {
        StompHeaderAccessor headerAccessor = StompHeaderAccessor.wrap(event.getMessage());
        String sessionId = headerAccessor.getUser().getName();

        log.info("sessionId: {} - disconnect from lobby", sessionId);
        lobby.removePlayer(sessionId);
    }
}
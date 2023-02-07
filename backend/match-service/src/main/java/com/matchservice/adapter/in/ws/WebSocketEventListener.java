package com.matchservice.adapter.in.ws;

import com.matchservice.core.domain.Lobby;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.context.event.EventListener;
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
        log.info("Received a new web socket connection");
        StompHeaderAccessor headerAccessor = StompHeaderAccessor.wrap(event.getMessage());
    }

    @EventListener
    public void handleSessionConnectEvent(SessionConnectedEvent event) {
        StompHeaderAccessor stompHeaderAccessor = StompHeaderAccessor.wrap(event.getMessage());
        String sessionId = stompHeaderAccessor.getUser().getName();
        lobby.addSender(sessionId, messageSendingOperations);
    }

    @EventListener
    public void handleWebSocketDisconnectListener(SessionDisconnectEvent event) {
        StompHeaderAccessor headerAccessor = StompHeaderAccessor.wrap(event.getMessage());
        String username = (String) headerAccessor.getSessionAttributes().get("username");
        String session = headerAccessor.getUser().getName();

        if (username != null) {
            log.info("User Disconnected : " + username);
            lobby.removeSender(session);
        }
    }
}

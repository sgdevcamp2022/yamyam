package com.matchservice.adapter.out.client;

import com.matchservice.core.domain.match.Match;
import com.matchservice.core.port.out.GameUseCase;
import jakarta.websocket.ClientEndpoint;
import jakarta.websocket.CloseReason;
import jakarta.websocket.ContainerProvider;
import jakarta.websocket.MessageHandler.Whole;
import jakarta.websocket.OnClose;
import jakarta.websocket.OnMessage;
import jakarta.websocket.OnOpen;
import jakarta.websocket.Session;
import jakarta.websocket.WebSocketContainer;
import java.net.URI;
import org.springframework.beans.factory.annotation.Value;

@ClientEndpoint
public class GameWebSocketClient implements GameUseCase {

    private Session session;

    @Value("url.poker-service")
    private String pokerURL;

    @OnOpen
    public void onOpen(Session session) {
        System.out.println("WebSocket opened: " + session.getId());
    }

    @OnMessage
    public void onMessage(String message, Session session) {
        System.out.println("Message received: " + message);
    }

    @OnClose
    public void onClose(Session session, CloseReason closeReason) {
        System.out.println("WebSocket closed: " + closeReason);
    }

    @Override
    public void connectToGame(Match match) {
        WebSocketContainer container = ContainerProvider.getWebSocketContainer();
        try {
            URI uri = new URI("ws://" + pokerURL + "poker-ws");
            session = container.connectToServer(GameWebSocketClient.class, uri);
            subscribe(match.getId());
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    public void subscribe(long gameId) {
        try {
            session.addMessageHandler((Whole<String>) message ->
                System.out.println("Message received: " + message));
            session.getBasicRemote()
                .sendText("SUBSCRIBE\nid:sub-0\ndestination:/pub/sub" + gameId + "\n\n");
        } catch (Exception e) {
            e.printStackTrace();
        }
    }
}

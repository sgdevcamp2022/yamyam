package com.pokerservice.controller.ws;

import com.pokerservice.core.domain.Player;
import java.util.Map;
import java.util.Set;
import java.util.WeakHashMap;
import org.springframework.messaging.Message;
import org.springframework.messaging.simp.SimpMessageHeaderAccessor;
import org.springframework.messaging.simp.SimpMessageSendingOperations;
import org.springframework.messaging.simp.SimpMessageType;
import org.springframework.messaging.simp.SimpMessagingTemplate;
import org.springframework.messaging.support.AbstractMessageChannel;

public class GameMessage {

    private MessageType type;
    private Map<String, Integer> content;
    private String sender;
    private long gameId;
    private int seatNo;
    private float delay;

    public enum MessageType {
        ERROR,
        JOIN, GAME_START,
        DRAW,
        FOCUS, BET, DIE,
        WIN, LOOSE,
        BATTLE,
        LEAVE
    }

    public GameMessage() {
    }

    public GameMessage(MessageType type, Map<String, Integer> content, String sender, int seatNo,
        float delay) {
        this.type = type;
        this.content = content;
        this.sender = sender;
        this.seatNo = seatNo;
        this.delay = delay;
    }

    public static GameMessage createFocusMessage(int focusSeatNo) {
        WeakHashMap<String, Integer> storage = new WeakHashMap<>(1);
        return new GameMessage(MessageType.FOCUS, null, null, focusSeatNo, 0f);
    }

    public static GameMessage createBettingMessage(int betAmount, int currentChip, int seatNo) {
        WeakHashMap<String, Integer> storage = new WeakHashMap<>(1);
        storage.put("betAmount", betAmount);
        storage.put("currentChip", currentChip);

        return new GameMessage(MessageType.BET, storage, null, seatNo, 0f);
    }

    public static GameMessage createDrawMessage(int seatNo, int cardValue) {
        WeakHashMap<String, Integer> storage = new WeakHashMap<>(1);
        storage.put("card", cardValue);

        return new GameMessage(MessageType.DRAW, storage, null, seatNo, 0.5f);
    }

    public static GameMessage createStartMessage() {
        return new GameMessage(MessageType.GAME_START, null, null, 0, 0f);
    }

    public void sendAll(Set<Player> players, String destination) {
        players.forEach(p -> send(p, destination));
    }

    public void send(Player player, String destination) {
        SimpMessageSendingOperations messageTemplate = new SimpMessagingTemplate(
            new AbstractMessageChannel() {
                @Override
                protected boolean sendInternal(Message<?> message, long timeout) {
                    return false;
                }
            });
        SimpMessageHeaderAccessor headerAccessor = SimpMessageHeaderAccessor.create(
            SimpMessageType.MESSAGE);
        headerAccessor.setSessionId(player.getSession());
        headerAccessor.setLeaveMutable(true);
        messageTemplate.convertAndSendToUser(player.getSession(), destination, this);
    }

    public MessageType getType() {
        return type;
    }

    public Map<String, Integer> getContent() {
        return content;
    }

    public void addContent(String key, Integer value) {
        this.content.put(key, value);
    }

    public String getSender() {
        return sender;
    }

    public int getSeatNo() {
        return seatNo;
    }

    public float getDelay() {
        return delay;
    }

    public long getGameId() {
        return gameId;
    }

    @Override
    public String toString() {
        return "GameMessage{" +
            "type=" + type +
            ", content=" + content +
            ", sender='" + sender + '\'' +
            ", seatNo=" + seatNo +
            ", delay=" + delay +
            '}';
    }
}

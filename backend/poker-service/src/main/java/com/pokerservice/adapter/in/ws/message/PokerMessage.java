package com.pokerservice.adapter.in.ws.message;

import com.pokerservice.adapter.in.ws.message.content.BetResponseContent;
import com.pokerservice.adapter.in.ws.message.content.DieContent;
import com.pokerservice.adapter.in.ws.message.content.FocusContent;
import com.pokerservice.adapter.in.ws.message.content.GameStartContent;
import com.pokerservice.adapter.in.ws.message.content.JoinContent;

public class PokerMessage<T> {

    private MessageType type;

    public enum MessageType {
        // CLIENT SEND THIS
        READY,
        DRAW,
        BET, DIE,
        LEAVE,

        // SERVER SEND THIS
        JOIN,
        GAME_START,
        FOCUS,
        BATTLE_RESULT,
        ERROR,
    }

    private T content;
    private double delay;

    private PokerMessage(MessageType type, T content, double delay) {
        this.type = type;
        this.content = content;
        this.delay = delay;
    }

    public static PokerMessage<JoinContent> joinMessage(JoinContent content) {
        return new PokerMessage<>(MessageType.JOIN, content, 0.0);
    }

    public static PokerMessage<GameStartContent> gameStartMessage(GameStartContent content) {
        return new PokerMessage<>(MessageType.GAME_START, content, 0.0);
    }

    public static PokerMessage<FocusContent> focusMessage(FocusContent content) {
        return new PokerMessage<>(MessageType.FOCUS, content, 1.0);
    }

    public static PokerMessage<BetResponseContent> betMessage(BetResponseContent content) {
        return new PokerMessage<>(MessageType.BET, content, 1.0);
    }

    public static PokerMessage<DieContent> dieMessage(DieContent content) {
        return new PokerMessage<>(MessageType.DIE, content, 1.0);
    }

    public MessageType getType() {
        return type;
    }

    public T getContent() {
        return content;
    }

    public double getDelay() {
        return delay;
    }

    @Override
    public String toString() {
        return "PokerMessage{" +
            "type=" + type +
            ", content=" + content +
            ", delay=" + delay +
            '}';
    }
}

package com.matchservice.core.domain;

import java.util.HashMap;
import java.util.Map;

public class MatchMessage {

    private MessageType type;
    private String sender;
    private Map<String, String> content;
    private long userId;

    public MatchMessage() {
    }

    public MatchMessage(MessageType type) {
        this.type = type;
        content = new HashMap<>();
    }

    public enum MessageType {
        MATCH,
        PENALTY,
        MATCH_DONE,
        GAME_START,
        ACCEPT,
        DECLINE,
        ERROR
    }

    public MessageType getType() {
        return type;
    }

    public String getSender() {
        return sender;
    }

    public void addContent(String key, String value) {
        content.put(key, value);
    }

    public Map<String, String> getContent() {
        return content;
    }

    public long getUserId() {
        return userId;
    }
}

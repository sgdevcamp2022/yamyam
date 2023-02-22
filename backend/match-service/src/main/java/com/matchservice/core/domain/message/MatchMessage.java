package com.matchservice.core.domain.message;

import java.util.HashMap;
import java.util.Map;

public class MatchMessage {

    private MessageType type;
    private String sender;
    private long userId;
    private Map<String, String> content;

    public MatchMessage() {
    }

    public MatchMessage(MessageType type) {
        this.type = type;
        content = new HashMap<>();
    }

    public enum MessageType {
        MATCH,
        MATCH_DONE,
        ERROR
    }

    public void addContent(String key, String value) {
        content.put(key, value);
    }

    public void setSender(String sender) {
        this.sender = sender;
    }

    public void setUserId(long userId) {
        this.userId = userId;
    }

    public MessageType getType() {
        return type;
    }

    public String getSender() {
        return sender;
    }

    public long getUserId() {
        return userId;
    }

    public Map<String, String> getContent() {
        return content;
    }
}

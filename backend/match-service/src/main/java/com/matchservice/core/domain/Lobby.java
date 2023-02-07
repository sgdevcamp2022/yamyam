package com.matchservice.core.domain;

import java.util.HashMap;
import java.util.Map;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.config.ConfigurableBeanFactory;
import org.springframework.context.annotation.Scope;
import org.springframework.messaging.simp.SimpMessageSendingOperations;
import org.springframework.stereotype.Component;

@Component
@Scope(value = ConfigurableBeanFactory.SCOPE_SINGLETON)
public class Lobby {

    private static final Logger logger = LoggerFactory.getLogger(Lobby.class);

    private final Map<Integer, Match> matches;

    static private Map<String, SimpMessageSendingOperations> sessionMgr = new HashMap<>();

    static SimpMessageSendingOperations getSender(String sessionid) {
        return sessionMgr.get(sessionid);
    }

    @Autowired
    public Lobby() {
        matches = new HashMap<>();
    }

    public void addSender(String sessionid, SimpMessageSendingOperations sender) {
        sessionMgr.put(sessionid, sender);
    }

    public void removeSender(String sessionid) {
        sessionMgr.remove(sessionid);
    }
}

package com.matchservice.config;

import java.security.Principal;
import java.util.Map;
import java.util.UUID;
import java.util.logging.Logger;
import org.springframework.http.server.ServerHttpRequest;
import org.springframework.web.socket.WebSocketHandler;
import org.springframework.web.socket.server.support.DefaultHandshakeHandler;

public class CustomHandshakeHandler extends DefaultHandshakeHandler {

    private Logger logger = Logger.getLogger("CustomHandshakeHandler");
    // Custom class for storing principal
    @Override
    protected Principal determineUser(ServerHttpRequest request,
                                      WebSocketHandler wsHandler,
                                      Map<String, Object> attributes) {
        // Generate principal with UUID as name
        String sessionId = UUID.randomUUID().toString();
        logger.info("create session id by uuid " + sessionId);
        return new StompPrincipal(sessionId);
    }
}

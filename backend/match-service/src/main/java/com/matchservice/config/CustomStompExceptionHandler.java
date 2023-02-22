package com.matchservice.config;

import org.springframework.messaging.Message;
import org.springframework.messaging.simp.stomp.StompHeaderAccessor;
import org.springframework.web.socket.messaging.StompSubProtocolErrorHandler;

public class CustomStompExceptionHandler extends StompSubProtocolErrorHandler {

    @Override
    public Message<byte[]> handleErrorMessageToClient(Message<byte[]> errorMessage) {
        return super.handleErrorMessageToClient(errorMessage);
    }

    @Override
    protected Message<byte[]> handleInternal(StompHeaderAccessor errorHeaderAccessor,
        byte[] errorPayload, Throwable cause, StompHeaderAccessor clientHeaderAccessor) {
        return super.handleInternal(errorHeaderAccessor, errorPayload, cause, clientHeaderAccessor);
    }

    @Override
    public Message<byte[]> handleClientMessageProcessingError(Message<byte[]> clientMessage,
        Throwable ex) {
        return super.handleClientMessageProcessingError(clientMessage, ex);
    }
}

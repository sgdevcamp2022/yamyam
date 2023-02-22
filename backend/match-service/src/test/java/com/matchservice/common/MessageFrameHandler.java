package com.matchservice.common;

import java.lang.reflect.Type;
import java.util.concurrent.CompletableFuture;
import org.springframework.messaging.simp.stomp.StompFrameHandler;
import org.springframework.messaging.simp.stomp.StompHeaders;

public class MessageFrameHandler<T> implements StompFrameHandler {

    private final CompletableFuture<T> completableFuture = new CompletableFuture<>();

    private final Class<T> tClass;

    public MessageFrameHandler(Class<T> tClass) {
        this.tClass = tClass;
    }

    public CompletableFuture<T> getCompletableFuture() {
        return completableFuture;
    }

    @Override
    public Type getPayloadType(StompHeaders headers) {
        return this.tClass;
    }

    @Override
    public void handleFrame(StompHeaders headers, Object payload) {
        completableFuture.complete((T)payload);
    }
}

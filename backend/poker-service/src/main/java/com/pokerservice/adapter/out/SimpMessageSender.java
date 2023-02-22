package com.pokerservice.adapter.out;

import com.pokerservice.adapter.in.ws.message.PokerMessage;
import com.pokerservice.core.port.out.MessageSendPort;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.messaging.simp.SimpMessageSendingOperations;
import org.springframework.stereotype.Component;

@Component
class SimpMessageSender implements MessageSendPort {

    private static final Logger log = LoggerFactory.getLogger(SimpMessageSender.class);
    private final SimpMessageSendingOperations operations;

    SimpMessageSender(SimpMessageSendingOperations operations) {
        this.operations = operations;
    }

    @Override
    public <T extends PokerMessage> void sendMessage(String destination, T payload) {
        operations.convertAndSend(destination, payload);
    }
}

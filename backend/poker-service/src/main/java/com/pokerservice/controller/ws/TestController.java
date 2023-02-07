package com.pokerservice.controller.ws;

import com.pokerservice.controller.ws.GameMessage.MessageType;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.messaging.handler.annotation.MessageMapping;
import org.springframework.messaging.handler.annotation.Payload;
import org.springframework.messaging.simp.SimpMessageHeaderAccessor;
import org.springframework.messaging.simp.SimpMessageSendingOperations;
import org.springframework.web.bind.annotation.RestController;

@RestController
public class TestController {

    private static final Logger logger = LoggerFactory.getLogger(TestController.class);
    private final SimpMessageSendingOperations sendingOperations;

    public TestController(SimpMessageSendingOperations sendingOperations) {
        this.sendingOperations = sendingOperations;
    }

    @MessageMapping("/test")
    public void test(@Payload GameMessage gameMessage, SimpMessageHeaderAccessor headerAccessor) {
        try {
            headerAccessor.getSessionAttributes()
                .put("ws-session", headerAccessor.getUser().getName());
        } catch (Exception e) {
            sendingOperations.convertAndSend("/sub/public/" + gameMessage.getGameId(),
                new GameMessage(MessageType.ERROR, null, null, 0, 0));
        }

        sendingOperations.convertAndSend("/sub/public/" + gameMessage.getGameId(), gameMessage);
    }
}

package com.pokerservice.controller.ws;

import com.pokerservice.controller.ws.GameMessage.MessageType;
import com.pokerservice.core.domain.User;
import com.pokerservice.core.port.GameEnterUseCase;
import com.pokerservice.core.port.GameUseCase;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.messaging.handler.annotation.MessageMapping;
import org.springframework.messaging.handler.annotation.Payload;
import org.springframework.messaging.simp.SimpMessageHeaderAccessor;
import org.springframework.messaging.simp.SimpMessageSendingOperations;
import org.springframework.web.bind.annotation.RestController;

@RestController
public class GameController {

    private final GameEnterUseCase gameEnterUsecase;
    private final GameUseCase gameUseCase;
    private final SimpMessageSendingOperations sendingOperations;
    private static final Logger log = LoggerFactory.getLogger(GameController.class);

    public GameController(GameEnterUseCase gameEnterUsecase,
        GameUseCase gameUseCase, SimpMessageSendingOperations sendingOperations) {
        this.gameEnterUsecase = gameEnterUsecase;
        this.gameUseCase = gameUseCase;
        this.sendingOperations = sendingOperations;
    }

    @MessageMapping("/enter")
    public void enter(@Payload GameMessage gameMessage, SimpMessageHeaderAccessor headerAccessor) {
        try {
            String sessionId = headerAccessor.getSessionId();
            long userId = (long) gameMessage.getContent().get("userId");

            gameEnterUsecase.enterGame(new User(userId, sessionId, gameMessage.getSender()),
                gameMessage.getGameId());
            headerAccessor.getSessionAttributes()
                .put("ws-session", headerAccessor.getUser().getName());
        } catch (Exception e) {
            sendingOperations.convertAndSend("/sub/public/" + gameMessage.getGameId(),
                new GameMessage(MessageType.ERROR, null, null, 0, 0));
        }

        sendingOperations.convertAndSend("/sub/public/" + gameMessage.getGameId(), gameMessage);
    }


//    @MessageMapping("/action")
//    public void gameAction(@Payload GameMessage gameMessage,
//        SimpMessageHeaderAccessor headerAccessor) {
//        MessageType packet = gameMessage.getType();
//
//        switch (packet) {
//            case GAME_START -> {
//                if (gameUseCase.gameStart(gameMessage.getGameId())) {
//                    sendingOperations.convertAndSend("/sub/public/" + gameMessage.getGameId(),
//                        gameMessage);
//                }
//            }
//
//            case BET -> {
//                gameUseCase.betting(gameMessage.getGameId(),
//                    gameMessage.getContent().get("betAmount"));
//                sendingOperations.convertAndSend("/sub/public/" + gameMessage.getGameId(),
//                    gameMessage);
//            }
//        }
//
//    }
}
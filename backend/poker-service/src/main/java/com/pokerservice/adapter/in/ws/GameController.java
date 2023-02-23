package com.pokerservice.adapter.in.ws;

import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.pokerservice.adapter.in.ws.message.PokerMessage;
import com.pokerservice.adapter.in.ws.message.PokerMessage.MessageType;
import com.pokerservice.adapter.in.ws.message.content.clientContent.BetRequestContent;
import com.pokerservice.adapter.in.ws.message.content.clientContent.DieResponseContent;
import com.pokerservice.adapter.in.ws.message.content.clientContent.ReadyContent;
import com.pokerservice.core.port.in.GameUseCase;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.messaging.handler.annotation.MessageMapping;
import org.springframework.messaging.handler.annotation.Payload;
import org.springframework.messaging.simp.SimpMessageHeaderAccessor;
import org.springframework.web.bind.annotation.RestController;

@RestController
public class GameController {

    private final GameUseCase gameUseCase;
    private static final Logger log = LoggerFactory.getLogger(GameController.class);
    ObjectMapper mapper = new ObjectMapper();

    public GameController(GameUseCase gameUseCase) {
        this.gameUseCase = gameUseCase;
    }

    @MessageMapping("/action")
    public void gameAction(@Payload PokerMessage pokerMessage,
        SimpMessageHeaderAccessor headerAccessor) {
        log.info("pokerMessage = {}", pokerMessage.toString());

        MessageType packet = pokerMessage.getType();

        long gameId = 0;
        long userId = 0;

        switch (packet) {
            case READY -> {
                ReadyContent content = mapTo(pokerMessage.getContent(), ReadyContent.class);
                gameId = content.gameId();
                userId = content.userId();
                boolean allReady = gameUseCase.checkReady(gameId, userId);

                if (allReady) {
                    gameUseCase.settingGame(gameId);
                    gameUseCase.sendFocus(gameId);
                }
            }
            case RAISE, CALL, ALLIN -> {
                BetRequestContent content = mapTo(pokerMessage.getContent(), BetRequestContent.class);
                gameUseCase.betting(pokerMessage.getType(), content);

            }
            case DIE -> {
                DieResponseContent content = mapTo(pokerMessage.getContent(), DieResponseContent.class);
                gameId = content.gameId();
                userId = content.id();
                gameUseCase.die(gameId, userId);
                gameUseCase.sendFocus(gameId);
            }
//            case BATTLE_RESULT -> {
//                gameUseCase.calcGameResult();
//            }
        }
    }

    private <S> S mapTo(Object from, Class<S> to) {
        S content;
        try {
            String json = mapper.writeValueAsString(from);
            content = mapper.readValue(json, to);
        } catch (JsonProcessingException e) {
            throw new RuntimeException(e);
        }

        return content;
    }
}
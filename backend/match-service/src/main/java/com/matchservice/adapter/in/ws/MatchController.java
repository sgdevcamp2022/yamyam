package com.matchservice.adapter.in.ws;

import com.matchservice.core.domain.MatchMessage;
import com.matchservice.core.domain.MatchMessage.MessageType;
import com.matchservice.core.domain.MatchType;
import com.matchservice.core.domain.Player;
import com.matchservice.core.port.in.MatchingUseCase;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.messaging.handler.annotation.MessageMapping;
import org.springframework.messaging.handler.annotation.Payload;
import org.springframework.messaging.simp.SimpMessageHeaderAccessor;
import org.springframework.web.bind.annotation.RestController;

@RestController
public class MatchController {

    private static final Logger log = LoggerFactory.getLogger(MatchController.class);
    private final MatchingUseCase matchingUseCase;

    public MatchController(MatchingUseCase matchingUseCase) {
        this.matchingUseCase = matchingUseCase;
    }

    @MessageMapping("/match")
    public void addUser(@Payload MatchMessage message,
        SimpMessageHeaderAccessor headerAccessor) {

        switch (message.getType()) {
            case ACCEPT -> {
                log.info("Send to Game Server");
                matchingUseCase.accept(
                    new Player(message.getUserId(), headerAccessor.getUser().getName()),
                    Long.parseLong(message.getContent().get("matchId")));
            }

            case DECLINE -> {
                long matchId = Long.parseLong(message.getContent().get("matchId"));

            }

            case MATCH -> {
                MatchType matchType = switch (message.getContent().get("match_type")) {
                    case "2p" -> MatchType.P2;
                    case "4p" -> MatchType.P4;
                    default -> throw new AssertionError("Not Supported Type");
                };

                String sessionId = headerAccessor.getUser().getName();
                Player player = new Player(message.getUserId(), sessionId);
                matchingUseCase.matching(player, matchType);
            }
        }
    }
}

package com.matchservice.adapter.in.ws;

import com.matchservice.core.domain.Lobby;
import com.matchservice.core.domain.Player;
import com.matchservice.core.domain.match.Match.GameType;
import com.matchservice.core.domain.message.MatchMessage;
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
    private final Lobby lobby;

    public MatchController(MatchingUseCase matchingUseCase, Lobby lobby) {
        this.matchingUseCase = matchingUseCase;
        this.lobby = lobby;
    }

    @MessageMapping("/v1/match")
    public void matchAction(@Payload MatchMessage message,
        SimpMessageHeaderAccessor headerAccessor) {

        String sessionId = headerAccessor.getUser().getName();

        switch (message.getType()) {
            case MATCH -> {
                GameType gameType = switch (message.getContent().get("match_type").toLowerCase()) {
                    case "2p" -> GameType.P2;
                    case "4p" -> GameType.P4;
                    default -> throw new AssertionError("Not Supported Type");
                };

                Player player = lobby.getPlayerInfo(sessionId);

                matchingUseCase.matching(player, gameType);
            }
        }
    }
}

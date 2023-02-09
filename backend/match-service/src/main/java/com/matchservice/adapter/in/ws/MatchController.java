package com.matchservice.adapter.in.ws;

import com.matchservice.core.domain.MatchMessage;
import com.matchservice.core.domain.GameType;
import com.matchservice.core.domain.Player;
import com.matchservice.core.port.in.MatchingUseCase;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.Parameter;
import io.swagger.v3.oas.annotations.Parameters;
import io.swagger.v3.oas.annotations.tags.Tag;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.messaging.handler.annotation.MessageMapping;
import org.springframework.messaging.handler.annotation.Payload;
import org.springframework.messaging.simp.SimpMessageHeaderAccessor;
import org.springframework.web.bind.annotation.RestController;

@Tag(name = "match", description = "매칭 API")
@RestController
public class MatchController {

    private static final Logger log = LoggerFactory.getLogger(MatchController.class);
    private final MatchingUseCase matchingUseCase;

    public MatchController(MatchingUseCase matchingUseCase) {
        this.matchingUseCase = matchingUseCase;
    }

    @Operation(summary = "matching", description = "클라이언트가 서버에게 보내는 매칭 관련 websocket 통신")
    @Parameters({
        @Parameter(name = "userId", description = "유저 ID", example = "1"),
        @Parameter(name = "type", description = "메시지 타입", example = "승인 : ACCEPT, 거절 : CANCEL"),
        @Parameter(name = "content", description = "필요 데이터", example = "{matchId: 1}"),

    })
    @MessageMapping("/v1/match")
    public void matchAction(@Payload MatchMessage message,
        SimpMessageHeaderAccessor headerAccessor) {

        switch (message.getType()) {
            case ACCEPT -> {
                log.info("Send to Game Server");
                matchingUseCase.accept(
                    new Player(message.getUserId(), headerAccessor.getUser().getName()),
                    Long.parseLong(message.getContent().get("matchId")));
            }

            case CANCEL -> {
                matchingUseCase.cancel(
                    new Player(message.getUserId(), headerAccessor.getUser().getName()),
                    Long.parseLong(message.getContent().get("matchId"))
                );
            }

            case MATCH -> {
                GameType gameType = switch (message.getContent().get("match_type")) {
                    case "2p" -> GameType.P2;
                    case "4p" -> GameType.P4;
                    default -> throw new AssertionError("Not Supported Type");
                };

                String sessionId = headerAccessor.getUser().getName();
                Player player = new Player(message.getUserId(), sessionId);
                matchingUseCase.matching(player, gameType);
            }
        }
    }
}

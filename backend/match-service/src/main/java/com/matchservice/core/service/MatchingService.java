package com.matchservice.core.service;

import com.matchservice.core.domain.Match;
import com.matchservice.core.domain.MatchMessage;
import com.matchservice.core.domain.MatchMessage.MessageType;
import com.matchservice.core.domain.MatchType;
import com.matchservice.core.domain.Player;
import com.matchservice.core.port.in.MatchingUseCase;
import com.matchservice.core.port.out.MatchPort;
import com.matchservice.core.port.out.MatchQueuePort;
import com.matchservice.core.port.out.PlayerPort;
import java.util.List;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.messaging.simp.SimpMessagingTemplate;
import org.springframework.stereotype.Service;

@Service
public class MatchingService implements MatchingUseCase {

    private final Logger log = LoggerFactory.getLogger(MatchingService.class);
    private final MatchPort matchPort;
    private final MatchQueuePort matchQueuePort;
    private final PlayerPort playerPort;
    private final SimpMessagingTemplate template;

    public MatchingService(MatchPort matchPort, MatchQueuePort matchQueuePort, PlayerPort playerPort,
        SimpMessagingTemplate template) {
        this.matchPort = matchPort;
        this.matchQueuePort = matchQueuePort;
        this.playerPort = playerPort;
        this.template = template;
    }

    @Override
    public void matching(Player player, MatchType matchType) {
        if (playerPort.isExist(player)) {
            player = playerPort.find(player);
        } else {
            playerPort.save(player);
        }

        if (player.isPenaltyUser()) {
            sendForWaitingByPenalty(player);
            return;
        }

        matchQueuePort.save(player, matchType);

        List<Match> matches = matchPort.showMatches(matchType);

        if (matches.isEmpty()) {
            Match match = new Match(matchType);
            Match savedMatch = matchPort.save(match);
            for (int i = 0; i < 4; i++) {
                Player waitingPlayer = matchQueuePort.remove(matchType);
                match.addPlayer(waitingPlayer);
            }

            matchFullCheck(savedMatch);
        } else {
            Match match = matches.get(0);
            match.addPlayer(player);

            matchFullCheck(match);
        }
    }

    private void matchFullCheck(Match match) {
        if (match.isFull()) {
            log.info("===== matching full!");
            MatchMessage message = new MatchMessage(MessageType.MATCH_DONE);
            message.addContent("matchId", "" + match.getId());
            match.sendAll(message);
        }
    }

    private void sendForWaitingByPenalty(Player player) {
        MatchMessage matchMessage = new MatchMessage(MessageType.PENALTY);
        template.convertAndSendToUser(player.getSession(), "/topic/match", matchMessage);
    }

    @Override
    public void accept(Player player, long matchId) {
        Match match = matchPort.findById(matchId);
        match.acceptMatching(player);

        if (match.allAccept()) {
            MatchMessage message = new MatchMessage(MessageType.GAME_START);

            template.convertAndSendToUser(player.getSession(), "/topic/match", message);
        }
    }

    @Override
    public void cancel(Player player, long matchId) {
        Match match = matchPort.findById(matchId);
        match.deletePlayer(player);
    }


}

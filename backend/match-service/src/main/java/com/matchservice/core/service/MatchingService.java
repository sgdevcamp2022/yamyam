package com.matchservice.core.service;

import com.matchservice.adapter.out.client.PokerAPIRestTemplate;
import com.matchservice.core.domain.match.Match;
import com.matchservice.core.domain.match.Match.GameType;
import com.matchservice.core.domain.message.MatchMessage;
import com.matchservice.core.domain.message.MatchMessage.MessageType;
import com.matchservice.core.domain.match.MatchStatus;
import com.matchservice.core.domain.Player;
import com.matchservice.core.port.in.MatchingUseCase;
import com.matchservice.core.port.out.MatchPort;
import com.matchservice.core.port.out.MatchQueuePort;
import com.matchservice.core.port.out.PlayerPort;
import java.util.List;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.stereotype.Service;

@Service
public class MatchingService implements MatchingUseCase {

    private final Logger log = LoggerFactory.getLogger(MatchingService.class);

    private final MatchPort matchPort;
    private final MatchQueuePort matchQueuePort;
    private final PlayerPort playerPort;
    private final PokerAPIRestTemplate pokerTemplate;

    public MatchingService(MatchPort matchPort, MatchQueuePort matchQueuePort,
        PlayerPort playerPort,
        PokerAPIRestTemplate pokerTemplate) {
        this.matchPort = matchPort;
        this.matchQueuePort = matchQueuePort;
        this.playerPort = playerPort;
        this.pokerTemplate = pokerTemplate;
    }

    @Override
    public void matching(Player player, GameType gameType) {
        log.info("request match {}", player);
        player = getOrCreatePlayer(player);

        matchQueuePort.save(player, gameType);
        Match match = getOrCreateMatch(gameType);

        if (fillUser(match)) {
            log.info("=== match success : matchId:{}, matchType: {}, players: {}, matchStatus: {}",
                match.getId(), match.getMatchType(), match.getWaitingPlayers(), match.getMatchStatus());
            long gameId = pokerTemplate.requestMakeGame(gameType);
            match.setId(gameId);
            sendMatchDoneMessage(match);
            match.changeStatus(MatchStatus.DONE);
        }
    }

    private Player getOrCreatePlayer(Player player) {
        if (playerPort.isExist(player)) {
            player = playerPort.find(player);
        } else {
            playerPort.save(player);
        }
        return player;
    }

    private Match getOrCreateMatch(GameType gameType) {
        List<Match> matches = matchPort.showMatches(gameType);

        Match match;
        if (matches.isEmpty()) {
            match = matchPort.save(new Match(gameType));
        } else {
            match = matches.get(0);
        }
        return match;
    }

    private boolean fillUser(Match match) {
        while (matchQueuePort.size(match.getGameType()) != 0) {
            Player waitingPlayer = matchQueuePort.remove(match.getGameType());
            log.info("add {}", waitingPlayer);
            match.addPlayer(waitingPlayer);

            if (match.isFull()) {
                return true;
            }
        }
        return false;
    }

    private void sendMatchDoneMessage(Match match) {
        MatchMessage message = new MatchMessage(MessageType.MATCH_DONE);
        message.addContent("matchId", "" + match.getId());
        match.sendAll(message);
    }
}

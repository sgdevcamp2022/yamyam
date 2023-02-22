package com.matchservice.core.service;

import com.matchservice.core.domain.match.Match;
import com.matchservice.core.domain.match.Match.GameType;
import com.matchservice.core.domain.Player;
import com.matchservice.core.port.in.MatchInfoUseCase;
import com.matchservice.core.port.out.MatchPort;
import com.matchservice.core.port.out.PlayerPort;
import java.util.List;
import org.springframework.stereotype.Service;

@Service
public class MatchInfoService implements MatchInfoUseCase {

    private final PlayerPort playerPort;
    private final MatchPort matchPort;

    public MatchInfoService(PlayerPort playerPort, MatchPort matchPort) {
        this.playerPort = playerPort;
        this.matchPort = matchPort;
    }

    @Override
    public List<Player> showAllPlayers() {
        return playerPort.findAll();
    }

    @Override
    public List<Match> showMatches(GameType gameType) {
        return matchPort.showMatches(gameType);
    }
}

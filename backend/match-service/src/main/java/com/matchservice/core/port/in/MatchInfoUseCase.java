package com.matchservice.core.port.in;

import com.matchservice.core.domain.match.Match;
import com.matchservice.core.domain.match.Match.GameType;
import com.matchservice.core.domain.Player;
import java.util.List;

public interface MatchInfoUseCase {

    List<Player> showAllPlayers();

    List<Match> showMatches(GameType gameType);
}

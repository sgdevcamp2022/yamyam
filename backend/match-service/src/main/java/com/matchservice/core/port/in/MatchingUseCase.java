package com.matchservice.core.port.in;

import com.matchservice.core.domain.match.Match.GameType;
import com.matchservice.core.domain.Player;

public interface MatchingUseCase {

    void matching(Player player, GameType gameType);
}

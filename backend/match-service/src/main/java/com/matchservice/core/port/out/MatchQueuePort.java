package com.matchservice.core.port.out;

import com.matchservice.core.domain.match.Match.GameType;
import com.matchservice.core.domain.Player;

public interface MatchQueuePort {
    void save(Player player, GameType gameType);

    Player remove(GameType gameType);

    long size(GameType gameType);
}

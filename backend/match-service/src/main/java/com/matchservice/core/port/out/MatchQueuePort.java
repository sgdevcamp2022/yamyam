package com.matchservice.core.port.out;

import com.matchservice.core.domain.Match;
import com.matchservice.core.domain.MatchType;
import com.matchservice.core.domain.Player;

public interface MatchQueuePort {
    void save(Player player, MatchType matchType);

    Player remove(MatchType matchType);

    long size(MatchType matchType);
}

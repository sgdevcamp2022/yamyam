package com.matchservice.core.port.in;

import com.matchservice.core.domain.MatchType;
import com.matchservice.core.domain.Player;

public interface MatchingUseCase {

    void matching(Player player, MatchType matchType);

    void accept(Player player, long matchId);

    void cancel(Player player, long matchId);
}

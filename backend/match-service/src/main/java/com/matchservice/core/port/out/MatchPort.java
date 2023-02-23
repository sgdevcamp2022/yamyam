package com.matchservice.core.port.out;

import com.matchservice.core.domain.match.Match;
import com.matchservice.core.domain.match.Match.GameType;
import java.util.List;

public interface MatchPort {

    Match save(Match match);

    Match findById(long id);

    List<Match> showMatches(GameType gameType);

    List<Match> showAll();

    Match remove(long id);

}

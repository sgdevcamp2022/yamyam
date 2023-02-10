package com.matchservice.core.port.out;

import com.matchservice.core.domain.Match;
import com.matchservice.core.domain.MatchType;
import java.util.List;

public interface MatchPort {

    Match save(Match match);

    Match findById(long id);

    List<Match> showMatches(MatchType matchType);

    Match remove(long id);
}

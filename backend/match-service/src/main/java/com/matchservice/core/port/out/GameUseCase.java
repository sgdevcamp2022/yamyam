package com.matchservice.core.port.out;

import com.matchservice.core.domain.match.Match;

public interface GameUseCase {

    void connectToGame(Match match);

}

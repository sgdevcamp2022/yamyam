package com.pokerservice.core.port;

import com.pokerservice.core.domain.GameType;

public interface GameMakeUseCase {

    long makeGame(GameType gameType);
}

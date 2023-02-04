package com.pokerservice.core.port;

import com.pokerservice.core.domain.User;

public interface GameEnterUseCase {

    void enterGame(User user, long gameId);
}

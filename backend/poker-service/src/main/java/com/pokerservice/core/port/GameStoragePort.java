package com.pokerservice.core.port;

import com.pokerservice.core.domain.Game;
import java.util.Set;

public interface GameStoragePort {

    void saveGame(Game game);

    Set<Game> findAllGames();

    Game findGameById(long id);
}

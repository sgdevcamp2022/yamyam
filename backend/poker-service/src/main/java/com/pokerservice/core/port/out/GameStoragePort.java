package com.pokerservice.core.port.out;

import com.pokerservice.core.domain.Game;
import java.util.Set;

public interface GameStoragePort {

    Game saveGame(Game game);

    Set<Game> findAllGames();

    Game findGameById(long id);
}

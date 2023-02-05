package com.pokerservice.core.service;

import com.pokerservice.core.domain.Game;
import com.pokerservice.core.domain.GameType;
import com.pokerservice.core.domain.Player;
import com.pokerservice.core.domain.User;
import com.pokerservice.core.port.GameEnterUseCase;
import com.pokerservice.core.port.GameMakeUseCase;
import com.pokerservice.core.port.GameUseCase;
import com.pokerservice.core.port.GameStoragePort;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.stereotype.Service;

@Service
public class GameService implements GameEnterUseCase, GameMakeUseCase, GameUseCase {

    private final Logger log = LoggerFactory.getLogger(GameService.class);

    private final GameStoragePort storagePort;

    public GameService(GameStoragePort storagePort) {
        this.storagePort = storagePort;
    }

    @Override
    public long makeGame(GameType gameType) {
        Game game = new Game(gameType);
        storagePort.saveGame(game);
        return game.getId();
    }

    @Override
    public void enterGame(User user, long gameId) {
        Game game = storagePort.findGameById(gameId);
        if (game != null) {
            log.info("game find! gameInfo = {}", game);
        }
        Player player = Player.create(user.userId(), user.sessionId(), user.nickname());
        game.enterGame(player);
        game.gameStartOrNot();
    }

    @Override
    public void betting(long gameId, int bettingAmount) {
        Game game = storagePort.findGameById(gameId);
        game.betting();
    }
}

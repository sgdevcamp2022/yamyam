package com.pokerservice.infra;

import com.pokerservice.core.domain.Game;
import com.pokerservice.core.port.GameStoragePort;
import java.util.HashSet;
import java.util.Map;
import java.util.Set;
import java.util.concurrent.ConcurrentHashMap;
import java.util.concurrent.atomic.AtomicLong;
import org.springframework.stereotype.Repository;

@Repository
public class GameInMemoryAdapter implements GameStoragePort {

    private final Map<Long, Game> storage = new ConcurrentHashMap<>();
    private static AtomicLong id = new AtomicLong(1);

    @Override
    public void saveGame(Game game) {
        long currentId = id.getAndIncrement();
        game.setId(currentId);
        storage.put(currentId, game);
    }

    @Override
    public Set<Game> findAllGames() {
        return new HashSet<>(storage.values());
    }

    @Override
    public Game findGameById(long id) {
        return storage.get(id);
    }
}

package com.matchservice.adapter.out.storage;

import com.matchservice.core.domain.Player;
import com.matchservice.core.port.out.PlayerPort;
import java.util.List;
import java.util.Map;
import java.util.concurrent.ConcurrentHashMap;
import org.springframework.stereotype.Repository;

@Repository
public class PlayerInMemoryAdapter implements PlayerPort {

    private static Map<Long, Player> storage = new ConcurrentHashMap<>();

    @Override
    public void save(Player player) {
        storage.put(player.getId(), player);
    }

    @Override
    public void remove(Player player) {
        storage.remove(player);
    }

    @Override
    public Player find(Player player) {
        return storage.get(player.getId());
    }

    @Override
    public List<Player> findAll() {
        return storage.values()
            .stream().toList();
    }

    @Override
    public boolean isExist(Player player) {
        return storage.containsValue(player);
    }
}

package com.matchservice.adapter.out.storage;

import com.matchservice.core.domain.match.Match.GameType;
import com.matchservice.core.domain.Player;
import com.matchservice.core.port.out.MatchQueuePort;
import java.util.Queue;
import java.util.concurrent.ConcurrentLinkedQueue;
import org.springframework.stereotype.Repository;

@Repository
public class MatchQueueInMemoryAdapter implements MatchQueuePort {

    private static Queue<Player> P2Queue = new ConcurrentLinkedQueue<>();
    private static Queue<Player> P4Queue = new ConcurrentLinkedQueue<>();

    @Override
    public void save(Player player, GameType gameType) {
        switch (gameType) {
            case P2 -> P2Queue.add(player);
            case P4 -> P4Queue.add(player);
        };
    }

    @Override
    public Player remove(GameType gameType) {
        return switch (gameType) {
            case P2 -> P2Queue.remove();
            case P4 -> P4Queue.remove();
        };
    }

    @Override
    public long size(GameType gameType) {
        return switch (gameType) {
            case P2 -> P2Queue.size();
            case P4 -> P4Queue.size();
        };
    }
}

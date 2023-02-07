package com.matchservice.adapter.out;

import com.matchservice.core.domain.MatchType;
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
    public void save(Player player, MatchType matchType) {
        switch (matchType) {
            case P2 -> P2Queue.add(player);
            case P4 -> P4Queue.add(player);
        };
    }

    @Override
    public Player remove(MatchType matchType) {
        return switch (matchType) {
            case P2 -> P2Queue.remove();
            case P4 -> P4Queue.remove();
        };
    }

    @Override
    public long size(MatchType matchType) {
        return switch (matchType) {
            case P2 -> P2Queue.size();
            case P4 -> P4Queue.size();
        };
    }
}

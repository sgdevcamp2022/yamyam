package com.matchservice.adapter.out.storage;

import com.matchservice.core.domain.match.Match;
import com.matchservice.core.domain.match.Match.GameType;
import com.matchservice.core.port.out.MatchPort;
import java.util.ArrayList;
import java.util.List;
import java.util.Map;
import java.util.concurrent.ConcurrentHashMap;
import java.util.concurrent.atomic.AtomicLong;
import java.util.stream.Collectors;
import org.springframework.stereotype.Repository;

@Repository
public class MatchInMemoryAdapter implements MatchPort {

    private static Map<Long, Match> storage = new ConcurrentHashMap();
    private static AtomicLong idFlag = new AtomicLong(0);

    @Override
    public Match save(Match match) {
        long currentId = idFlag.getAndIncrement();
        match.setId(currentId);
        storage.put(currentId, match);
        return match;
    }

    @Override
    public Match findById(long id) {
        if (storage.get(id) == null) {
            throw new IllegalArgumentException("match not exist");
        }
        return storage.get(id);
    }

    @Override
    public List<Match> showMatches(GameType gameType) {
        return storage.values()
            .stream()
            .filter(match -> match.getMatchType() == gameType)
            .sorted()
            .collect(Collectors.toList());
    }

    @Override
    public List<Match> showAll() {
        return new ArrayList<>(storage.values());
    }

    @Override
    public Match remove(long id) {
        return storage.remove(id);
    }

}

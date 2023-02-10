package com.matchservice.adapter.out;

import com.matchservice.core.domain.Match;
import com.matchservice.core.domain.MatchType;
import com.matchservice.core.port.out.MatchPort;
import java.util.Comparator;
import java.util.List;
import java.util.Map;
import java.util.Queue;
import java.util.concurrent.ConcurrentHashMap;
import java.util.concurrent.ConcurrentLinkedQueue;
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
        return storage.get(id);
    }

    @Override
    public List<Match> showMatches(MatchType matchType) {
        return storage.values()
            .stream()
            .filter(match -> match.getMatchType() == matchType)
            .sorted()
            .collect(Collectors.toList());
    }

    @Override
    public Match remove(long id) {
        return storage.remove(id);
    }

}

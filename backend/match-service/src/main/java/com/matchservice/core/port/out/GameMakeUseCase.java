package com.matchservice.core.port.out;


import com.fasterxml.jackson.core.JsonProcessingException;
import com.matchservice.core.domain.match.Match.GameType;

public interface GameMakeUseCase {

    long requestMakeGame(GameType gameType) throws JsonProcessingException;
}

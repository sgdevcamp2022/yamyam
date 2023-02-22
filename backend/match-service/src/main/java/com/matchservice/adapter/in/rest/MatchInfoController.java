package com.matchservice.adapter.in.rest;

import com.matchservice.core.domain.match.Match;
import com.matchservice.core.domain.match.Match.GameType;
import com.matchservice.core.domain.Player;
import com.matchservice.core.port.in.MatchInfoUseCase;
import java.util.List;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RestController;

@RestController
public class MatchInfoController {

    private final MatchInfoUseCase matchInfoUseCase;

    public MatchInfoController(MatchInfoUseCase matchInfoUseCase) {
        this.matchInfoUseCase = matchInfoUseCase;
    }

    @GetMapping("/match/players")
    public ResponseEntity<List<Player>> players() {
        return ResponseEntity.ok(
            matchInfoUseCase.showAllPlayers()
        );
    }

    @GetMapping("/match/matches/{gameType}")
    public ResponseEntity<List<Match>> matches(@PathVariable GameType gameType) {
        return ResponseEntity.ok(
            matchInfoUseCase.showMatches(gameType)
        );
    }
}

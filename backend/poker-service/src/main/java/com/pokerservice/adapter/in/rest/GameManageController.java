package com.pokerservice.adapter.in.rest;

import com.pokerservice.core.domain.GameType;
import com.pokerservice.core.port.in.GameMakeUseCase;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RestController;

@RestController
public class GameManageController {

    private final GameMakeUseCase gameMakeUseCase;

    public GameManageController(GameMakeUseCase gameMakeUseCase) {
        this.gameMakeUseCase = gameMakeUseCase;
    }


    @PostMapping("/game")
    public ResponseEntity<Long> makeGame(String gameType) {
        String key = gameType.toUpperCase();
        System.out.println("gameType = " + key);
        return ResponseEntity.ok(gameMakeUseCase.makeGame(GameType.valueOf(key)));
    }
}

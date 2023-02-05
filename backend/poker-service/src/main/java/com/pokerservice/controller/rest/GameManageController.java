package com.pokerservice.controller.rest;

import com.pokerservice.core.domain.GameType;
import com.pokerservice.core.port.GameMakeUseCase;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
public class GameManageController {

    private final GameMakeUseCase gameMakeUseCase;

    public GameManageController(GameMakeUseCase gameMakeUseCase) {
        this.gameMakeUseCase = gameMakeUseCase;
    }


    @PostMapping("/game")
    public ResponseEntity<Long> makeGame(GameType gameType) {
        return ResponseEntity.ok(gameMakeUseCase.makeGame(gameType));
    }

}

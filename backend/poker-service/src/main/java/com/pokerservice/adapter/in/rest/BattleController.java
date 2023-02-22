package com.pokerservice.adapter.in.rest;

import com.pokerservice.adapter.in.rest.dto.GameResult;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
public class BattleController {

    private static final Logger logger = LoggerFactory.getLogger(BattleController.class);
//    private final BattleService battleService;



    @PostMapping("/game/battle")
    public void getGameInfo(GameResult gameResult) {

    }
}

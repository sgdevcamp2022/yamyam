package com.pokerservice.controller.rest;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RestController;

@RestController
public class GameInfoController {

    private static final Logger logger = LoggerFactory.getLogger(GameInfoController.class);

    @GetMapping("/gameInfo/{gameId}")
    public void getGameInfo(@PathVariable long gameId) {

    }

    @GetMapping("/gameInfo/players/{gameId}")
    public void getPlayers(@PathVariable long gameId) {

    }

    @GetMapping("/gameInfo/ping/{gameId}")
    public void ping(@PathVariable long gameId) {
    }
}

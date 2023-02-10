package com.pokerservice.core.service;

import com.pokerservice.controller.ws.GameMessage;

public record SessionMessage(
    GameMessage gameMessage,
    String session,
    String name
) {

}

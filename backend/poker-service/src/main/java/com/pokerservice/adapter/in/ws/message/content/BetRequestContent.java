package com.pokerservice.adapter.in.ws.message.content;

public record BetRequestContent(
    long userId,
    long gameId,
    int betAmount
) {

}

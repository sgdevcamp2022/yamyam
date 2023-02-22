package com.pokerservice.adapter.in.ws.message.content.clientContent;

public record BetRequestContent(
    long userId,
    long gameId,
    int betAmount
) {

}

package com.pokerservice.adapter.in.ws.message.content;

public record BetResponseContent(
    long gameId,
    long playerId,
    int betAmount,
    int currentAmount,
    int totalAmount
) {
}

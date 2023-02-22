package com.pokerservice.adapter.in.ws.message.content;

public record PlayerInfo(
    long id,
    int currentChip,
    int card) {
}

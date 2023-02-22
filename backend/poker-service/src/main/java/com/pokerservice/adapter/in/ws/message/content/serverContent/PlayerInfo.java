package com.pokerservice.adapter.in.ws.message.content.serverContent;

public record PlayerInfo(
    long id,
    int currentChip,
    int card) {
}

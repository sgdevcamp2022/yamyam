package com.pokerservice.adapter.in.ws.message.content.serverContent;

public record PlayerResultInfo(
    long id,
    int result,  // 1: 승리, 0: 패배
    int currentChip
) {

}

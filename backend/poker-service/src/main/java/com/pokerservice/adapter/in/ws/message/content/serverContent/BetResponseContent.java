package com.pokerservice.adapter.in.ws.message.content.serverContent;

public record BetResponseContent(
    long playerId,
    int betAmount,
    int currentAmount,  // 플레이어 소지 칩 개수
    int totalAmount     // 모든 플레이어가 배팅 전체 칩 개수
) {
}

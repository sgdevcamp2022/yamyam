package com.pokerservice.adapter.in.ws.message.content.serverContent;

import java.util.List;

public record GameStartContent(
    int totalBetAmount,
    List<PlayerInfo> playerInfos) {

}

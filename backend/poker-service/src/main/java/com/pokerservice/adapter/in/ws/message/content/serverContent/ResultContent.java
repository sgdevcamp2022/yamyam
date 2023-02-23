package com.pokerservice.adapter.in.ws.message.content.serverContent;

import java.util.List;

public record ResultContent(
    List<PlayerResultInfo> playerInfo) {
}

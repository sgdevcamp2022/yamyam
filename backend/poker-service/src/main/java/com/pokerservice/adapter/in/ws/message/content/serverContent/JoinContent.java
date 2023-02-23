package com.pokerservice.adapter.in.ws.message.content.serverContent;

public record JoinContent(long userId,
                          String nickname,
                          int order) {
}

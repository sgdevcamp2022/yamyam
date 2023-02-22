package com.pokerservice.adapter.in.ws.message.content;

public record JoinContent(long userId,
                          String nickname,
                          int order) {
}

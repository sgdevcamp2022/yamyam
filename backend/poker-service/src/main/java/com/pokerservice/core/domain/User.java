package com.pokerservice.core.domain;

public record User(
    long userId,
    String sessionId,
    String nickname
)  {

}

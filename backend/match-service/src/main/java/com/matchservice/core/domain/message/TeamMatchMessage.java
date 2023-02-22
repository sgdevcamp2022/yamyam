package com.matchservice.core.domain.message;

import com.matchservice.core.domain.Team;

public class TeamMatchMessage extends MatchMessage{

    private Team team;

    public TeamMatchMessage(MessageType type, Team team) {
        super(type);
        this.team = team;
    }
}

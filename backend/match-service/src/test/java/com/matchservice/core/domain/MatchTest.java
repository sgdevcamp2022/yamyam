package com.matchservice.core.domain;

import static org.assertj.core.api.Assertions.*;
import static org.junit.jupiter.api.Assertions.*;

import java.util.List;
import java.util.stream.Stream;
import org.junit.jupiter.api.Test;

class MatchTest {

    @Test
    void compare_moreWaitPlayer_WillFirst(){
        // given
        Match match1 = new Match(MatchType.P4);
        Match match2 = new Match(MatchType.P4);
        Match match3 = new Match(MatchType.P4);
        Match match4 = new Match(MatchType.P4);

        Player testPlayer1 = new Player(1, "test");
        Player testPlayer2 = new Player(2, "test");
        Player testPlayer3 = new Player(3, "test");
        Player testPlayer4 = new Player(4, "test");

        match1.addPlayer(testPlayer1);
        match1.addPlayer(testPlayer2);
        match1.addPlayer(testPlayer3);

        match2.addPlayer(testPlayer1);

        match3.addPlayer(testPlayer1);
        match3.addPlayer(testPlayer2);

        match4.addPlayer(testPlayer1);
        match4.addPlayer(testPlayer2);
        match4.addPlayer(testPlayer3);
        match4.addPlayer(testPlayer4);

        List<Match> matches = List.of(match1, match2, match3, match4);

        // when
        List<Match> sortResult = matches
            .stream()
            .sorted()
            .toList();

        // then
        assertThat(sortResult.get(0)).isEqualTo(match4);
        assertThat(sortResult.get(1)).isEqualTo(match1);
        assertThat(sortResult.get(2)).isEqualTo(match3);
        assertThat(sortResult.get(3)).isEqualTo(match2);
    }
}
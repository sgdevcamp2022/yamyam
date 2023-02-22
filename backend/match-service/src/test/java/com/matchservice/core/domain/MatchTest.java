package com.matchservice.core.domain;

import static org.assertj.core.api.Assertions.assertThat;

import com.matchservice.core.domain.match.Match;
import com.matchservice.core.domain.match.Match.GameType;
import java.util.List;
import org.junit.jupiter.api.Test;
import org.springframework.messaging.Message;
import org.springframework.messaging.simp.SimpMessagingTemplate;
import org.springframework.messaging.support.AbstractMessageChannel;

class MatchTest {
    private static class TestMessageTemplate extends SimpMessagingTemplate {

        public TestMessageTemplate() {
            super(new AbstractMessageChannel() {
                @Override
                protected boolean sendInternal(Message<?> message, long timeout) {
                    return false;
                }
            });
        }
    }
    @Test
    void compare_moreWaitPlayer_WillFirst(){
        // given
        Match match1 = new Match(GameType.P4);
        Match match2 = new Match(GameType.P4);
        Match match3 = new Match(GameType.P4);
        Match match4 = new Match(GameType.P4);

        Player testPlayer1 = new Player(1, "test", new TestMessageTemplate());
        Player testPlayer2 = new Player(2, "test", new TestMessageTemplate());
        Player testPlayer3 = new Player(3, "test", new TestMessageTemplate());
        Player testPlayer4 = new Player(4, "test", new TestMessageTemplate());

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
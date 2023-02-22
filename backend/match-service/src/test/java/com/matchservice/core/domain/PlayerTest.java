package com.matchservice.core.domain;

import org.assertj.core.api.Assertions;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;
import org.springframework.messaging.Message;
import org.springframework.messaging.MessageChannel;
import org.springframework.messaging.simp.SimpMessagingTemplate;
import org.springframework.messaging.support.AbstractMessageChannel;

class PlayerTest {

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

//    @DisplayName("같은 userId을 가지면 같은 플레이어로 취급한다.")
//    @Test
//    void equals_sameUserId_willTrue() {
//        // given
//        Player player1 = new Player(1L, "session", new SimpMessagingTemplate(
//            new AbstractMessageChannel() {
//                @Override
//                protected boolean sendInternal(Message<?> message, long timeout) {
//                    return false;
//                }
//            }));
//        Player player2 = new Player(1L, "session2", new TestMessageTemplate());
//        Player player3 = new Player(2L, "session", new TestMessageTemplate());
//        // when then
//        Assertions.assertThat(player1.equals(player2)).isTrue();
//        Assertions.assertThat(player1.equals(player3)).isFalse();
//    }
}
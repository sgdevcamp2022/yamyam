package com.pokerservice.core.domain;

import java.util.List;
import org.assertj.core.api.Assertions;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Nested;
import org.junit.jupiter.api.Test;

class GameTest {

    @Nested
    public class EnterGame_Context {

        @DisplayName("게임의 여석이 있는 경우에는 게임 입장이 가능하다.")
        @Test
        void game_notFull_success() {
            // given
            Game game = new Game(1L, GameType.P2);
            Player player1 = Player.create(1L, "testSession", "nickname", 1);
            Player player2 = Player.create(2L, "testSession2", "nickname2", 2);

            // when
            game.join(player1);
            game.join(player2);

            // then
        }

        @DisplayName("게임의 여석이 없는 경우에는 게임 입장이 불가능하다")
        @Test
        void game_already_full_fail() {
            // given
            Game game = new Game(1L, GameType.P2);
            Player player1 = Player.create(1L, "testSession", "nickname", 1);
            Player player2 = Player.create(2L, "testSession2", "nickname2", 2);
            Player player3 = Player.create(3L, "testSession3", "nickname3", 3);

            // when
            game.join(player1);
            game.join(player2);

            // then
            Assertions.assertThatThrownBy(() -> game.join(player3))
                .isInstanceOf(AssertionError.class);
        }

        @DisplayName("게임 입장 시 플레이어의 순서가 결정된다.")
        @Test
        void after_enter_order_set() {
            // given
            Game game = new Game(1L, GameType.P4);
            Player player1 = Player.create(1L, "testSession", "nickname", 1);
            Player player2 = Player.create(2L, "testSession2", "nickname2", 2);
            Player player3 = Player.create(3L, "testSession3", "nickname3", 3);
            Player player4 = Player.create(4L, "testSession4", "nickname4", 4);

            // when
            game.join(player2);
            game.join(player3);
            game.join(player1);
            game.join(player4);

            List<Player> players = game.getPlayers();

            // then
            Assertions.assertThat(players).containsExactly(
                player2, player3, player1, player4
            );

            Assertions.assertThat(players.get(0).getOrder()).isSameAs(0);
            Assertions.assertThat(players.get(1).getOrder()).isSameAs(1);
            Assertions.assertThat(players.get(2).getOrder()).isSameAs(2);
            Assertions.assertThat(players.get(3).getOrder()).isSameAs(3);
        }

    }

    @Nested
    public class DrawCard_Context {

        @DisplayName("일반적인 경우에 카드를 성공적으로 뽑는다.")
        @Test
        void drawCard_normal_success() {
            // given
            Game game = new Game(1L, GameType.P4);
            Player player1 = Player.create(1L, "testSession", "nickname", 1);
            Player player2 = Player.create(2L, "testSession2", "nickname2", 2);
            Player player3 = Player.create(3L, "testSession3", "nickname3", 3);
            Player player4 = Player.create(4L, "testSession4", "nickname4", 4);

            // when
            game.join(player2);
            game.join(player3);
            game.join(player1);
            game.join(player4);

            game.drawCard();

            List<Player> players = game.getPlayers();

            for (Player player : players) {
                Assertions.assertThat(player.getCard()).isNotSameAs(0);
            }
        }
    }

    @Nested
    public class Betting_Context {

        @DisplayName("플레이어는 배팅을 할 수 있다.")
        @Test
        void betting_normal_success() {
            // given
            Game game = new Game(1L, GameType.P4);
            Player player1 = Player.create(1L, "testSession", "nickname", 1);
            Player player2 = Player.create(2L, "testSession2", "nickname2", 2);
            Player player3 = Player.create(3L, "testSession3", "nickname3", 3);
            Player player4 = Player.create(4L, "testSession4", "nickname4", 4);

            // when
            game.join(player2);
            game.join(player3);
            game.join(player1);
            game.join(player4);

            game.betting(player2.getGameId(), 10);
            game.betting(player3.getGameId(), 10);
            game.betting(player1.getGameId(), 10);
            game.betting(player4.getGameId(), 10);
        }

        @DisplayName("최소 배팅금액보다 적게 배팅할 수 없다.")
        @Test
        void betting_under_minBetAmount_fail() {
            // given
            Game game = new Game(1L, GameType.P4);
            Player player1 = Player.create(1L, "testSession", "nickname", 1);
            Player player2 = Player.create(2L, "testSession2", "nickname2", 2);
            Player player3 = Player.create(3L, "testSession3", "nickname3", 3);
            Player player4 = Player.create(4L, "testSession4", "nickname4", 4);

            // when
            game.join(player2);
            game.join(player3);
            game.join(player1);
            game.join(player4);

            Assertions.assertThatThrownBy(() -> game.betting(player2.getGameId(), 5))
                .isInstanceOf(IllegalArgumentException.class)
                .hasMessage("최소 배팅금액보다 적게 배팅할 수 없습니다.");
        }

        @DisplayName("소지 금액보다 많이 배팅할 수 없다.")
        @Test
        void betting_more_currentChip_fail() {
            // given
            Game game = new Game(1L, GameType.P4);
            Player player1 = Player.create(1L, "testSession", "nickname", 1);

            // when
            game.join(player1);

            Assertions.assertThatThrownBy(() -> game.betting(player1.getGameId(), 150))
                .isInstanceOf(IllegalArgumentException.class)
                .hasMessage("소지한 금액보다 더 많은 금액을 배팅할 수 없습니다.");
        }

        @DisplayName("'소지 칩 개수'가 '최소 배팅 칩 개수'보다 작으면 올인을 한다.")
        @Test
        void currentChip_under_minBetAmount_allIn() {
            // given
            Game game = new Game(1L, GameType.P4);
            Player player1 = Player.create(1L, "testSession", "nickname", 1);

            // when
            game.join(player1);
            game.betting(player1.getId(), 95);
            game.betting(player1.getId(), 10);

            Assertions.assertThat(game.getTotalBetAmount()).isSameAs(100);
            Assertions.assertThat(player1.getChip()).isSameAs(0);
            Assertions.assertThat(player1.getCurrentBetAmount()).isSameAs(100);

        }
    }

    @Nested
    public class Focus_Context {

        @DisplayName("현재 순서인 유저 정보를 반환한다.")
        @Test
        void focus_normal_success() {
            // given
            Game game = new Game(1L, GameType.P4);
            Player player1 = Player.create(1L, "testSession", "nickname", 1);
            Player player2 = Player.create(2L, "testSession2", "nickname2", 2);
            Player player3 = Player.create(3L, "testSession3", "nickname3", 3);
            Player player4 = Player.create(4L, "testSession4", "nickname4", 4);

            // when
            game.join(player2);
            game.join(player3);
            game.join(player1);
            game.join(player4);

            Player focusPlayer = game.focus();

            // then
            Assertions.assertThat(player2).isEqualTo(focusPlayer);
        }
    }

    @Nested
    public class NextTurn_Context {

        @DisplayName("일반적으로 다음 턴으로 넘긴다.")
        @Test
        void nextTurn_success(){
            // given
            Game game = new Game(1L, GameType.P4);
            Player player1 = Player.create(1L, "testSession", "nickname", 1);
            Player player2 = Player.create(2L, "testSession2", "nickname2", 2);
            Player player3 = Player.create(3L, "testSession3", "nickname3", 3);
            Player player4 = Player.create(4L, "testSession4", "nickname4", 4);

            // when
            game.join(player2);
            game.join(player3);
            game.join(player1);
            game.join(player4);

            // then
            for (int i = 0; i <= 3; i++) {
                int currentTurn = game.getCurrentTurn();
                Assertions.assertThat(currentTurn).isSameAs(i);
                game.nextTurn();
            }
        }

        @DisplayName("한바퀴 돌면 다시 0으로 이동한다")
        @Test
        void nextTurn_newTurn_success(){
            // given
            Game game = new Game(1L, GameType.P4);
            Player player1 = Player.create(1L, "testSession", "nickname", 1);
            Player player2 = Player.create(2L, "testSession2", "nickname2", 2);
            Player player3 = Player.create(3L, "testSession3", "nickname3", 3);
            Player player4 = Player.create(4L, "testSession4", "nickname4", 4);

            // when
            game.join(player2);
            game.join(player3);
            game.join(player1);
            game.join(player4);

            // then
            for (int i = 0; i <= 3; i++) {
                int currentTurn = game.getCurrentTurn();
                Assertions.assertThat(currentTurn).isSameAs(i);
                game.nextTurn();
            }

            int currentTurn = game.getCurrentTurn();
            Assertions.assertThat(currentTurn).isSameAs(0);
        }

        @DisplayName("만약 해당 순서의 유저가 나가면 다음 유저로 이동한다")
        @Test
        void nextTurn__success(){
            // given
            Game game = new Game(1L, GameType.P4);
            Player player1 = Player.create(1L, "testSession", "nickname", 1);
            Player player2 = Player.create(2L, "testSession2", "nickname2", 2);
            Player player3 = Player.create(3L, "testSession3", "nickname3", 3);
            Player player4 = Player.create(4L, "testSession4", "nickname4", 4);

            // when
            game.join(player2);
            game.join(player3);
            game.join(player1);
            game.join(player4);

            game.exitGame(player1);

            // then
            game.nextTurn();
            game.nextTurn();

            int currentTurn = game.getCurrentTurn();
            Assertions.assertThat(currentTurn).isSameAs(3);

        }
    }

}
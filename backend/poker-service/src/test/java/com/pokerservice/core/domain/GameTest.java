package com.pokerservice.core.domain;

import java.util.List;
import org.assertj.core.api.Assertions;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Nested;
import org.junit.jupiter.api.Test;

class GameTest {

    @DisplayName("enterGame()은")
    @Nested
    public class EnterGame_Context {

        @DisplayName("게임의 여석이 있는 경우에는 게임 입장이 가능하다.")
        @Test
        void game_notFull_willSuccess() {
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
        void game_already_full_willFail() {
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

    @DisplayName("drawCard()은")
    @Nested
    public class DrawCard_Context {

        @DisplayName("일반적인 경우에 카드를 성공적으로 뽑는다.")
        @Test
        void drawCard_normal_willSuccess() {
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

    @DisplayName("raise()은")
    @Nested
    public class Raise_Context {

        @DisplayName("플레이어는 raise을 할 수 있다.")
        @Test
        void raise_normal_willSuccess() {
            // given
            Game game = new Game(1L, GameType.P4);
            Player player = Player.create(1L, "testSession", "nickname", 1);
            int beforeChip = player.getChip();

            // when
            game.join(player);

            game.raise(player.getId(), 10);

            // then
            Assertions.assertThat(player.getStatus()).isSameAs(PlayerStatus.RAISE);
            Assertions.assertThat(player.getCurrentBetAmount()).isSameAs(10);
            Assertions.assertThat(player.getChip()).isSameAs(beforeChip - 10);
            Assertions.assertThat(game.getTotalBetAmount()).isSameAs(10);
            Assertions.assertThat(game.getLastRaiseIndex()).isSameAs(0);
        }

        @DisplayName("최소 배팅금액보다 적게 배팅할 수 없다.")
        @Test
        void raise_under_minBetAmount_willFail() {
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

            game.raise(player2.getId(), 10);

            // then
            Assertions.assertThatThrownBy(() -> game.raise(player2.getId(), 5))
                .isInstanceOf(IllegalArgumentException.class)
                .hasMessage("최소 배팅금액보다 적게 배팅할 수 없습니다.");
            Assertions.assertThat(game.getTotalBetAmount()).isSameAs(player2.getCurrentBetAmount());
            Assertions.assertThat(game.getLastRaiseIndex()).isSameAs(0);
        }

        @DisplayName("소지 금액보다 많이 배팅할 수 없다.")
        @Test
        void raise_more_currentChip_willFail() {
            // given
            Game game = new Game(1L, GameType.P4);
            Player player1 = Player.create(1L, "testSession", "nickname", 1);

            // when
            game.join(player1);

            Assertions.assertThatThrownBy(() -> game.raise(player1.getId(), 150))
                .isInstanceOf(IllegalArgumentException.class)
                .hasMessage("소지 칩 수보다 많이 배팅할 수 없습니다.");
            Assertions.assertThat(game.getTotalBetAmount()).isSameAs(0);
            Assertions.assertThat(game.getLastRaiseIndex()).isSameAs(-1);
        }

        @DisplayName("PLAYING or RAISE 상태가 아닌 유저는 RAISE가 불가능하다.")
        @Test
        void raise_no_player_raise_status_willFail() {
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

            game.die(player2.getId());
            game.raise(player3.getId(), 10);
            game.call(player1.getId());
            game.call(player4.getId());

            // then
            Assertions.assertThat(player3.getStatus()).isSameAs(PlayerStatus.RAISE);
            Assertions.assertThat(player1.getStatus()).isSameAs(PlayerStatus.CALL);
            Assertions.assertThat(player4.getStatus()).isSameAs(PlayerStatus.CALL);
            Assertions.assertThatThrownBy(() -> game.call(player2.getId()))
                .isInstanceOf(IllegalStateException.class);
            Assertions.assertThat(game.getTotalBetAmount()).isSameAs(player3.getCurrentBetAmount() * 3);
            Assertions.assertThat(game.getLastRaiseIndex()).isSameAs(1);
        }

        @DisplayName("RAISE 시 최소 배팅 금액이 상승한다.")
        @Test
        void raise_minBetAmount_willIncrease() {
            // given
            Game game = new Game(1L, GameType.P4);
            Player player1 = Player.create(1L, "testSession", "nickname", 1);
            Player player2 = Player.create(2L, "testSession2", "nickname2", 2);
            Player player3 = Player.create(3L, "testSession3", "nickname3", 3);
            Player player4 = Player.create(4L, "testSession4", "nickname4", 4);

            int player1BeforeChip = player1.getChip();

            // when
            game.join(player2);
            game.join(player3);
            game.join(player1);
            game.join(player4);

            game.raise(player2.getId(), 10);
            game.raise(player3.getId(), 20);
            game.call(player1.getId());

            // then
            Assertions.assertThat(player2.getStatus()).isSameAs(PlayerStatus.RAISE);
            Assertions.assertThat(game.getMinBetAmount()).isSameAs(20);

            Assertions.assertThat(player3.getStatus()).isSameAs(PlayerStatus.RAISE);

            Assertions.assertThat(player1.getStatus()).isSameAs(PlayerStatus.CALL);
            Assertions.assertThat(player1.getChip()).isSameAs(player1BeforeChip - 20);

            Assertions.assertThatThrownBy(() -> game.raise(player4.getId(), 10))
                .isInstanceOf(IllegalArgumentException.class)
                .hasMessage("최소 배팅금액보다 적게 배팅할 수 없습니다.");
        }
    }

    @DisplayName("call()은")
    @Nested
    public class Call_Context {

        @DisplayName("플레이어는 call을 할 수 있다.")
        @Test
        void call_normal_willSuccess() {

            // given
            Game game = new Game(1L, GameType.P4);
            Player player1 = Player.create(1L, "testSession", "nickname", 1);
            Player player2 = Player.create(2L, "testSession", "nickname", 1);
            int player2BeforeChip = player2.getChip();

            // when
            game.join(player1);
            game.join(player2);

            game.raise(player1.getId(), 20);
            game.call(player2.getId());

            // then
            Assertions.assertThat(player2.getStatus()).isSameAs(PlayerStatus.CALL);
            Assertions.assertThat(player2.getCurrentBetAmount()).isSameAs(20);
            Assertions.assertThat(player2.getChip()).isSameAs(player2BeforeChip - 20);
        }

        @DisplayName("RAISE or CALL 상태가 아닌 유저는 CALL을 할 수 없다.")
        @Test
        void call_user_is_DieStatus_willFail() {
            // given
            Game game = new Game(1L, GameType.P4);
            Player player1 = Player.create(1L, "testSession", "nickname", 1);
            Player player2 = Player.create(2L, "testSession2", "nickname2", 2);
            Player player3 = Player.create(3L, "testSession3", "nickname3", 3);
            Player player4 = Player.create(4L, "testSession4", "nickname4", 4);

            // when
            game.join(player1);
            game.join(player2);
            game.join(player3);
            game.join(player4);

            game.die(player1.getId());
            game.raise(player2.getId(), 10);
            game.call(player3.getId());
            game.call(player4.getId());

            // then
            Assertions.assertThatThrownBy(() -> game.call(player1.getId()))
                .isInstanceOf(IllegalStateException.class);
        }

        @DisplayName("소지 금액보다 많이 배팅할 수 없다.")
        @Test
        void raise_more_currentChip_willFail() {
            // given
            Game game = new Game(1L, GameType.P4);
            Player player1 = Player.create(1L, "testSession", "nickname", 1);

            // when
            game.join(player1);

            Assertions.assertThatThrownBy(() -> game.raise(player1.getId(), 150))
                .isInstanceOf(IllegalArgumentException.class)
                .hasMessage("소지 칩 수보다 많이 배팅할 수 없습니다.");
        }

        @DisplayName("Raise는 PLAYING이나, RAISE 상태에만 가능하다.")
        @Test
        void raise_no_player_raise_status_willFail() {
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

            game.die(player2.getId());
            game.raise(player3.getId(), 10);
            game.call(player1.getId());
            game.call(player4.getId());

            // then
            Assertions.assertThat(player3.getStatus()).isSameAs(PlayerStatus.RAISE);
            Assertions.assertThat(player1.getStatus()).isSameAs(PlayerStatus.CALL);
            Assertions.assertThat(player4.getStatus()).isSameAs(PlayerStatus.CALL);
            Assertions.assertThatThrownBy(() -> game.call(player2.getId()))
                .isInstanceOf(IllegalStateException.class);
        }
    }

    @DisplayName("allIn()은")
    @Nested
    public class AllIn_Context {

        @DisplayName("플레이어는 AllIn을 할 수 있다.")
        @Test
        void allIn_normal_willSuccess() {

            // given
            Game game = new Game(1L, GameType.P4);
            Player player1 = Player.create(1L, "testSession", "nickname", 1);
            int beforePlayer1Chip = player1.getChip();
            // when
            game.join(player1);

            game.allIn(player1.getId());

            // then
            Assertions.assertThat(player1.getStatus()).isSameAs(PlayerStatus.ALLIN);
            Assertions.assertThat(player1.getChip()).isSameAs(0);
            Assertions.assertThat(player1.getCurrentBetAmount()).isSameAs(beforePlayer1Chip);
            Assertions.assertThat(game.getTotalBetAmount()).isSameAs(beforePlayer1Chip);
        }

        @DisplayName("RAISE or CALL 상태가 아닌 유저는 AllIn을 할 수 없다.")
        @Test
        void allIn_user_DieStatus_willFail() {
            // given
            Game game = new Game(1L, GameType.P4);
            Player player1 = Player.create(1L, "testSession", "nickname", 1);
            Player player2 = Player.create(2L, "testSession2", "nickname2", 2);

            // when
            game.join(player1);
            game.join(player2);

            game.die(player1.getId());
            game.raise(player2.getId(), 10);

            // then
            Assertions.assertThatThrownBy(() -> game.allIn(player1.getId()))
                .isInstanceOf(IllegalStateException.class);
        }
    }

    @DisplayName("focus()은")
    @Nested
    public class Focus_Context {

        @DisplayName("현재 순서인 유저 정보를 반환한다.")
        @Test
        void focus_normal_willSuccess() {
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

    @DisplayName("nextTurn()은")
    @Nested
    public class NextTurn_Context {

        @DisplayName("일반적으로 다음 턴으로 넘긴다.")
        @Test
        void nextTurn_willSuccess() {
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
        void nextTurn_newTurn_willSuccess() {
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

        @DisplayName("만약 해당 순서의 유저가 나간상태라면 다음 유저로 이동한다")
        @Test
        void nextTurn_exitUser__willPass() {
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

        @DisplayName("만약 해당 순서의 유저가 DIE 상태라면 다음 유저로 이동한다")
        @Test
        void nextTurn_dieUser__willPass() {
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

            player3.changeStatus(PlayerStatus.DIE);
            player1.changeStatus(PlayerStatus.DIE);

            // then
            game.nextTurn();
            game.nextTurn();

            int currentTurn = game.getCurrentTurn();
            Assertions.assertThat(currentTurn).isSameAs(0);
        }
    }

    @DisplayName("isOpenTime()은")
    @Nested
    public class IsOpenTime_Context {

        @DisplayName("모든 플레이어가가 CALL, ALLIN, DIE 중 하나여야 한다")
        @Test
        void all_playerStatus_is_call_allIn_die_willSuccess(){
            // given
            Game game = new Game(1L, GameType.P4);
            Player player1 = Player.create(1L, "testSession", "nickname", 1);
            Player player2 = Player.create(2L, "testSession2", "nickname2", 2);
            Player player3 = Player.create(3L, "testSession3", "nickname3", 3);
            Player player4 = Player.create(4L, "testSession4", "nickname4", 4);

            game.join(player2);
            game.join(player3);
            game.join(player1);
            game.join(player4);

            // when
            player1.changeStatus(PlayerStatus.CALL);
            player2.changeStatus(PlayerStatus.DIE);
            player3.changeStatus(PlayerStatus.ALLIN);
            player4.changeStatus(PlayerStatus.CALL);

            // then
            Assertions.assertThat(game.isOpenTime()).isTrue();
        }

        @DisplayName("한명이라도 RAISE 상태이면 거짓을 반환한다")
        @Test
        void anyMatch_RAISE_willFALSE(){
            // given
            Game game = new Game(1L, GameType.P4);
            Player player1 = Player.create(1L, "testSession", "nickname", 1);
            Player player2 = Player.create(2L, "testSession2", "nickname2", 2);
            Player player3 = Player.create(3L, "testSession3", "nickname3", 3);
            Player player4 = Player.create(4L, "testSession4", "nickname4", 4);

            game.join(player2);
            game.join(player3);
            game.join(player1);
            game.join(player4);

            // when
            player1.changeStatus(PlayerStatus.RAISE);
            player2.changeStatus(PlayerStatus.DIE);
            player3.changeStatus(PlayerStatus.ALLIN);
            player4.changeStatus(PlayerStatus.CALL);

            // then
            Assertions.assertThat(game.isOpenTime()).isFalse();
        }
    }
}
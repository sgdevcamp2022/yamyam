package com.pokerservice.core.service;

import com.pokerservice.adapter.in.ws.GameManager;
import com.pokerservice.adapter.in.ws.message.PokerMessage;
import com.pokerservice.adapter.in.ws.message.content.serverContent.BetResponseContent;
import com.pokerservice.adapter.in.ws.message.content.clientContent.DieResponseContent;
import com.pokerservice.adapter.in.ws.message.content.serverContent.ExitContent;
import com.pokerservice.adapter.in.ws.message.content.serverContent.FocusContent;
import com.pokerservice.adapter.in.ws.message.content.serverContent.GameStartContent;
import com.pokerservice.adapter.in.ws.message.content.serverContent.JoinContent;
import com.pokerservice.adapter.in.ws.message.content.serverContent.PlayerInfo;
import com.pokerservice.adapter.in.ws.message.content.serverContent.PlayerResultInfo;
import com.pokerservice.adapter.in.ws.message.content.serverContent.ResultContent;
import com.pokerservice.core.domain.Game;
import com.pokerservice.core.domain.GameType;
import com.pokerservice.core.domain.Player;
import com.pokerservice.core.domain.PlayerStatus;
import com.pokerservice.core.port.in.GameMakeUseCase;
import com.pokerservice.core.port.in.GameUseCase;
import com.pokerservice.core.port.out.MessageSendPort;
import java.util.ArrayList;
import java.util.List;
import java.util.concurrent.atomic.AtomicLong;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.stereotype.Service;

@Service
public class GameService implements GameMakeUseCase, GameUseCase {

    private final Logger log = LoggerFactory.getLogger(GameService.class);
    private static final String POKER_ENDPOINT = "/topic/game/";

    private AtomicLong id = new AtomicLong(1);
    private final MessageSendPort messageSendPort;

    public GameService(MessageSendPort messageSendPort) {
        this.messageSendPort = messageSendPort;
    }

    @Override
    public long makeGame(GameType gameType) {
        Game game = new Game(id.getAndIncrement(), gameType);
        GameManager.activateGame(game);
        return game.getId();
    }

    @Override
    public void joinGame(long gameId, Player player) {
        Game game = GameManager.getActivateGame(gameId);
        game.join(player);

        if (game.isFull()) {
            game.getPlayers().forEach(p -> {
                PokerMessage<JoinContent> message = PokerMessage
                    .joinMessage(new JoinContent(p.getId(), p.getNickname(), p.getOrder()));
                log.info("send Message to {}, message: {}", player, message);
                messageSendPort.sendMessage(POKER_ENDPOINT + gameId, message);
            });
        }
    }

    @Override
    public boolean checkReady(long gameId, long playerId) {
        Game game = GameManager.getActivateGame(gameId);
        game.ready(playerId);

        return game.isAllReady();
    }

    @Override
    public void sendFocus(long gameId) {
        Game game = GameManager.getActivateGame(gameId);
        Player focusPlayer = game.focus();
        game.nextTurn();

        PokerMessage<FocusContent> message = PokerMessage.focusMessage(
            new FocusContent(focusPlayer.getId()));

        log.info("send Focus Message to everyone, message: {}", message);
        messageSendPort.sendMessage(POKER_ENDPOINT + gameId, message);
    }

    @Override
    public void drawCard(long gameId) {
        Game game = GameManager.getActivateGame(gameId);
        List<PlayerInfo> playerInfos = new ArrayList<>();
        game.drawCard();

        List<Player> players = game.getPlayers();
        players.forEach(p -> {
            playerInfos.add(new PlayerInfo(p.getId(), p.getChip(), p.getCard()));
        });
        GameStartContent content = new GameStartContent(playerInfos);
        PokerMessage<GameStartContent> message = PokerMessage.gameStartMessage(
            content);

        log.info("send Game_Start Message to everyone, message: {}", message);
        messageSendPort.sendMessage(POKER_ENDPOINT + gameId, message);
    }

    @Override
    public void betting(long gameId, long playerId, int betAmount) {
        Game game = GameManager.getActivateGame(gameId);

        if (!game.isPlayerTurn(playerId)) {
            throw new AssertionError("현재 플레이할 수 있는 턴이 아닙니다. 게임: " + gameId + ", 플레이어 : " + playerId);
        }

        game.betting(playerId, betAmount);

        Player player = game.findPlayerById(playerId);

        PokerMessage<BetResponseContent> message = PokerMessage.betMessage(
            new BetResponseContent(playerId, betAmount,
                player.getChip(),
                game.getTotalBetAmount()));

        log.info("send Betting Message to everyone, message: {}", message);
        messageSendPort.sendMessage(POKER_ENDPOINT + gameId, message);
    }

    @Override
    public void die(long gameId, long playerId) {
        Game game = GameManager.getActivateGame(gameId);

        if (!game.isPlayerTurn(playerId)) {
            throw new AssertionError("현재 플레이할 수 있는 턴이 아닙니다. 게임: " + gameId + ", 플레이어 : " + playerId);
        }

        game.die(playerId);

        if (checkOnlyOneLive(game)) {
            List<PlayerResultInfo> playerResultInfos = game.getPlayerResultInfos();

            messageSendPort.sendMessage(POKER_ENDPOINT + gameId,
                PokerMessage.resultMessage(new ResultContent(playerResultInfos)));
        } else {
            messageSendPort.sendMessage(POKER_ENDPOINT + gameId,
                PokerMessage.dieMessage(new DieResponseContent(gameId, playerId)));
        }
    }

    @Override
    public void exitGame(String socketUserId) {
        Player player = GameManager.findPlayerBySessionId(socketUserId);
        long gameId = player.getGameId();

        Game game = GameManager.getActivateGame(gameId);
        GameManager.removePlayer(socketUserId);
        game.exitGame(player);

        messageSendPort.sendMessage(POKER_ENDPOINT + gameId,
            PokerMessage.exitMessage(new ExitContent(player.getId(), player.getNickname())));
    }

    private boolean checkOnlyOneLive(Game game) {
        long livePlayerCount = game.getPlayers().stream()
            .filter(p -> p.getStatus() == PlayerStatus.PLAYING)
            .count();

        return livePlayerCount == 1;
    }
}

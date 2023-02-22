package com.pokerservice.core.service;

import com.pokerservice.adapter.in.ws.GameManager;
import com.pokerservice.adapter.in.ws.message.PokerMessage;
import com.pokerservice.adapter.in.ws.message.content.BetResponseContent;
import com.pokerservice.adapter.in.ws.message.content.DieContent;
import com.pokerservice.adapter.in.ws.message.content.FocusContent;
import com.pokerservice.adapter.in.ws.message.content.GameStartContent;
import com.pokerservice.adapter.in.ws.message.content.JoinContent;
import com.pokerservice.adapter.in.ws.message.content.PlayerInfo;
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
                    .joinMessage(new JoinContent(p.getUserId(), p.getNickname(), p.getOrder()));
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
            new FocusContent(focusPlayer.getUserId()));

        log.info("send Message to everyone, message: {}", message);
        messageSendPort.sendMessage(POKER_ENDPOINT + gameId, message);
    }

    @Override
    public void drawCard(long gameId) {
        Game game = GameManager.getActivateGame(gameId);
        List<PlayerInfo> playerInfos = new ArrayList<>();
        game.drawCard();

        List<Player> players = game.getPlayers();
        players.forEach(p -> {
            playerInfos.add(new PlayerInfo(p.getUserId(), p.getChip(), p.getCard()));
        });
        GameStartContent content = new GameStartContent(playerInfos);
        PokerMessage<GameStartContent> message = PokerMessage.gameStartMessage(
            content);

        log.info("send Message to everyone, message: {}", message);
        messageSendPort.sendMessage(POKER_ENDPOINT + gameId, message);
    }

    @Override
    public void betting(long gameId, long playerId, int betAmount) {
        Game game = GameManager.getActivateGame(gameId);
        game.betting(playerId, betAmount);

        Player player = game.findPlayerById(playerId);

        PokerMessage<BetResponseContent> message = PokerMessage.betMessage(
            new BetResponseContent(gameId, playerId, betAmount,
                player.getCurrentBetAmount(),
                game.getTotalBetAmount()));

        log.info("send Message to everyone, message: {}", player, message);
        messageSendPort.sendMessage(POKER_ENDPOINT + gameId, message);
    }

    @Override
    public void die(long gameId, long playerId) {
        Game game = GameManager.getActivateGame(gameId);
        game.die(playerId);

        if (checkOnlyOneLive(game)) {
            messageSendPort.sendMessage(POKER_ENDPOINT + gameId,
                PokerMessage.dieMessage(new DieContent(gameId, playerId)));
        }
    }

    private boolean checkOnlyOneLive(Game game) {
        long livePlayerCount = game.getPlayers().stream()
            .filter(p -> p.getStatus() == PlayerStatus.PLAYING)
            .count();

        return livePlayerCount == 1;
    }
}

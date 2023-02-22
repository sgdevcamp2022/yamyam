package com.pokerservice.core.domain;

import com.pokerservice.adapter.in.ws.message.content.serverContent.PlayerResultInfo;
import java.time.LocalDateTime;
import java.util.ArrayList;
import java.util.List;
import java.util.Optional;
import java.util.Random;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

public class Game {

    private final Logger log = LoggerFactory.getLogger(Game.class);
    private static final int MIN_CARD_VALUE = 1;
    private static final int MAX_CARD_VALUE = 10;
    private static final int MAX_TURN = 3;

    private final long id;
    private List<Player> players;
    private int currentTurn;
    private boolean[] gameCard = new boolean[10];
    private int minBetAmount = 10;
    private int totalBetAmount = 0;
    private int readyCount = 0;

    private final GameType gameType;
    private final LocalDateTime createdAt;
    private LocalDateTime updatedAt;
    private Player winner;

    public Game(long id, GameType gameType) {
        this.id = id;
        this.gameType = gameType;
        this.players = new ArrayList<>(gameType.getPlayerSize());
        this.createdAt = LocalDateTime.now();
    }

    public void join(Player player) {
        if (isFull()) {
            throw new AssertionError("game room already Full!  current: " + players.size());
        }

        player.setOrder(players.size());
        players.add(player.getOrder(), player);
        log.info("enter the game / gameId: {} players: {}/{}", id, players.size(),
            gameType.getPlayerSize());
    }

    public Player exitGame(Player player) {
        players.remove(player);
        return player;
    }

    public void settingGame() {
        // checking dead user
        players.stream()
            .filter(player -> player.getChip() + player.getCurrentBetAmount() == 0)
            .forEach(p -> p.changeStatus(PlayerStatus.LOOSE));



        currentTurn = 0;
        minBetAmount = 10;
        totalBetAmount = 0;
        gameCard = new boolean[10];
    }

    public void drawCard() {
        Random random = new Random();
        for (Player player : players) {
            while (true) {
                int pickCard = random.nextInt(MAX_CARD_VALUE) + MIN_CARD_VALUE;
                if (!gameCard[pickCard - 1]) {
                    log.info("---------- player {}, draw the card {}", player, pickCard);
                    player.setCardInfo(pickCard);
                    gameCard[pickCard - 1] = true;
                    break;
                }
            }
        }
    }

    public void betting(long playerId, int betAmount) {
        if (minBetAmount > betAmount) {
            throw new IllegalArgumentException("최소 배팅금액보다 적게 배팅할 수 없습니다.");
        }

        Player player = findPlayerById(playerId);

        if (player.getChip() <= minBetAmount) {
            betAmount = player.allIn();
            totalBetAmount += betAmount;
        } else if (player.getChip() < betAmount) {
            throw new IllegalArgumentException("소지한 금액보다 더 많은 금액을 배팅할 수 없습니다.");
        } else {
            player.betting(betAmount);
            totalBetAmount += betAmount;
        }
    }

    public void die(long playerId) {
        Player player = findPlayerById(playerId);
        player.die();
    }

    public Player findPlayerById(long playerId) {
        Player player = players.stream()
            .filter(p -> p.getId() == playerId)
            .findFirst()
            .orElseThrow(() -> new IllegalArgumentException("no player"));
        return player;
    }

    public Player focus() {
        Player player = null;

        for (Player currentPlayer : players) {
            if (currentPlayer.getOrder() == currentTurn) {
                player = currentPlayer;
                break;
            }
        }

        return player;
    }

    public void nextTurn() {
        if (currentTurn > players.size() - 1) {
            currentTurn = 0;
        } else {
            currentTurn++;
        }

        currentTurnUserExist();
    }

    private void currentTurnUserExist() {
        var wrapper = new Object() {
            boolean userExist;
        };

        players.forEach(p -> {
                if (p.getOrder() == currentTurn) {
                    wrapper.userExist = true;
                }
            }
        );

        if (!wrapper.userExist) {
            nextTurn();
        }
    }

    public void calcBattle(long winnerId) {
        Player winner = players.stream()
            .filter(p -> p.getId() == winnerId)
            .findFirst()
            .orElseThrow(() -> new IllegalArgumentException("no player"));

        winner.addChip(totalBetAmount);
    }

    public boolean isFull() {
        return gameType.getPlayerSize() == players.size();
    }

    public void ready(long id) {
        players.forEach(p -> {
            if (p.getId() == id) {
                readyCount++;
            }
        });
    }

    public boolean isAllReady() {
        return gameType.getPlayerSize() == readyCount;
    }

    public int getReadyCount() {
        return readyCount;
    }

    private void calcWinner() {
        Player winner = players.stream()
            .filter(p -> p.getStatus() == PlayerStatus.PLAYING)
            .findFirst()
            .get();
        this.winner = winner;
        winner.addChip(totalBetAmount);
    }

    public List<PlayerResultInfo> getPlayerResultInfos() {
        if (winner == null) {
            calcWinner();
        }

        return players.stream()
            .map(p -> {
                int result = 0;
                if (p.equals(winner)) {
                    result = 1;
                }
                return new PlayerResultInfo(p.getId(), result, p.getChip());
            })
            .toList();
    }

    public boolean isPlayerTurn(long playerId) {
        Player player = findPlayerById(playerId);
        return player.getOrder() == currentTurn;
    }

    public Logger getLog() {
        return log;
    }

    public List<Player> getPlayers() {
        return players;
    }

    public int getCurrentTurn() {
        return currentTurn;
    }

    public int getMaxTurn() {
        return MAX_TURN;
    }

    public int getMinBetAmount() {
        return minBetAmount;
    }

    public int getTotalBetAmount() {
        return totalBetAmount;
    }

    public LocalDateTime getCreatedAt() {
        return createdAt;
    }

    public LocalDateTime getUpdatedAt() {
        return updatedAt;
    }

    public long getId() {
        return id;
    }

    public GameType getGameType() {
        return gameType;
    }
}

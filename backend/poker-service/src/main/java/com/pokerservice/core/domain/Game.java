package com.pokerservice.core.domain;

import com.pokerservice.adapter.in.ws.message.content.serverContent.PlayerResultInfo;
import java.time.LocalDateTime;
import java.util.ArrayList;
import java.util.Comparator;
import java.util.List;
import java.util.Random;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

public class Game {

    private final Logger log = LoggerFactory.getLogger(Game.class);
    private static final int MIN_CARD_VALUE = 1;
    private static final int MAX_CARD_VALUE = 10;

    private final long id;
    private final int START_FEE = 10;
    private List<Player> players;
    private int currentTurn = 0;
    private boolean[] gameCard = new boolean[10];
    private int minBetAmount = 0;
    private int totalBetAmount = 0;
    private int readyCount = 0;
    private int matchTurn = 1;
    private List<Player> exileLooseUser = new ArrayList<>();

    private int lastRaiseIndex = -1;
    private final GameType gameType;
    private final LocalDateTime createdAt;
    private LocalDateTime updatedAt;
    private Player winner;
    private final boolean battle3D = false;

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
        resetData();

        chargeFee();

        settingUserStatus();
        exileLooseUser();
    }

    private void resetData() {
        currentTurn = 0;
        minBetAmount = 1;
        totalBetAmount = 0;
        gameCard = new boolean[10];
        exileLooseUser = new ArrayList<>(players.size());
    }

    private void chargeFee() {
        players.forEach(p -> {
            if (p.getChip() < START_FEE) {
                int chip = p.getChip();
                p.minusChip(chip);
                totalBetAmount += chip;
            } else {
                p.minusChip(START_FEE);
                totalBetAmount += START_FEE;
            }
        });
    }

    private void settingUserStatus() {
        players.stream()
            .forEach(p -> {
                if (p.getChip() == 0) {
                    p.changeStatus(PlayerStatus.LOOSE);
                } else {
                    p.changeStatus(PlayerStatus.PLAYING);
                }
            });
    }

    private List<Player> exileLooseUser() {
        players.stream()
            .filter(p -> p.getStatus() == PlayerStatus.LOOSE)
            .forEach(p -> {
                players.remove(p);
                exileLooseUser.add(p);
            });

        return exileLooseUser;
    }

    public void drawCard() {
        Random random = new Random();
        players.stream()
            .filter(p -> p.getStatus() == PlayerStatus.PLAYING)
            .forEach(p -> {
                while (true) {
                    int pickCard = random.nextInt(MAX_CARD_VALUE) + MIN_CARD_VALUE;
                    if (!gameCard[pickCard - 1]) {
                        log.info("---------- player {}, draw the card {}", p, pickCard);
                        p.setCardInfo(pickCard);
                        gameCard[pickCard - 1] = true;
                        break;
                    }
                }
            });
    }

    public void raise(long playerId, int betAmount) {
        if (minBetAmount > betAmount) {
            throw new IllegalArgumentException("최소 배팅금액보다 적게 배팅할 수 없습니다.");
        }

        Player player = findPlayerById(playerId);
        if (!player.canRaise()) {
            throw new IllegalStateException("RAISE를 할 수 없는 상태입니다. id: " +
                playerId + ", 상태: " + player.getStatus());
        }
        player.betting(betAmount);

        totalBetAmount += betAmount;
        minBetAmount = betAmount;
        player.changeStatus(PlayerStatus.RAISE);
        lastRaiseIndex = player.getOrder();
    }

    public void call(long playerId) {
        Player player = findPlayerById(playerId);

        if (!player.canCall()) {
            throw new IllegalStateException("CALL을 할 수 없는 상태입니다. id: " +
                playerId + ", 상태: " + player.getStatus());
        }
        player.betting(minBetAmount);
        player.changeStatus(PlayerStatus.CALL);

        totalBetAmount += minBetAmount;
    }

    public void allIn(long playerId) {
        Player player = findPlayerById(playerId);

        if (!player.canAllIn()) {
            throw new IllegalStateException("CALL을 할 수 없는 상태입니다. id: " +
                playerId + ", 상태: " + player.getStatus());
        }

        player.betting(player.getChip());
        player.changeStatus(PlayerStatus.ALLIN);
        totalBetAmount += player.getCurrentBetAmount();
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

    public boolean isOpenTime() {
        return players.stream()
            .allMatch(p -> p.getStatus() == PlayerStatus.CALL ||
                p.getStatus() == PlayerStatus.ALLIN ||
                p.getStatus() == PlayerStatus.DIE);
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
        if (currentTurn > gameType.getPlayerSize() - 1) {
            currentTurn = 0;
        } else {
            currentTurn++;
        }

        isPlayable();
    }

    private void isPlayable() {
        var wrapper = new Object() {
            boolean userExist = false;
        };

        for (Player player : players) {
            if (player.getOrder() == currentTurn) {
                if (player.canCall()) {
                    wrapper.userExist = true;
                    break;
                }
            }
        }

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
        return gameType.getPlayerSize() <= players.size();
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

    private void summary() {
        Player winner = players.stream()
            .filter(p -> p.getStatus() == PlayerStatus.RAISE ||
                p.getStatus() == PlayerStatus.CALL ||
                p.getStatus() == PlayerStatus.ALLIN
            ).max(Comparator.comparingInt(Player::getCard))
            .get();
        this.winner = winner;
        winner.addChip(totalBetAmount);
    }

    public List<PlayerResultInfo> getPlayerResultInfos() {
        if (winner == null) {
            summary();
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

    public List<Player> getPlayers() {
        return players;
    }

    public int getCurrentTurn() {
        return currentTurn;
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

    public int getLastRaiseIndex() {
        return lastRaiseIndex;
    }
}

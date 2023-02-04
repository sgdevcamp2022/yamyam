package com.pokerservice.core.domain;

import com.pokerservice.controller.ws.GameMessage;
import com.pokerservice.controller.ws.GameMessage.MessageType;
import java.time.LocalDateTime;
import java.util.Random;
import java.util.Set;
import java.util.concurrent.ConcurrentHashMap;
import java.util.logging.Logger;

public class Game {

    private final Logger logger = Logger.getLogger("Game");

    private long id;
    private final int MIN_CARD_VALUE = 1;
    private final int MAX_CARD_VALUE = 10;
    private final GameType gameType;
    private final int maxTurn = 3;

    private Set<Player> players;
    private int currentTurn;
    private int[] gameCard = new int[10];
    private GameState gameState = GameState.WAIT;
    private int betAmount = 10;
    private int totalBetAmount = 0;

    private final LocalDateTime createdAt;
    private LocalDateTime updatedAt;

    public Game(GameType gameType) {
        this.gameType = gameType;
        this.players = ConcurrentHashMap.newKeySet(gameType.getPlayerCount());
        this.createdAt = LocalDateTime.now();
    }

    public static Game create2PGame() {
        return new Game(GameType.P2);
    }

    public static Game create4PGame() {
        return new Game(GameType.P4);
    }

    public void enterGame(Player player) {
        players.add(player);
    }

    public void exitGame(Player player) {
        players.remove(player);
    }

    public void settingGame() {
        // checking dead user
        players.stream()
            .filter(player -> player.getChip() + player.getCurrentBetAmount() == 0)
            .forEach(p -> p.changeStatus(PlayerStatus.DIE));

        currentTurn = 0;
        betAmount = 10;
        totalBetAmount = 0;
        gameCard = new int[10];
    }

    public void drawCard() {
        gameState = GameState.CARD;
        Random random = new Random();

        for (Player player : players) {
            int pickCard = random.nextInt(MAX_CARD_VALUE) + MIN_CARD_VALUE;
            while (++gameCard[pickCard] >= 1) {
                player.drawCard(pickCard);

                GameMessage sendMyCard = GameMessage.createDrawMessage(player.getSeatNo(), pickCard);
                sendMyCard.send(player, "/topic/public/" + id);
            }
        }
    }

    public void gameStart() {
        if (isFull()) {
            gameState = GameState.START;

            totalBetAmount = 0;
            GameMessage gameMessage = GameMessage.createStartMessage();
            gameMessage.sendAll(players, "/topic/public/" + id);
        }
    }

    public void betting() {
        for (Player player : players) {
            player.updateChip(-betAmount);

            GameMessage message = GameMessage.createBettingMessage(betAmount,
                player.getChip(), player.getSeatNo());
            totalBetAmount += betAmount;
            message.sendAll(players, "/topic/public/" + id);
        }
    }

    public void focus(int focusSeat) {
        GameMessage message = GameMessage.createFocusMessage(focusSeat);
        message.sendAll(players, "/topic/public/" + id);
    }

    public void requestAction() {
        int idx = 0;
        int timeout = 15;

        for (Player player : players) {
            idx += 1;
            GameMessage message = new GameMessage(MessageType.BATTLE, null, null,0, 0);
            message.send(player, "/topic/public/" + id);

            focus(player.getSeatNo());
        }
    }

    public void turn(int turn) {
        this.currentTurn = turn;
        logger.info("========= turn: " + turn + " =========");

        if (currentTurn >= 1 && currentTurn <= 3) {
            requestAction();
        }
    }

    private void openCard() {
        gameState = GameState.RESULT;
    }

    public boolean isFull() {
        return gameType.getPlayerCount() == players.size();
    }

    public Logger getLogger() {
        return logger;
    }

    public int getMIN_CARD_VALUE() {
        return MIN_CARD_VALUE;
    }

    public int getMAX_CARD_VALUE() {
        return MAX_CARD_VALUE;
    }

    public Set<Player> getPlayers() {
        return players;
    }

    public int getCurrentTurn() {
        return currentTurn;
    }

    public int getMaxTurn() {
        return maxTurn;
    }

    public int[] getGameCard() {
        return gameCard;
    }

    public GameState getGameState() {
        return gameState;
    }

    public int getBetAmount() {
        return betAmount;
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

    public void setId(long id) {
        this.id = id;
    }
}

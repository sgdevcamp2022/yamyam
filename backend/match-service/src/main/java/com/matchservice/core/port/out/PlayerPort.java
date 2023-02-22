package com.matchservice.core.port.out;

import com.matchservice.core.domain.Player;
import java.util.List;

public interface PlayerPort {

    void save(Player player);

    void remove(Player player);

    Player find(Player player);

    List<Player> findAll();

    boolean isExist(Player player);
}

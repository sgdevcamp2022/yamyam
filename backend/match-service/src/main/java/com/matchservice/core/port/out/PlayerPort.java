package com.matchservice.core.port.out;

import com.matchservice.core.domain.Player;

public interface PlayerPort {

    void save(Player player);

    void remove(Player player);

    Player find(Player player);

    boolean isExist(Player player);
}

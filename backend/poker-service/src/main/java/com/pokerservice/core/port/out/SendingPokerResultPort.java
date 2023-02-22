package com.pokerservice.core.port.out;

import com.pokerservice.core.domain.Player;
import java.util.List;

public interface SendingPokerResultPort {

    void sendToPlayerInfo(List<Player> players);

}

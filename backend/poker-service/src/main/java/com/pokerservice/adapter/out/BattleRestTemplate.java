package com.pokerservice.adapter.out;


import com.pokerservice.core.domain.Player;
import com.pokerservice.core.port.out.SendingPokerResultPort;
import java.util.List;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.http.ResponseEntity;
import org.springframework.stereotype.Component;
import org.springframework.web.client.RestTemplate;

@Component
class BattleRestTemplate implements SendingPokerResultPort {

    private final RestTemplate restTemplate;

    @Value("url.battle")
    private String battleServerURL;

    BattleRestTemplate() {
        this.restTemplate = new RestTemplate();
    }

    @Override
    public void sendToPlayerInfo(List<Player> players) {
        ResponseEntity<Void> responseEntity = restTemplate.postForEntity(
            battleServerURL + "/battle", players, Void.class);
    }
}

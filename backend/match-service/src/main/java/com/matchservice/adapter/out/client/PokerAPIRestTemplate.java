package com.matchservice.adapter.out.client;

import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.matchservice.core.domain.match.Match.GameType;
import com.matchservice.core.port.out.GameMakeUseCase;
import java.lang.reflect.ParameterizedType;
import org.apache.coyote.Response;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.core.ParameterizedTypeReference;
import org.springframework.http.HttpEntity;
import org.springframework.http.HttpHeaders;
import org.springframework.http.MediaType;
import org.springframework.http.ResponseEntity;
import org.springframework.stereotype.Component;
import org.springframework.util.LinkedMultiValueMap;
import org.springframework.util.MultiValueMap;
import org.springframework.web.client.RestTemplate;

@Component
public class PokerAPIRestTemplate implements GameMakeUseCase {

    private RestTemplate restTemplate;

    @Value("${url.poker-service}")
    private String pokerServiceURL;

    public PokerAPIRestTemplate() {
        this.restTemplate = new RestTemplate();
    }

    public long requestMakeGame(GameType gameType) {
        HttpHeaders headers = new HttpHeaders();
        headers.setContentType(MediaType.APPLICATION_FORM_URLENCODED);

        MultiValueMap<String, String> map= new LinkedMultiValueMap<String, String>();
        map.add("gameType", gameType.toString());

        HttpEntity<MultiValueMap<String, String>> request = new HttpEntity<MultiValueMap<String, String>>(map, headers);

        ResponseEntity<Long> responseEntity = restTemplate.postForEntity(pokerServiceURL + "/game",
            request, Long.class);

        Long gameId = responseEntity.getBody();
        if (gameId == null) {
            throw new AssertionError("Connection Error With Poker Server");
        }

        return gameId;
    }
}

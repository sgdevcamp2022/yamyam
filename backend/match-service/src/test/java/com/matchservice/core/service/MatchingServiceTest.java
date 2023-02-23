package com.matchservice.core.service;

import com.matchservice.core.port.out.MatchPort;
import com.matchservice.core.port.out.MatchQueuePort;
import com.matchservice.core.port.out.PlayerPort;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.context.SpringBootTest;
import org.springframework.messaging.simp.SimpMessagingTemplate;

@SpringBootTest
class MatchingServiceTest {

    @Autowired
    private MatchingService matchingService;

    @Autowired
    private MatchPort matchPort;

    @Autowired
    private MatchQueuePort matchQueuePort;

    @Autowired
    private PlayerPort playerPort;

    @Autowired
    private SimpMessagingTemplate template;
}
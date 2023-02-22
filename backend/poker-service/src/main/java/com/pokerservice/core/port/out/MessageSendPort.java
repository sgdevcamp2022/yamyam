package com.pokerservice.core.port.out;

import com.pokerservice.adapter.in.ws.message.PokerMessage;

public interface MessageSendPort {

    <T extends PokerMessage> void sendMessage(String destination, T payload);
}

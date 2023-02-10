package com.pokerservice;

import com.pokerservice.controller.ws.GameMessage.MessageType;
import java.util.Random;
import org.junit.jupiter.api.Assertions;
import org.junit.jupiter.api.Test;

public class RandomTest {

    @Test
    void randomTest(){
        int min = 1;
        int max = 10;
        Random random = new Random();
        for (int i = 0; i < 100; i++) {
            System.out.println(random.nextInt((max - min) + min) + min);
        }
    }

    @Test
    void enumToStringTest(){
        // given
        MessageType join = MessageType.valueOf("JOIN");
        MessageType draw = MessageType.valueOf("DRAW");

        // when
        Assertions.assertTrue(join == MessageType.JOIN);
        Assertions.assertFalse(join == MessageType.DRAW);

        // then
    }
}

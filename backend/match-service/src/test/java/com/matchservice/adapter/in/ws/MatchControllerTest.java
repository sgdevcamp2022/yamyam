package com.matchservice.adapter.in.ws;

import static org.springframework.restdocs.mockmvc.MockMvcRestDocumentation.document;
import static org.springframework.restdocs.payload.PayloadDocumentation.fieldWithPath;
import static org.springframework.restdocs.payload.PayloadDocumentation.responseFields;
import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.get;
import static org.springframework.test.web.servlet.result.MockMvcResultMatchers.status;

import com.matchservice.common.MessageFrameHandler;
import com.matchservice.common.StompSupport;
import com.matchservice.core.domain.Lobby;
import com.matchservice.core.domain.message.MatchMessage;
import com.matchservice.core.domain.message.MatchMessage.MessageType;
import org.assertj.core.api.Assertions;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.autoconfigure.restdocs.AutoConfigureRestDocs;
import org.springframework.boot.test.autoconfigure.web.servlet.AutoConfigureMockMvc;
import org.springframework.test.web.servlet.MockMvc;

@AutoConfigureMockMvc
@AutoConfigureRestDocs
class MatchControllerTest extends StompSupport {

    @Autowired
    private MockMvc mockMvc;

    @Autowired
    private Lobby lobby;

//    @Test
//    @DisplayName("매칭 서비스에 연결되면 임시 세션 스토리지에 저장된다.")
//    void connectTest(){
//        // given
//        String sessionId = stompSession.getSessionId();
//        System.out.println(sessionId);
//
//        Assertions.assertThat(stompSession.isConnected()).isTrue();
//        Assertions.assertThat(Lobby.getPlayerOperation(sessionId)).isNotNull();
//    }
//
//
//    @Test
//    @DisplayName("유저는 매칭요청을 보낼 수 있다.")
//    public void matching_request_willSuccess() throws Exception {
//
//        MatchMessage matchMessage = new MatchMessage(MessageType.MATCH);
//        matchMessage.addContent("match_type", "4p");
//        matchMessage.setSender("aaa");
//        matchMessage.setUserId(1L);
//        stompSession.subscribe("/user/topic/match",
//            new MessageFrameHandler<>(MatchMessage.class));
//
//        stompSession.send("/pub/v1/match", matchMessage);
//
//        this.mockMvc.perform(get("/match/matches/{gameType}", "P4"))
//            .andExpect(status().isOk())
//            .andDo(document("match-request", responseFields(
//                fieldWithPath("match[].id").description("PHONE ID DESCRIPTION"),
//                fieldWithPath("match[].gameType").description("PHONE ID DESCRIPTION"))))
//            .andReturn().getResponse().getContentAsString();
//
//        stompSession.disconnect();
//    }
}
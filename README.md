# camp2022_yamyam

2022스마일게이트 윈터데브캠프에서 진행한 YamYam팀의 NoPoker프로젝트 레파지토리입니다.

NoPoker은 NoSQL에서 이름을 따온것으로, Not Only Poker 입니다.
Poker게임뿐만이 아니라 Action게임까지 즐길 수 있는 뇌지컬과 피지컬을 필요로하는 보드액션게임입니다.

### 게임규칙
#### Phase 1. 인디언 포커
랜덤으로 카드를 뽑고, 다른 플레이어들의 카드와 배팅을 보고 본인의 배팅을 결정
#### Phase 2. 난투
- 카드 숫자를 바탕으로 무기를 지급 받음(1단계 ~ 10단계)
- 난투에서 살아남은 1인이 배팅된 칩을 모두 가져감
- 다시 Phase1으로 이동
<br></br>
최후의 1인이 남을때까지 반복하며, 초기자금이 0이되면 패배하면 로비로 이동

***
### 기술스택
- [백엔드](https://github.com/sgdevcamp2022/yamyam/tree/main/backend)
  - SpringBoot
  - Django
  - PostgreSQL
  - redis
  - NginX

  
- [클라이언트](https://github.com/sgdevcamp2022/yamyam/tree/main/client/NoPOKER)
  - 유니티
  - Photon
***

### 기능
- 인증
  - [백엔드](https://github.com/sgdevcamp2022/yamyam/tree/main/backend/auth_server)
  - [클라이언트](https://github.com/sgdevcamp2022/yamyam/tree/main/client/NoPOKER/Assets/Script/2D/Login)
- 로비
  - [백엔드](https://github.com/sgdevcamp2022/yamyam/tree/main/backend/lobby_server)
  - [클라이언트](https://github.com/sgdevcamp2022/yamyam/tree/main/client/NoPOKER/Assets/Script/2D/Lobby)
- 매칭
  - [백엔드](https://github.com/sgdevcamp2022/yamyam/tree/main/backend/match-service)
  - [클라이언트](https://github.com/sgdevcamp2022/yamyam/tree/main/client/NoPOKER/Assets/Script/2D/Match)
- 2D포커
  - [백엔드](https://github.com/sgdevcamp2022/yamyam/tree/main/backend/poker-service)
  - [클라이언트](https://github.com/sgdevcamp2022/yamyam/tree/main/client/NoPOKER/Assets/Script/2D/Poker)
- 3D액션
  - [백엔드](https://github.com/sgdevcamp2022/yamyam/tree/main/client/NoPOKER/Assets/Photon)
  - [클라이언트](https://github.com/sgdevcamp2022/yamyam/tree/main/client/NoPOKER/Assets/Script/3D)
***
### 화면
#### 인증화면
![인증화면](https://user-images.githubusercontent.com/67956564/221084224-e14a1255-4c58-4853-8cbc-558db7e5784c.png)

#### 로비화면
![로비화면](https://user-images.githubusercontent.com/67956564/221084340-26c96519-b77d-4cc5-a4c1-b2f99bc43a02.png)

#### 매칭화면
![매칭화면](https://user-images.githubusercontent.com/67956564/221084424-0ef52faa-1a01-426a-a1d8-02d0adcb75f7.png)

#### 포커게임화면
![포커화면](https://user-images.githubusercontent.com/67956564/221084448-e43aea59-9714-4b35-a101-724b4e691dc5.png)

#### 액션게임화면
![3D화면](https://user-images.githubusercontent.com/67956564/221084483-2f699461-45cf-494f-8b08-c184b4c5b13f.png)

***
### 패키지 구조
```
├── README.md
├── backend
└── client
```

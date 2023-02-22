FROM gradle:jdk17-focal

WORKDIR /usr/src/match

COPY . .
RUN gradle clean build
ARG JAR_FILE_PATH=build/libs/*-SNAPSHOT.jar
COPY ${JAR_FILE_PATH} match.jar

ENTRYPOINT ["java", "-jar", "match.jar", "--url.poker-service=http://localhost:8004"]
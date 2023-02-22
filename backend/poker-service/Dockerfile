FROM gradle:jdk17-focal

WORKDIR /usr/src/poker

COPY . .
RUN gradle clean build
ARG JAR_FILE_PATH=./build/libs/*-SNAPSHOT.jar
COPY ${JAR_FILE_PATH} poker.jar

ENTRYPOINT ["java", "-jar", "poker.jar"]
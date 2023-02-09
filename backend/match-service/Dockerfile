FROM gradle:jdk17-focal

COPY . .
RUN gradle clean build
ARG JAR_FILE_PATH=build/libs/*-SNAPSHOT.jar
COPY ${JAR_FILE_PATH} app.jar

ENTRYPOINT ["java", "-jar", "app.jar"]
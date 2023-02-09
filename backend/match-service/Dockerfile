FROM openjdk:17-oracle
CMD ["./gradlew", "clean", "package"]
ARG JAR_FILE_PATH=./build/libs/match-service-0.0.1-SNAPSHOT.jar
COPY ${JAR_FILE_PATH} app.jar
ENTRYPOINT ["java", "-jar", "app.jar"]
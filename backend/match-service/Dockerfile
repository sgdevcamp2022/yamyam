FROM gradle:jdk17-focal AS build

#RUN microdnf install findutils

WORKDIR /workspace/app
COPY . /workspace/app
RUN gradle clean build
RUN mkdir -p build/dependency && (cd build/dependency; jar -xf ../libs/*-SNAPSHOT.jar)

FROM gradle:jdk17-focal
VOLUME /tmp
ARG DEPENDENCY=/workspace/app/build/dependency

COPY --from=build ${DEPENDENCY} ./app.jar

ENTRYPOINT ["java", "-jar", "app.jar"]
package com.matchservice.config;

import java.security.Principal;

public class StompPrincipal implements Principal {

    long id;
    String name;

    public StompPrincipal(String name) {
        this.name = name;
    }

    public long getId() {
        return id;
    }

    @Override
    public String getName() {
        return name;
    }
}
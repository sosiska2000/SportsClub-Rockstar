package com.example.rockstarmobile.models;

import java.io.Serializable;

public class ServiceType implements Serializable {
    private int id;
    private int directionId;
    private String name;
    private String description;
    private int defaultDuration;
    private boolean isActive;

    public ServiceType() {}

    public ServiceType(int id, int directionId, String name, String description, int defaultDuration) {
        this.id = id;
        this.directionId = directionId;
        this.name = name;
        this.description = description;
        this.defaultDuration = defaultDuration;
        this.isActive = true;
    }

    public int getId() { return id; }
    public void setId(int id) { this.id = id; }

    public int getDirectionId() { return directionId; }
    public void setDirectionId(int directionId) { this.directionId = directionId; }

    public String getName() { return name; }
    public void setName(String name) { this.name = name; }

    public String getDescription() { return description; }
    public void setDescription(String description) { this.description = description; }

    public int getDefaultDuration() { return defaultDuration; }
    public void setDefaultDuration(int defaultDuration) { this.defaultDuration = defaultDuration; }

    public boolean isActive() { return isActive; }
    public void setActive(boolean active) { isActive = active; }

    public String getDisplayName() {
        return name + " (" + defaultDuration + " мин)";
    }
}
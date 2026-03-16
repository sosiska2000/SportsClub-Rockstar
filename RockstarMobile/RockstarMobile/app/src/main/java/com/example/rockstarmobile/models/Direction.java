package com.example.rockstarmobile.models;

import java.io.Serializable;
import java.util.ArrayList;
import java.util.List;

public class Direction implements Serializable {
    private int id;
    private String name;
    private String nameKey;
    private String description;
    private boolean isActive;
    private String createdAt;
    private List<Service> services;
    private List<Subscription> subscriptions;

    // Конструкторы
    public Direction() {
        this.services = new ArrayList<>();
        this.subscriptions = new ArrayList<>();
    }

    public Direction(int id, String name, String nameKey, String description) {
        this.id = id;
        this.name = name;
        this.nameKey = nameKey;
        this.description = description;
        this.isActive = true;
        this.services = new ArrayList<>();
        this.subscriptions = new ArrayList<>();
    }

    // Геттеры и сеттеры
    public int getId() { return id; }
    public void setId(int id) { this.id = id; }

    public String getName() { return name; }
    public void setName(String name) { this.name = name; }

    public String getNameKey() { return nameKey; }
    public void setNameKey(String nameKey) { this.nameKey = nameKey; }

    public String getDescription() { return description; }
    public void setDescription(String description) { this.description = description; }

    public boolean isActive() { return isActive; }
    public void setActive(boolean active) { isActive = active; }

    public String getCreatedAt() { return createdAt; }
    public void setCreatedAt(String createdAt) { this.createdAt = createdAt; }

    public List<Service> getServices() { return services; }
    public void setServices(List<Service> services) { this.services = services; }

    public List<Subscription> getSubscriptions() { return subscriptions; }
    public void setSubscriptions(List<Subscription> subscriptions) { this.subscriptions = subscriptions; }

    public int getServicesCount() {
        return services != null ? services.size() : 0;
    }

    public int getSubscriptionsCount() {
        return subscriptions != null ? subscriptions.size() : 0;
    }

    public String getDirectionColor() {
        switch (nameKey) {
            case "yoga":
                return "#FF9F4D";
            case "fitness":
                return "#4CAF50";
            case "climbing":
                return "#2196F3";
            default:
                return "#9C27B0";
        }
    }
}
package com.example.rockstarmobile.models;

import java.io.Serializable;

public class Subscription implements Serializable {
    private int id;
    private String name;
    private Integer directionId;
    private String directionName;
    private String directionKey;
    private double price;
    private int sessionsCount;
    private String description;
    private boolean isActive;
    private String createdAt;

    // Конструкторы
    public Subscription() {}

    public Subscription(int id, String name, Integer directionId, double price,
                        int sessionsCount, String description) {
        this.id = id;
        this.name = name;
        this.directionId = directionId;
        this.price = price;
        this.sessionsCount = sessionsCount;
        this.description = description;
        this.isActive = true;
    }

    // Геттеры и сеттеры
    public int getId() { return id; }
    public void setId(int id) { this.id = id; }

    public String getName() { return name; }
    public void setName(String name) { this.name = name; }

    public Integer getDirectionId() { return directionId; }
    public void setDirectionId(Integer directionId) { this.directionId = directionId; }

    public String getDirectionName() { return directionName; }
    public void setDirectionName(String directionName) { this.directionName = directionName; }

    public String getDirectionKey() { return directionKey; }
    public void setDirectionKey(String directionKey) { this.directionKey = directionKey; }

    public double getPrice() { return price; }
    public void setPrice(double price) { this.price = price; }

    public int getSessionsCount() { return sessionsCount; }
    public void setSessionsCount(int sessionsCount) { this.sessionsCount = sessionsCount; }

    public String getDescription() { return description; }
    public void setDescription(String description) { this.description = description; }

    public boolean isActive() { return isActive; }
    public void setActive(boolean active) { isActive = active; }

    public String getCreatedAt() { return createdAt; }
    public void setCreatedAt(String createdAt) { this.createdAt = createdAt; }

    // Вспомогательные методы
    public String getPriceDisplay() {
        return String.format("%.0f ₽", price);
    }

    public String getSessionsDisplay() {
        if (sessionsCount > 1) {
            return sessionsCount + " занятий";
        } else {
            return "Разовое";
        }
    }

    public String getDirectionDisplay() {
        return directionName != null ? directionName : "Без направления";
    }

    public String getDirectionColor() {
        if (directionKey == null) return "#9C27B0";
        switch (directionKey) {
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
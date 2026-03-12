package com.example.rockstarmobile.models;

import java.io.Serializable;

public class Service implements Serializable {
    private int id;
    private int directionId;
    private String name;
    private double price;
    private int sessionsCount;
    private Integer durationMinutes;
    private String description;
    private boolean isActive;
    private String createdAt;

    // Конструкторы
    public Service() {}

    public Service(int id, int directionId, String name, double price,
                   int sessionsCount, Integer durationMinutes, String description) {
        this.id = id;
        this.directionId = directionId;
        this.name = name;
        this.price = price;
        this.sessionsCount = sessionsCount;
        this.durationMinutes = durationMinutes;
        this.description = description;
        this.isActive = true;
    }

    // Геттеры и сеттеры
    public int getId() { return id; }
    public void setId(int id) { this.id = id; }

    public int getDirectionId() { return directionId; }
    public void setDirectionId(int directionId) { this.directionId = directionId; }

    public String getName() { return name; }
    public void setName(String name) { this.name = name; }

    public double getPrice() { return price; }
    public void setPrice(double price) { this.price = price; }

    public int getSessionsCount() { return sessionsCount; }
    public void setSessionsCount(int sessionsCount) { this.sessionsCount = sessionsCount; }

    public Integer getDurationMinutes() { return durationMinutes; }
    public void setDurationMinutes(Integer durationMinutes) { this.durationMinutes = durationMinutes; }

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

    public String getDurationDisplay() {
        if (durationMinutes != null) {
            return durationMinutes + " мин";
        } else {
            return "—";
        }
    }

    public String getDisplayName() {
        return name + " - " + getPriceDisplay();
    }
}